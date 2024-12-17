namespace BlImplementation;

using BO;
using Helpers;
using System;

internal class VolunteerImplementation : BlApi.IVolunteer
{
    // The implemention of the function in the observer 
    #region Stage 5
    public void AddObserver(Action listObserver) =>
        VolunteerManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
        VolunteerManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
        VolunteerManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
        VolunteerManager.Observers.RemoveObserver(id, observer); //stage 5
    #endregion Stage 5


    //The contract which we will make all the action with when using the Dal layer
    private readonly DalApi.IDal s_dal = DalApi.Factory.Get;

    /// <summary>
    /// This method accepts a well defiend BO.Volunteer variabel which is needed to be added to the dataabse
    /// It would check its logics again (like in the update action)
    /// It would check its format again (like in the update action)
    /// It would create a new DO.Volunteer entity using the volunteer's values,
    /// Then it would call the Create action from the DAL layer
    /// If such a volunteer already exists it would handle the thrown excpetion by throwing a new exception to the upper layers
    /// </summary>
    /// <param name="volunteer"></param>
    /// <exception cref="BlAlreadyExistsException">A</exception>
    public void AddVolunteer(BO.Volunteer volunteer)
    {
        try
        {
            //Check logics and formmating
            if (!Helpers.VolunteerManager.IsVolunteerValid(volunteer))
            {
                throw new BO.BlInvalidEntityDetails($"BL Error: volunteer {volunteer.Id} fields are invalid");
            }

            (double? lat, double? lng) = (null, null);
            if(volunteer.FullCurrentAddress != null)
            {
                (lat,lng) = Helpers.VolunteerManager.GetGeoCordinates(volunteer.FullCurrentAddress!);
            }

            //Create Dal Volunteer entity
            DO.Volunteer newVolunteer = new DO.Volunteer
            {
                Id = volunteer.Id,
                Role = (DO.UserRole)Enum.Parse(typeof(DO.UserRole), volunteer.Role.ToString()),
                FullName = volunteer.FullName,
                PhoneNumber = volunteer.PhoneNumber,
                Email = volunteer.Email,
                MaxDistanceToCall = volunteer.MaxDistanceToCall,
                RangeType = (DO.TypeOfRange)Enum.Parse(typeof(DO.TypeOfRange), volunteer.RangeType.ToString()),
                IsActive = volunteer.IsActive,
                Password = volunteer.Password == null
                        ? null
                        : VolunteerManager.GetSHA256HashedPassword(volunteer.Password),
                FullCurrentAddress = volunteer.FullCurrentAddress,
                Latitude = lat,
                Longitude = lng
            };

            //Add the new entity to the database
            s_dal.Volunteer.Create(newVolunteer);

        }
        catch (DO.DalAlreadyExistsException ex)
        {
            //Throw new BL excpetion to the upper layers
            throw new BO.BlAlreadyExistsException($"BL-DAL: Such object already exists in the database", ex);
        }
    }

