using DalApi;
using System.Net.Http.Headers;
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

    /// <summary>
    /// Accepts a Call Id and returns it status based on the CallStatus enum values
    /// </summary>
    /// <param name="callID">The Call Id value</param>
    /// <returns>The status of the call</returns>
    /// <exception cref="DO.DalDoesNotExistException">Throws an exception if such call doesn't exists</exception>
    public static BO.CallStatus GetStatus(int callID)
    {
        DO.Call res = s_dal.Call.Read(call => call.Id == callID)
            ?? throw new DO.DalDoesNotExistException("Call Does not exist");

        DO.Assignment? resAssignments = s_dal.Assignment.Read(ass => ass.CallId == callID);

        //If there is not assingment - no one took the call therefor the call is open
        if(resAssignments == null )
        {
            return ((res.DeadLine - ClockManager.Now) <= s_dal.Config.RiskRange)
                    ? BO.CallStatus.OpenAndRisky
                    : BO.CallStatus.Open;
        }

        //If there is such assignment containing the CallId - Check if expiered or in progress
        if (resAssignments.TypeOfEnding == null)
        {
            if (res.DeadLine <= ClockManager.Now) return BO.CallStatus.Expiered;
            if (res.DeadLine > ClockManager.Now) return BO.CallStatus.InProgress;
            if ((res.DeadLine - ClockManager.Now) <= s_dal.Config.RiskRange) return BO.CallStatus.InProgressAndRisky;
        }
        
        return BO.CallStatus.Closed;
    }


    internal static bool IsCallValid(BO.Call call)
    {
        //Check if the times are valid
        if (call.CallStartTime > call.CallDeadLine || call.CallDeadLine < ClockManager.Now)
        {
            throw new BO.BlInvalidEntityDetails("The deadline of the call cannot be before the start time of the call");
        }

        //Checks if the address is valid (if cordinates exist)
        (double? lat, double? lng) = VolunteerManager.GetGeoCordinates(call.CallAddress);
        if (lat == null || lng == null)
            throw new BO.BlInvalidEntityDetails($"BL: The given call address ({call.CallAddress}) is not a real address");
        return true;
    }
}
