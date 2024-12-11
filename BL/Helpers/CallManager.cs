using DalApi;
namespace Helpers;

internal static class CallManager
{
    private static IDal s_dal = Factory.Get; //stage 4
    public static void UpdateAllOpenAndExpierdCalls()
    {
        List<DO.Call> listOfCalls = s_dal.Call.ReadAll((DO.Call call) => call.DeadLine < ClockManager.Now).ToList();
        List<DO.Assignment> listOfAssignments = s_dal.Assignment.ReadAll().ToList();

        listOfCalls
            .Where(call => !listOfAssignments.Any(assignment => assignment.CallId == call.Id))
            .Select(call => call)
            .ToList()
            .ForEach(call => s_dal.Assignment.Create( new DO.Assignment
        {
            CallId = call.Id,
            TimeOfStarting = call.OpeningTime,
            TimeOfEnding = ClockManager.Now,
            Id = -1,
            TypeOfEnding = DO.TypeOfEnding.CancellationExpired,
            VolunteerId = -1,//The instructions asked us to put NULL here, but the project instructions themselves state that it should not be NULL, so we made it -1 because it makes the most sense for us
        }
        ));

        listOfAssignments
            .Where(assignment => listOfCalls.Any(call => call.Id == assignment.CallId))
            .Select(assignment => assignment)
            .ToList()
            .ForEach((assignment) => s_dal.Assignment.Update(assignment with
        {
            TimeOfEnding = ClockManager.Now,
            TypeOfEnding = DO.TypeOfEnding.CancellationExpired
        }));


    }

    public static BO.CallStatus GetStatus(int callID)
    {
        DO.Call res = s_dal.Call.Read(call => call.Id == callID);
        DO.Assignment resAssignments = s_dal.Assignment.Read(ass => ass.CallId == callID);

        if( res == null) 
        {
            throw new DO.DalDoesNotExistException("Call Does not exist");
        }
        if(resAssignments == null )
        {
            return BO.CallStatus.Open;
        }
        if(res.DeadLine < ClockManager.Now && resAssignments.TypeOfEnding == null)
        {
            return BO.CallStatus.Expiered;
        }
        if(res.DeadLine > ClockManager.Now && resAssignments.TypeOfEnding == null)
        {
            return BO.CallStatus.InProgress;
        }
        if(res.DeadLine > ClockManager.Now && resAssignments.TypeOfEnding != null)
        {
            return BO.CallStatus.Closed;
        }
        if((res.DeadLine - ClockManager.Now) <= s_dal.Config.RiskRange && resAssignments == null)
        {
            return BO.CallStatus.OpenAndRisky;
        }
        if ((res.DeadLine - ClockManager.Now) <= s_dal.Config.RiskRange && resAssignments.TypeOfEnding == null)
        {
            return BO.CallStatus.InProgressAndRisky;
        }

        return BO.CallStatus.Undefined;

    }
}
