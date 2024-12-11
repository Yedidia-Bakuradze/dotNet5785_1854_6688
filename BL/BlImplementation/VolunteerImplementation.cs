namespace BlImplementation;
using BlApi;
using BO;
using System;

internal class VolunteerImplementation : IVolunteer
{
    //The contract which we will make all the action with when using the Dal layer
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;



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
                throw new BoInvalidEntityDetails($"BL-DAL Error: When craeting new Volunteer entity: For id: {volunteer.Id}");
            }

            //Create Dal Volunteer entity
            DO.Volunteer newVolunteer = new DO.Volunteer
            {
                Id = volunteer.Id,
                Role = (DO.UserRole)Enum.Parse(typeof(DO.UserRole) ,volunteer.Role.ToString()) ,
                FullName = volunteer.FullName,
                PhoneNumber = volunteer.PhoneNumber,
                Email = volunteer.Email,
                MaxDistanceToCall = volunteer.MaxDistanceToCall,
                RangeType = (DO.TypeOfRange)Enum.Parse(typeof(DO.TypeOfRange),volunteer.RangeType.ToString()),
                IsActive = volunteer.IsActive,
                Password = volunteer.Password,
                FullCurrentAddress = volunteer.FullCurrentAddress,
                Latitude = volunteer.Latitude,
                Longitude = volunteer.Longitude
            };

            //Add the new entity to the database
            _dal.Volunteer.Create(newVolunteer);
        }
        catch(DO.DalAlreadyExistsException ex)
        {
            //Throw new BL excpetion to the upper layers
            throw new BoAlreadyExistsException($"BL: Such object already exists in the database",ex);
        }
    }

    public void DeleteVolunteer(int id)
    {
        throw new NotImplementedException();
    }

    public BO.Volunteer GetVolunteerDetails(int id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<BO.VolunteerInList> GetVolunteers(bool? filterByStatus, BO.VolunteerInListField sortByField)
    {
        throw new NotImplementedException();
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
