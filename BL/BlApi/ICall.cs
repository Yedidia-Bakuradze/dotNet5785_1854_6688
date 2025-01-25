using BO;

namespace BlApi;
public interface ICall : IObservable //Stage 5
{
    /// <summary>
    /// Get the total number of calls by their status
    /// </summary>
    public int[] GetTotalCallsByStatus();

    /// <summary>
    /// Get a list of calls based on specified parameters
    /// </summary>
    /// <param name="filterField">The field to filter the calls by</param>
    /// <param name="filterValue">The value to filter the calls by</param>
    /// <param name="sortFiled">The sorting field to sort the calls by</param>
    public IEnumerable<BO.CallInList> GetListOfCalls(BO.CallInListFields? filterField = null, object? filterValue = null, BO.CallInListFields? sortFiled = null, IEnumerable<BO.CallInList>? source = null);

    /// <summary>
    /// Get the details of a specific call
    /// </summary>
    /// <param name="callId">The ID of the call</param>
    public BO.Call GetDetielsOfCall(int callId);

    /// <summary>
    /// Update the details of a call
    /// </summary>
    /// <param name="call">The updated call object</param>
    public Task UpdateCall(BO.Call call);

    /// <summary>
    /// Delete a call request
    /// </summary>
    /// <param name="VolunteerId">The ID of the volunteer</param>
    public void DeleteCallRequest(int VolunteerId);

    /// <summary>
    /// Add a new call
    /// </summary>
    /// <param name="call">The call object to add</param>
    public Task AddCall(BO.Call call);

    /// <summary>
    /// Get a list of closed calls for a specific volunteer
    /// </summary>
    /// <param name="id">The ID of the volunteer</param>
    /// <param name="callType">The type of call to filter by</param>
    /// <param name="parameter">Optional filterField for sorting the resulting list</param>
    public IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int id, BO.CallType? callType, BO.ClosedCallInListFields? parameter);

    /// <summary>
    /// Get a list of open calls for a specific volunteer
    /// </summary>
    /// <param name="id">The ID of the volunteer</param>
    /// <param name="callType">The type of call to filter by</param>
    /// <param name="parameter">The field to filter the open calls by</param>
    public IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteer(int id, BO.CallType? callType, BO.OpenCallFields? parameter);

    /// <summary>
    /// Update the end of a call
    /// </summary>
    /// <param name="VolunteerId">The ID of the volunteer</param>
    /// <param name="callId">The ID of the call</param>
    public void CancelAssignement(int VolunteerId, int callId);

    /// <summary>
    /// Update the status of the end of a call
    /// </summary>
    /// <param name="VolunteerId">The ID of the volunteer</param>
    /// <param name="callId">The ID of the call</param>
    public void FinishAssignement(int VolunteerId, int callId);

    /// <summary>
    /// Select a call to do by a volunteer
    /// </summary>
    /// <param name="VolunteerId">The ID of the volunteer</param>
    /// <param name="callId">The ID of the call</param>
    public void SelectCallToDo(int VolunteerId, int callId);


    public void CancleCallSendEmail(BO.CallInList c);
    public void AddCallSendEmail(BO.Call c);


    public IEnumerable<(double, double)> GetListOfOpenCallsForVolunteerCordinates(int volunteerId);
    public IEnumerable<(double, double)> ConvertOpenCallsToCordinates(IEnumerable<OpenCallInList> listOfCalls);
    public IEnumerable<(double, double)> ConvertClosedCallsIntoCordinates(IEnumerable<ClosedCallInList> listOfCalls);
}
