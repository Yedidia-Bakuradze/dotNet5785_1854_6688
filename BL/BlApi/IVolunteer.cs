namespace BlApi;

/// <summary>
/// This interface exposese all the fucntionally which are used on Volunteer entities
/// </summary>
public interface IVolunteer :IObservable //Stage 5
{
    /// <summary>
    /// Logs into an account using the past username (Emaill address) and password
    /// If such a user doesn't exists or the credentials aren't correct, the method shall thrown an exception
    /// </summary>
    /// <param name="username">User's username value</param>
    /// <param name="password">[Optional]: User's password value</param>
    /// <returns>The type of user</returns>
    string Login(string username, string? password);

    /// <summary>
    /// This method returns an enumarable of VolunteerInList entities.
    /// The enumerable can be filtered and unfiltered, sorted and unsorted depending on the past parameters
    /// </summary>
    /// <param name="filterByStatus">If null the returned enumarable would be full and not filtered, otherwise the return enumerable will be filtered by the given boolean value</param>
    /// <param name="sortByField">If null the returned enumerable would be sorted by the Id, otherwise the enumerable will be sorted by the given value</param>
    /// <returns></returns>
    IEnumerable<BO.VolunteerInList> GetVolunteers(bool? filterByStatus, BO.VolunteerInListField? sortByField);

    /// <summary>
    /// This method returns filtered and sorted list of VolunteerInList given the parameters which the user insereted
    /// It has been requiered from us to do so in order to provide the PL layer a filter list of values
    /// </summary>
    /// <param name="filterField">The requested field to filter it by the second value parameter</param>
    /// <param name="filterValue">The value which we want the filter field to have</param>
    /// <param name="sortByField">Optional: Field which it would sort by</param>
    /// <returns>Filtered and optionlly sorted VolunteerInLists</returns>
    IEnumerable<BO.VolunteerInList> GetFilteredVolunteers(BO.VolunteerInListField filterField, object filterValue, BO.VolunteerInListField? sortByField);

    /// <summary>
    /// This method accepts an id value, calls the Read method from DAL, using the returned value it creates a new BO.Volunteer entity,
    /// also, the method adds a related call to the volunteer in the BO.CallInProgress field, adds to the created BO.Volunteer entity and retuns it.
    /// If such volunteer hasn't been found, the system will throw an exception which would be taken care of in the BL layer and which then will throw an exception to the upper layer
    /// </summary>
    /// <param name="id">The requersted volunteer's id</param>
    /// <returns>An BO.Volunteer entity which contains the requested entity's values and the current in progress call which is taken care of by the volunteer</returns>
    BO.Volunteer GetVolunteerDetails(int id);

    /// <summary>
    /// This method shall update the corisponding Volunteer entity from DO 
    /// It checks if the given id field is assosiated with a manager or with the updated volunteer person
    /// It checks if the new fields (id, address) are valid
    /// It requests the volunteer from the DO and compares what field been modified and check if the fields are modifiable by the user which makes the action (Role is modifable by the manager only)
    /// It would create a DO.Volunteer entity out of the given volunteer variable and will request an Update action from the DAL layer
    /// If such user doesn't exist the exception would be handled in the logical layer and it will throw an exception to the upper layer
    /// </summary>
    /// <param name="id">The user id which wants to make the update action</param>
    /// <param name="volunteer">The volunteer entity which is need an updated</param>
    void UpdateVolunteerDetails(int id, BO.Volunteer volunteer, bool isPasswordBeenModified = true);

    /// <summary>
    /// This method accespts an id value, requests from the DAL layer to check if it is allowed to delete the volunteer with the given id value
    /// a volunteer is allowed to be deleted if it never and would ever hadle a call
    /// If the action is now allowed it would throw an exception to the upper layer
    /// If the volunteer hasn't been found it would handler the thrown exception by throwing a new one to the upper layer
    /// </summary>
    /// <param name="id">The requested volunteer's id value</param>
    void DeleteVolunteer(int id);

    /// <summary>
    /// This method accepts a well defiend BO.Volunteer variabel which is needed to be added to the dataabse
    /// It would check its logics again (like in the update action)
    /// It would cehck its formmting again (like in the update action)
    /// It would create a new DO.Volunteer entity using the volunteer's values, the it would call the Create action from the DAL layer
    /// If such a volunteer already exists it would handle the thrown excpetion by throwing a new exception to the upper layers
    /// </summary>
    /// <param name="volunteer"></param>
    void AddVolunteer(BO.Volunteer volunteer);
}
