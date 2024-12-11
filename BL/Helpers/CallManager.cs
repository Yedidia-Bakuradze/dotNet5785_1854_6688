using DalApi;
using DO;
using System.Security.Cryptography;
namespace Helpers;

internal static class CallManager
{
    private static IDal ss_dal = Factory.Get; //stage 4
    public static void UpdateAllOpenAndExpierdCalls()
    {
        List<DO.Call> listOfCalls = ss_dal.Call.ReadAll((DO.Call call) => call.DeadLine < ClockManager.Now).ToList();
        List<DO.Assignment> listOfAssignments = ss_dal.Assignment.ReadAll().ToList();

        listOfCalls
            .Where(call => !listOfAssignments.Any(assignment => assignment.CallId == call.Id))
            .Select(call => call)
            .ToList()
            .ForEach(call => ss_dal.Assignment.Create( new DO.Assignment
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
            .ForEach((assignment) => ss_dal.Assignment.Update(assignment with
        {
            TimeOfEnding = ClockManager.Now,
            TypeOfEnding = DO.TypeOfEnding.CancellationExpired
        }));


    }
}
