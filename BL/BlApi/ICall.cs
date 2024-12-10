namespace BlApi;
public interface ICall
{
    public int[] GetTotalCallsByStatus();
    public IEnumerable<BO.CallInList> GetListOfCalls(BO.CallInListFields? parameter = null,object? value = null, BO.CallInListFields? parameter1 = null);
    public BO.Call GetDetialsOfCall(int callId);
    public void UpdateCall(BO.Call call);
    public void DeleteCallRequest(int VolunteerId);
    public void AddCall(BO.Call call);
    public IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int id, BO.CallType? callType, BO.ClosedCallInListFields? parameter);
    public IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteer(int id, BO.CallType? callType, BO.OpenCallFields? parameter);
    public void UpdateCallEnd(int VolunteerId,int callId);
    public void EndOfCallStatusUpdate(int VolunteerId,int callId);
    public void SelectCallToDo(int VolunteerId,int callId);


}
