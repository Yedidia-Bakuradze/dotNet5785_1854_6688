
namespace BlApi;

public interface ICall
{
    int GetTotalCallsByStatus();
    IEnumerable<BO.CallInList> GetListOfCalls(BO.CallInList? parameter = null,object? value = null, BO.CallInList? parameter1 = null);
    IEnumerable<BO.Call> GetDetialsOfCall(int callId);
    void Update(BO.Call call);
    void Delete(int id);

}