    /// <summary>
    /// This method accespts an id value, requests from the DAL layer such a volunteer with that id, to check if it is allowed to be deleted
    /// a volunteer is allowed to be deleted if it never and would ever hadle a call
    /// If the action is now allowed it would throw an exception to the upper layer
    /// If the volunteer hasn't been found it would handler the thrown exception by throwing a new one to the upper layer
    /// </summary>
    /// <param name="id">The requested volunteer's id value</param>
    public void DeleteVolunteer(int id)
    {
        //Tries to find such volunteer
        DO.Volunteer volunteer = s_dal.Volunteer.Read(id)
            ?? throw new BO.BlDoesNotExistException($"BL: Error while tyring to remove the volunteer {id}");

        //Checks if the volunteer is in any records of assignments
        if (s_dal.Assignment.Read((DO.Assignment assignment) => assignment.VolunteerId == id) != null)
        {
            throw new BO.BlEntityRecordIsNotEmpty($"BL: Unable to remove the volunteer {id} due to that it has references in other assignment records");
        }

        //Tries to remove if the volunteer exists
        try
        {
            s_dal.Volunteer.Delete(id);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"BL-Dal: Volunteer {id} doesn't exists", ex);
        }

    }

    /// <summary>
    /// This method accepts an id value, calls the Read method from DAL, using the returned value it creates a new BO.Volunteer entity,
    /// also, the method adds a related call to the volunteer in the BO.CallInProgress field, adds to the created BO.Volunteer entity and retuns it.
    /// If such volunteer hasn't been found, the system will throw an exception which would be taken care of in the BL layer and which then will throw an exception to the upper layer
    /// </summary>
    /// <param name="id">The requersted volunteer's id</param>
    /// <returns>An BO.Volunteer entity which contains the requested entity's values and the current in progress call which is taken care of by the volunteer</returns>
    public BO.Volunteer GetVolunteerDetails(int id)
    {
        DO.Volunteer volunteer = s_dal.Volunteer.Read(id)
            ?? throw new BO.BlDoesNotExistException($"BL: Volunteer with id of {id} doesn't exists");

        DO.Assignment? volunteerAssignment = s_dal.Assignment.Read(assignment => assignment.VolunteerId == volunteer.Id);
        BO.CallInProgress? volunteerCallInProgress = null;

        //If there is an assingment - there is a call to create
        if (volunteerAssignment != null)
        {
            //Get the Dal call
            DO.Call volunteerCall = s_dal.Call.Read(call => call.Id == volunteerAssignment.CallId)
                ?? throw new BO.BlDoesNotExistException($"BL: UNWANTED: There is not call with id of {volunteerAssignment.CallId}");

            //Calculate the distance
            double distance = -1.0;
            if (volunteer.FullCurrentAddress != null)
            {
                distance = VolunteerManager.CalculateDistanceFromVolunteerToCall(volunteer.FullCurrentAddress!, volunteerCall.FullAddressCall, volunteer.RangeType);
            }

            //Create the CallInProgress intance for the Volunteer's field
            volunteerCallInProgress = new BO.CallInProgress
            {
                Id = volunteerAssignment.Id,
                CallId = volunteerCall.Id,
                OpenningTime = volunteerCall.OpeningTime,
                Description = volunteerCall.Description,
                EmailAddress = volunteer.Email,
                TypeOfCall = (BO.CallType)volunteerCall.Type,
                EntryTime = volunteerAssignment.TimeOfStarting,
                LastTimeForClosingTheCall = volunteerCall.DeadLine,
                Status = CallManager.GetStatus(volunteerCall.Id),
                DistanceFromAssignedVolunteer = distance,
            };
        }

        //Create the BO Volunteer
        return VolunteerManager.ConvertDoVolunteerToBoVolunteer(volunteer, volunteerCallInProgress);
    }

    /// <summary>
    /// This method returns an enumarable of VolunteerInList entities.
    /// The enumerable can be filtered and unfiltered, sorted and unsorted depending on the past parameters
    /// </summary>
    /// <param name="filterByActiveStatus">If null the returned enumarable would be full and not filtered, otherwise the return enumerable will be filtered by the given boolean value</param>
    /// <param name="sortByField">If null the returned enumerable would be sorted by the Id, otherwise the enumerable will be sorted by the given value</param>
    /// <returns></returns>
    public IEnumerable<BO.VolunteerInList> GetVolunteers(bool? filterByActiveStatus, BO.VolunteerInListField? sortByField)
    {

        //Get filtered or unfiltered enumerable of Volunteers
        IEnumerable<DO.Volunteer> volunteers = (filterByActiveStatus == null)
            ? s_dal.Volunteer.ReadAll()
            : s_dal.Volunteer.ReadAll((DO.Volunteer volunteer) => volunteer.IsActive == (bool)filterByActiveStatus);

        //Convert to VolunteerInList entities
        IEnumerable<BO.VolunteerInList> volunteerInLists = from volunteer in volunteers
                                                           select new BO.VolunteerInList
                                                           {
                                                               Id = volunteer.Id,
                                                               FullName = volunteer.FullName,
                                                               IsActive = volunteer.IsActive,
                                                               CallId = s_dal.Assignment
                                                                    .Read((DO.Assignment assignemnt) => assignemnt.VolunteerId == volunteer.Id)
                                                                    ?.CallId

                                                           };
        //Sort the given above enumerable
        if (sortByField is null)
            return from volunteer in volunteerInLists
                   orderby volunteer.Id
                   select volunteer;
        //Issue #16: Refactor
        switch (sortByField)
        {
            case BO.VolunteerInListField.Id:
                {
                    volunteers = from volunteer in volunteers
                                    orderby volunteer.Id
                                    select volunteer;
                    break;
                }
            case BO.VolunteerInListField.FullName:
                {
                    volunteers = from volunteer in volunteers
                                    orderby volunteer.FullName
                                    select volunteer;
                    break;
                }
            case BO.VolunteerInListField.IsActive:
                    volunteerInLists = from volunteer in volunteerInLists
                                    orderby volunteer.IsActive
                                    select volunteer;
                    break;
            case VolunteerInListField.TotalCallsDoneByVolunteer:
                volunteerInLists = from volunteer in volunteerInLists
                                    orderby volunteer.TotalCallsDoneByVolunteer
                                    select volunteer;
                break;
            case VolunteerInListField.TotalCallsCancelByVolunteer:
                volunteerInLists = from volunteer in volunteerInLists
                                    orderby volunteer.TotalCallsCancelByVolunteer
                                    select volunteer;
                break;
            case VolunteerInListField.TotalCallsExpiredByVolunteer:
                volunteerInLists = from volunteer in volunteerInLists
                                    orderby volunteer.TotalCallsExpiredByVolunteer
                                    select volunteer;
                break;
            case VolunteerInListField.CallId:
                volunteerInLists = from volunteer in volunteerInLists
                                    orderby volunteer.CallId
                                    select volunteer;
                break;
            case VolunteerInListField.TypeOfCall:
                volunteerInLists = from volunteer in volunteerInLists
                                    orderby volunteer.TypeOfCall
                                    select volunteer;
                break;
            case null:
                throw new BO.BlInvalidOperationException($"BL: Aren't able to order by the field null value");
            default:
                    throw new BO.BlInvalidOperationException($"BL: Aren't able to order by the field {sortByField}");
        }

        return volunteerInLists;
    }

    /// <summary>
    /// Logs into an account using the past username (Email address) and password
    /// If such a user doesn't exists or the credentials aren't correct, then method will throw an exception
    /// </summary>
    /// <param name="username">User's username value</param>
    /// <param name="password">User's password value</param>
    /// <returns>The type of user</returns>
    public string Login(string username, string? password)
    {
        string? hashedPassword = password is null
            ? null
            : VolunteerManager.GetSHA256HashedPassword(password);
        
        DO.Volunteer volunteer = s_dal.Volunteer
            .Read((DO.Volunteer volunteer) => volunteer.Email == username && volunteer.Password == hashedPassword)
            ?? throw new BO.BlDoesNotExistException($"BL: Volunteer with email address: {username} and password: {password} doesn't exsits");
        
        return volunteer.Role.ToString();
    }

    /// <summary>
    /// This method updates the corisponding Volunteer entity from DO 
    /// It checks if the given id field is assosiated with a manager or with the updated volunteer person
    /// It checks if the new fields (id, address) are valid
    /// It requests the volunteer from the DO and compares what field been modified and check if the fields are modifiable by the user which makes the action (Role is modifable by the manager only)
    /// It would create a DO.Volunteer entity out of the given volunteer variable and will request an Update action from the DAL layer
    /// If such user doesn't exist the exception would be handled in the logical layer and it will throw an exception to the upper layer
    /// </summary>
    /// <param name="id">The user id which wants to make the update action</param>
    /// <param name="volunteer">The volunteer entity which is need an updated</param>
    /// <param name="hasOldPassword">[Optional] an indicator whether the user has modified his password or not</param>
    public void UpdateVolunteerDetails(int id, BO.Volunteer volunteer, bool isPasswordBeenModified = true)
    {
        //Check if allowed to modify
        if (id != volunteer.Id && s_dal.Volunteer.Read((DO.Volunteer volunteer) => volunteer.Id == id && volunteer.Role == DO.UserRole.Admin) == null)
            throw new BO.BlForbidenSystemActionExeption($"BL: Un granted access volunteer (Id:{id}) tries to modify the volunteer Id: {volunteer.Id} values");
        
        //Check if logics are correct
        if (!VolunteerManager.IsVolunteerValid(volunteer, !isPasswordBeenModified))
            throw new BO.BlInvalidEntityDetails($"BL: volunteer's fields (Id: {volunteer.Id}) are invalid");

        //Get original Volunteer for comparing
        DO.Volunteer currentVolunteer = s_dal.Volunteer.Read((DO.Volunteer oldVolunteer) => oldVolunteer.Id == volunteer.Id)
            ?? throw new BO.BlDoesNotExistException($"BL: Volunteer with Id {volunteer.Id} doesn't exsits");

        //Checks what fields are requested to be modified - The role is modifable by only the manager
        if (volunteer.Role != (BO.UserRole) currentVolunteer.Role && s_dal.Volunteer.Read((DO.Volunteer volunteer) => volunteer.Id == id && volunteer.Role == DO.UserRole.Admin) == null)
            throw new BO.BlForbidenSystemActionExeption($"BL: Non-admin volunteer (Id: {id}) attemts to modify volunteer's Role (Id: {volunteer.Id})");

        //Update the cordinates
        if(volunteer.FullCurrentAddress != null)
        {
            (volunteer.Latitude, volunteer.Longitude) = VolunteerManager.GetGeoCordinates(volunteer.FullCurrentAddress);
        }
        else
        {
            (volunteer.Latitude, volunteer.Longitude) = (null, null);
        }
        
        //Create new instance of DO.Volunteer
        DO.Volunteer newVolunteer = VolunteerManager.ConvertBoVolunteerToDoVolunteer(volunteer);

        //Update the entity in Dal
        try
        {
            s_dal.Volunteer.Update(newVolunteer);
        }
        catch(DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Bl: The volunteer Id: {volunteer.Id} doesn't exists", ex);
        }
    }
}

