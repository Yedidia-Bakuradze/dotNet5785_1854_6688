namespace BlImplementation;
using BlApi;
using BO;
using DO;
using Helpers;
using System;

internal class VolunteerImplementation : IVolunteer
{
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
    /// <exception cref="BoAlreadyExistsException">A</exception>
    public void AddVolunteer(BO.Volunteer volunteer)
    {
        try
        {
            //Check logics and formmating
            if (!Helpers.VolunteerManager.IsVolunteerValid(volunteer))
            {
                throw new BO.BoInvalidEntityDetails($"BL Error: volunteer {volunteer.Id} fields are invalid");
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
                Password = volunteer.Password,
                FullCurrentAddress = volunteer.FullCurrentAddress,
                Latitude = volunteer.Latitude,
                Longitude = volunteer.Longitude
            };

            //Add the new entity to the database
            s_dal.Volunteer.Create(newVolunteer);

        }
        catch (DO.DalAlreadyExistsException ex)
        {
            //Throw new BL excpetion to the upper layers
            throw new BO.BoAlreadyExistsException($"BL-DAL: Such object already exists in the database", ex);
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
            ?? throw new BO.BoDoesNotExistException($"BL: Error while tyring to remove the volunteer {id}");

        //Checks if the volunteer is in any records of assignments
        if (s_dal.Assignment.Read((DO.Assignment assignment) => assignment.VolunteerId == id) != null)
        {
            throw new BO.BoEntityRecordIsNotEmpty($"BL: Unable to remove the volunteer {id} due to that it has references in other assignment records");
        }

        //Tries to remove if the volunteer exists
        try
        {
            s_dal.Volunteer.Delete(id);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BoDoesNotExistException($"BL-Dal: Volunteer {id} doesn't exists", ex);
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
            ?? throw new BO.BoDoesNotExistException($"BL: Volunteer with id of {id} doesn't exists");

        DO.Assignment? volunteerAssignment = s_dal.Assignment.Read(assignment => assignment.VolunteerId == volunteer.Id);
        BO.CallInProgress? volunteerCallInProgress = null;

        //If there is an assingment - there is a call to create
        if (volunteerAssignment != null)
        {
            //Get the Dal call
            DO.Call volunteerCall = s_dal.Call.Read(call => call.Id == volunteerAssignment.CallId)
                ?? throw new BO.BoDoesNotExistException($"BL: UNWANTED: There is not call with id of {volunteerAssignment.CallId}");

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

        //Sort the given above enumerable
        if (sortByField is null)
        {
            volunteers = from volunteer in volunteers
                         orderby volunteer.Id
                         select volunteer;
        }
        else
        {
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
                    {
                        volunteers = from volunteer in volunteers
                                        orderby volunteer.IsActive
                                        select volunteer;
                        break;
                    }
                    //Issue #15: Find why theses fields aren't orderby-able
                    { 
                        //case BO.VolunteerInListField.TotalCallsDoneByVolunteer:
                        //case BO.VolunteerInListField.TotalCallsCancelByVolunteer:
                        //case BO.VolunteerInListField.TotalCallsExpiredByVolunteer:
                        //case BO.VolunteerInListField.CallId:
                        //case BO.VolunteerInListField.TypeOfCall:
                    }
                default:
                    {
                        throw new BO.BoForbidenActionExeption($"BL: Aren't able to order by the field {sortByField}");
                    }
            }
        }

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
        return volunteerInLists;
    }

    public string Login(string username, string password)
    {
        throw new NotImplementedException();
    }

    public void UpdateVolunteerDetails(int id, BO.Volunteer volunteer)
    {
        throw new NotImplementedException();
    }
}

