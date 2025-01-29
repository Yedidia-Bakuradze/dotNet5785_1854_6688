namespace BlImplementation;

using BO;
using BlApi;
using Helpers;
using System;
using DO;

internal class VolunteerImplementation : IVolunteer
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
        AdminManager.ThrowOnSimulatorIsRunning();
        
        //Check logics and formmating
        if (!VolunteerManager.IsVolunteerValid(volunteer))
            throw new BO.BlInvalidEntityDetails($"BL Error: volunteer {volunteer.Id} fields are invalid");


        try
        {
            //Add the new entity to the database
            lock (AdminManager.BlMutex)
                s_dal.Volunteer.Create(new DO.Volunteer
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
                    Latitude = null,
                    Longitude = null
                });

            
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Bl Says: Such object already exists in the database", ex);
        }

        //Updates the cordinates of the volunteer async
        _ = VolunteerManager.UpdateVolunteerCordinates(volunteer.Id, volunteer.FullCurrentAddress, true);
        
        //Notifies all observers that a volunteer has been add
        VolunteerManager.Observers.NotifyListUpdated();
    }

    /// <summary>
    /// This method accespts an id value, requests from the DAL layer such a volunteer with that id, to check if it is allowed to be deleted
    /// a volunteer is allowed to be deleted if it never and would ever hadle a call
    /// If the action is now allowed it would throw an exception to the upper layer
    /// If the volunteer hasn't been found it would handler the thrown exception by throwing a new one to the upper layer
    /// </summary>
    /// <param name="volunteerId">The requested volunteer's id value</param>
    public void DeleteVolunteer(int volunteerId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();

        //Throws an exception if volunteer cant be deleted
        VolunteerManager.VertifyVolunteerDeletionAttempt(volunteerId);

        try
        {
            //Tries to remove if the volunteer exists
            lock (AdminManager.BlMutex)
            {
                s_dal.Volunteer.Delete(volunteerId);
            }

        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"BL-Dal: Volunteer {volunteerId} doesn't exists", ex);
        }

        //Notifies all observers that a volunteer has been add
        VolunteerManager.Observers.NotifyListUpdated();

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
        DO.Volunteer volunteer;
        DO.Assignment? volunteerAssignment;
        DO.Call volunteerCall;
        BO.CallInProgress? volunteerCallInProgress = null;
        lock (AdminManager.BlMutex)
        {
            volunteer = s_dal.Volunteer.Read(id)
            ?? throw new BO.BlDoesNotExistException($"BL: Volunteer with id of {id} doesn't exists");
            volunteerAssignment = s_dal.Assignment
                .ReadAll(assignment => assignment.VolunteerId == volunteer.Id && assignment.TypeOfEnding == null)
                .LastOrDefault();
        }


        //If there is an assingment - there is a call to create
        if (volunteerAssignment != null)
        {
            //Get the Dal call
            lock (AdminManager.BlMutex)
            { 
                volunteerCall = s_dal.Call.Read(call => call.Id == volunteerAssignment.CallId)
                    ?? throw new BO.BlDoesNotExistException($"BL: UNWANTED: There is not call with id of {volunteerAssignment.CallId}");
            }

            //Calculate the distance
            double distance = -1.0;
            if (volunteer.FullCurrentAddress != null)
                distance = VolunteerManager.CalculateDistanceFromVolunteerToCall((volunteer.Latitude, volunteer.Longitude), (volunteerCall.Latitude, volunteerCall.Longitude),volunteer.RangeType);

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
        IEnumerable<DO.Volunteer> volunteers;
        IEnumerable<BO.VolunteerInList> volunteerInLists;
        lock (AdminManager.BlMutex)
        {
            // Get filtered or unfiltered enumerable of Volunteers
            volunteers = (filterByActiveStatus == null)
                ? s_dal.Volunteer.ReadAll()
                : s_dal.Volunteer.ReadAll(volunteer => volunteer.IsActive == filterByActiveStatus);

            // Convert to VolunteerInList entities
            volunteerInLists = from volunteer in volunteers
                                   let currentAssignment = s_dal.Assignment
                                       .ReadAll(assignemnt => assignemnt.VolunteerId == volunteer.Id && assignemnt.TimeOfEnding == null)
                                       .OrderBy(ass => ass.Id)
                                       .LastOrDefault()
                                   let currentCall = currentAssignment == null ? null : s_dal.Call.Read(call => call.Id == currentAssignment.CallId)
                                   select new BO.VolunteerInList
                                   {
                                       Id = volunteer.Id,
                                       FullName = volunteer.FullName,
                                       IsActive = volunteer.IsActive,
                                       CallId = currentAssignment?.CallId,
                                       TypeOfCall = currentCall != null ? (BO.CallType)currentCall.Type : BO.CallType.Undefined
                                   };

        }
        // Sort the enumerable based on the specified field
        if (sortByField != null)
        {
            volunteerInLists = sortByField switch
            {
                BO.VolunteerInListField.Id => volunteerInLists.OrderBy(volunteer => volunteer.Id),
                BO.VolunteerInListField.FullName => volunteerInLists.OrderBy(volunteer => volunteer.FullName),
                BO.VolunteerInListField.IsActive => volunteerInLists.OrderBy(volunteer => volunteer.IsActive),
                BO.VolunteerInListField.TotalCallsDoneByVolunteer => volunteerInLists.OrderBy(volunteer => volunteer.TotalCallsDoneByVolunteer),
                BO.VolunteerInListField.TotalCallsCancelByVolunteer => volunteerInLists.OrderBy(volunteer => volunteer.TotalCallsCancelByVolunteer),
                BO.VolunteerInListField.TotalCallsExpiredByVolunteer => volunteerInLists.OrderBy(volunteer => volunteer.TotalCallsExpiredByVolunteer),
                BO.VolunteerInListField.CallId => volunteerInLists.OrderBy(volunteer => volunteer.CallId),
                BO.VolunteerInListField.TypeOfCall => volunteerInLists.OrderBy(volunteer => volunteer.TypeOfCall),
                _ => throw new BO.BlInvalidOperationException($"BL: Aren't able to order by the field {sortByField}")
            };
        }
        else
        {
            volunteerInLists = volunteerInLists.OrderBy(volunteer => volunteer.Id);
        }

        return volunteerInLists;
    }

    /// <summary>
    /// This method returns filtered and sorted list of VolunteerInList given the parameters which the user insereted
    /// It has been requiered from us to do so in order to provide the PL layer a filter list of values
    /// </summary>
    /// <param name="filterField">The requested field to filter it by the second value parameter</param>
    /// <param name="filterValue">The value which we want the filter field to have</param>
    /// <param name="sortByField">Optional: Field which it would sort by</param>
    /// <returns>Filtered and optionlly sorted VolunteerInLists</returns>
    public IEnumerable<BO.VolunteerInList> GetFilteredVolunteers(BO.VolunteerInListField? filterField, object? filterValue, BO.VolunteerInListField? sortByField)
    {
        var volunteers = GetVolunteers(null, null);

        if (filterField != null && filterValue != null)
        {
            switch (filterField)
            {
                case VolunteerInListField.Id:
                    if (int.TryParse(filterValue.ToString(), out int volunteerId))
                    {
                        volunteers = volunteers.Where(vol => vol.Id == volunteerId);
                    }
                    else
                    {
                        throw new BlInputValueUnConvertableException($"Bl: The value {filterValue} is not a valid number for filtering by Id.");
                    }
                    break;

                case VolunteerInListField.FullName:
                    volunteers = volunteers.Where(vol =>
                        !string.IsNullOrEmpty(vol.FullName) &&
                        vol.FullName.Equals(filterValue.ToString(), StringComparison.OrdinalIgnoreCase));
                    break;

                case VolunteerInListField.IsActive:
                    if (bool.TryParse(filterValue.ToString(), out bool isActive))
                    {
                        volunteers = volunteers.Where(vol => vol.IsActive == isActive);
                    }
                    else
                    {
                        throw new BlInputValueUnConvertableException($"Bl: The value {filterValue} is not a valid boolean for filtering by IsActive.");
                    }
                    break;

                case VolunteerInListField.TotalCallsDoneByVolunteer:
                    if (int.TryParse(filterValue.ToString(), out int totalCallsDone))
                    {
                        volunteers = volunteers.Where(vol => vol.TotalCallsDoneByVolunteer == totalCallsDone);
                    }
                    else
                    {
                        throw new BlInputValueUnConvertableException($"Bl: The value {filterValue} is not a valid number for filtering by TotalCallsDoneByVolunteer.");
                    }
                    break;

                case VolunteerInListField.TotalCallsCancelByVolunteer:
                    if (int.TryParse(filterValue.ToString(), out int totalCallsCanceled))
                    {
                        volunteers = volunteers.Where(vol => vol.TotalCallsCancelByVolunteer == totalCallsCanceled);
                    }
                    else
                    {
                        throw new BlInputValueUnConvertableException($"Bl: The value {filterValue} is not a valid number for filtering by TotalCallsCancelByVolunteer.");
                    }
                    break;

                case VolunteerInListField.TotalCallsExpiredByVolunteer:
                    if (int.TryParse(filterValue.ToString(), out int totalCallsExpired))
                    {
                        volunteers = volunteers.Where(vol => vol.TotalCallsExpiredByVolunteer == totalCallsExpired);
                    }
                    else
                    {
                        throw new BlInputValueUnConvertableException($"Bl: The value {filterValue} is not a valid number for filtering by TotalCallsExpiredByVolunteer.");
                    }
                    break;
                case VolunteerInListField.CallId:
                    if (int.TryParse(filterValue.ToString(), out int callId))
                    {
                        volunteers = volunteers.Where(vol => vol.CallId == callId);
                    }
                    else
                    {
                        throw new BlInputValueUnConvertableException($"Bl: The value {filterValue} is not a valid number for filtering by TotalCallsExpiredByVolunteer.");
                    }
                    break;
                case VolunteerInListField.TypeOfCall:
                    if (Enum.TryParse(filterValue.ToString(), out BO.CallType callType))
                    {
                        volunteers = volunteers.Where(vol => vol.TypeOfCall == callType);
                    }
                    else
                    {
                        throw new BlInputValueUnConvertableException($"Bl: The value {filterValue} is not a valid number for filtering by TotalCallsExpiredByVolunteer.");
                    }
                    break;
                default:
                    throw new BlInvalidOperationException($"Bl: The filter field {filterField} is not supported.");
            }
        }

        if (sortByField != null)
        {
            switch (sortByField)
            {
                case VolunteerInListField.Id:
                    volunteers = volunteers.OrderBy(vol => vol.Id);
                    break;

                case VolunteerInListField.FullName:
                    volunteers = volunteers.OrderBy(vol => vol.FullName);
                    break;

                case VolunteerInListField.IsActive:
                    volunteers = volunteers.OrderBy(vol => vol.IsActive);
                    break;

                case VolunteerInListField.TotalCallsDoneByVolunteer:
                    volunteers = volunteers.OrderBy(vol => vol.TotalCallsDoneByVolunteer);
                    break;

                case VolunteerInListField.TotalCallsCancelByVolunteer:
                    volunteers = volunteers.OrderBy(vol => vol.TotalCallsCancelByVolunteer);
                    break;

                case VolunteerInListField.TotalCallsExpiredByVolunteer:
                    volunteers = volunteers.OrderBy(vol => vol.TotalCallsExpiredByVolunteer);
                    break;

                default:
                    throw new BlInvalidOperationException($"Bl: The sort field {sortByField} is not supported.");
            }
        }

        return volunteers;
    }

    /// <summary>
    /// Logs into an account using the past id (Email address) and password
    /// If such a user doesn't exists or the credentials aren't correct, then method will throw an exception
    /// </summary>
    /// <param name="id">User's id value</param>
    /// <param name="password">User's password value</param>
    /// <returns>The type of user</returns>
    public string Login(string id, string? password)
    {
        DO.Volunteer volunteer;
        if (!int.TryParse(id, out int validId))
            throw new BO.BlInvalidEntityDetails($"BL Error: Invalid id value");

        string? hashedPassword = password is null
            ? null
            : VolunteerManager.GetSHA256HashedPassword(password);

        lock (AdminManager.BlMutex)
        {
            volunteer = s_dal.Volunteer
                .Read((DO.Volunteer volunteer) => volunteer.Id == validId && volunteer.Password == hashedPassword)
                ?? throw new BO.BlDoesNotExistException($"BL: Volunteer with id: {id} doesn't exsits");
        }

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
    public async Task UpdateVolunteerDetails(int id, BO.Volunteer volunteer, bool isPasswordBeenModified = true)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        DO.Volunteer currentVolunteer;

        //Check if allowed to modify
        lock (AdminManager.BlMutex)
        {
            if (id != volunteer.Id && s_dal.Volunteer.Read((DO.Volunteer volunteer) => volunteer.Id == id && volunteer.Role == DO.UserRole.Admin) == null)
                throw new BO.BlForbidenSystemActionExeption($"BL: Un granted access volunteer (Id:{id}) tries to modify the volunteer Id: {volunteer.Id} values");
        }
        
        //Check if logics are correct
        if (! await VolunteerManager.IsVolunteerValid(volunteer, !isPasswordBeenModified))
            throw new BO.BlInvalidEntityDetails($"BL: volunteer's fields (Id: {volunteer.Id}) are invalid");

        lock (AdminManager.BlMutex)
        {
            //Get original Volunteer for comparing
            currentVolunteer = s_dal.Volunteer.Read((DO.Volunteer oldVolunteer) => oldVolunteer.Id == volunteer.Id)
                ?? throw new BO.BlDoesNotExistException($"BL: Volunteer with Id {volunteer.Id} doesn't exsits");
        }

        lock (AdminManager.BlMutex)
        {
            //Checks what fields are requested to be modified - The role is modifable by only the manager
            if (volunteer.Role != (BO.UserRole) currentVolunteer.Role && s_dal.Volunteer.Read((DO.Volunteer volunteer) => volunteer.Id == id && volunteer.Role == DO.UserRole.Admin) == null)
                throw new BO.BlForbidenSystemActionExeption($"BL: Non-admin volunteer (Id: {id}) attemts to modify volunteer's Role (Id: {volunteer.Id})");
        }

        //Checks if the user tries to be inactive while running a call
        if(volunteer.IsActive == false && volunteer.CurrentCall is not null)
            throw new BO.BlForbidenSystemActionExeption($"BL: Volunteer cannot deactivate while having an active call, please close the current call and try again");

        //Update the cordinates
        if(volunteer.FullCurrentAddress != null)
            (volunteer.Latitude, volunteer.Longitude) =await VolunteerManager.GetGeoCordinates(volunteer.FullCurrentAddress);
        else
            (volunteer.Latitude, volunteer.Longitude) = (null, null);
        
        //Create new instance of DO.Volunteer
        DO.Volunteer newVolunteer = VolunteerManager.ConvertBoVolunteerToDoVolunteer(volunteer);

        //Update the entity in Dal
        try
        {
            lock (AdminManager.BlMutex)
            {
                s_dal.Volunteer.Update(newVolunteer);
            }

            //Notifies all observers that a volunteer has been changed
            VolunteerManager.Observers.NotifyItemUpdated(newVolunteer.Id);
            VolunteerManager.Observers.NotifyListUpdated();

        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Bl: The volunteer Id: {volunteer.Id} doesn't exists", ex);
        }
    }
}

