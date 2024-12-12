namespace BlApi;
public interface ICall
{
    /// <summary>
    /// Get the total number of calls by their status
    /// </summary>
    public int[] GetTotalCallsByStatus();

    /// <summary>
    /// Get a list of calls based on specified parameters
    /// </summary>
    /// <param name="parameter">The field to filter the calls by</param>
    /// <param name="value">The value to filter the calls by</param>
    /// <param name="parameter1">The second field to filter the calls by</param>
    public IEnumerable<BO.CallInList> GetListOfCalls(BO.CallInListFields? parameter = null, object? value = null, BO.CallInListFields? parameter1 = null);

    /// <summary>
    /// Get the details of a specific call
    /// </summary>
    /// <param name="callId">The ID of the call</param>
    public BO.Call GetDetielsOfCall(int callId);

    /// <summary>
    /// Update the details of a call
    /// </summary>
    /// <param name="call">The updated call object</param>
    public void UpdateCall(BO.Call call);

    /// <summary>
    /// Delete a call request
    /// </summary>
    /// <param name="VolunteerId">The ID of the volunteer</param>
    public void DeleteCallRequest(int VolunteerId);

    /// <summary>
    /// Add a new call
    /// </summary>
    /// <param name="call">The call object to add</param>
    public void AddCall(BO.Call call);

    /// <summary>
    /// Get a list of closed calls for a specific volunteer
    /// </summary>
    /// <param name="id">The ID of the volunteer</param>
    /// <param name="callType">The type of call to filter by</param>
    /// <param name="parameter">The field to filter the closed calls by</param>
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
    public void UpdateCallEnd(int VolunteerId, int callId);

    /// <summary>
    /// Update the status of the end of a call
    /// </summary>
    /// <param name="VolunteerId">The ID of the volunteer</param>
    /// <param name="callId">The ID of the call</param>
    public void EndOfCallStatusUpdate(int VolunteerId, int callId);

    /// <summary>
    /// Select a call to do by a volunteer
    /// </summary>
    /// <param name="VolunteerId">The ID of the volunteer</param>
    /// <param name="callId">The ID of the call</param>
    public void SelectCallToDo(int VolunteerId, int callId);

    /// <summary>
    /// Read call from database
    /// </summary>
    /// <param name="callId">The ID of the call</param>
    public void Read(int callId);

}
