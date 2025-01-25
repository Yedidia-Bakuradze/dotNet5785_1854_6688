using DalApi;
using System.Net.Http.Headers;
namespace Helpers;

internal static class CallManager
{
    private static IDal s_dal = Factory.Get; //stage 4
    internal static ObserverManager Observers = new(); //stage 5 

    public static void UpdateAllOpenAndExpierdCalls()
    {
        lock (AdminManager.BlMutex)//stage 7
        {
            List<DO.Call> listOfCalls = s_dal.Call.ReadAll((DO.Call call) => call.DeadLine < AdminManager.Now).ToList();
            List<DO.Assignment> listOfAssignments = s_dal.Assignment.ReadAll().ToList();

            listOfCalls
                .Where(call => !listOfAssignments.Any(assignment => assignment.CallId == call.Id))
                .Select(call => call)
                .ToList()
                .ForEach(call => s_dal.Assignment.Create(new DO.Assignment
                {
                    CallId = call.Id,
                    TimeOfStarting = call.OpeningTime,
                    TimeOfEnding = AdminManager.Now,
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
                    TimeOfEnding = AdminManager.Now,
                    TypeOfEnding = DO.TypeOfEnding.CancellationExpired
                }));

        }

    }
    /// <summary>
    /// Accepts a Call Id and returns it status based on the CallStatus enum values
    /// </summary>
    /// <param name="callID">The Call Id value</param>
    /// <returns>The status of the call</returns>
    /// <exception cref="DO.DalDoesNotExistException">Throws an exception if such call doesn't exists</exception>
    public static BO.CallStatus GetStatus(int callID)
    {
        DO.Call res;
        DO.Assignment? resAssignments;
        lock (AdminManager.BlMutex)//stage 7
        {
            res = s_dal.Call.Read(call => call.Id == callID)
            ?? throw new DO.DalDoesNotExistException("Call Does not exist");

             resAssignments = s_dal.Assignment.ReadAll(ass => ass.CallId == callID).LastOrDefault();
        }
        if (res.DeadLine <= AdminManager.Now)
            return BO.CallStatus.Expiered;

        if (resAssignments is not null && resAssignments.TypeOfEnding == DO.TypeOfEnding.Treated)
            return BO.CallStatus.Closed;
        lock (AdminManager.BlMutex)
        {
            if (resAssignments is not null && resAssignments.TypeOfEnding is not null)
            {
                return ((res.DeadLine - AdminManager.Now) <= s_dal.Config.RiskRange)
                    ? BO.CallStatus.OpenAndRisky
                    : BO.CallStatus.Open;
            }


            if (resAssignments is not null && resAssignments.TypeOfEnding is null)
            {
                return ((res.DeadLine - AdminManager.Now) <= s_dal.Config.RiskRange)
                    ? BO.CallStatus.InProgressAndRisky
                    : BO.CallStatus.InProgress;
            }

            //If there is not assingment - no one took the call therefor the call is open
            if (resAssignments is null)
            {
                return ((res.DeadLine - AdminManager.Now) <= s_dal.Config.RiskRange)
                        ? BO.CallStatus.OpenAndRisky
                        : BO.CallStatus.Open;
            }
        }
            throw new Exception("Brahhhh whatsha duing");
    }

    /// <summary>
    /// This method checks if the given BO Call entity is valid
    /// </summary>
    /// <param name="call">The call to be reviewed</param>
    /// <returns>a boolean value whether the entity is valid or not</returns>
    /// <exception cref="BO.BlInvalidEntityDetails"></exception>
    internal static async Task<bool> IsCallValid(BO.Call call)
    {
        try
        {
            //Check if the times are valid
            if (call.CallStartTime > call.CallDeadLine || call.CallDeadLine < AdminManager.Now)
                throw new BO.BlInvalidEntityDetails("BL: The deadline of the call cannot be before the start time of the call");

            //Checks if the address is valid (if cordinates exist)
            (double? lat, double? lng) = await VolunteerManager.GetGeoCordinates(call.CallAddress);
            if (lat == null || lng == null)
                throw new BO.BlInvalidEntityDetails($"BL: The given call address ({call.CallAddress}) is not a real address");
        }
        catch(BO.BlInvalidEntityDetails ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
        return true;
    }
    
    /// <summary>
    /// This method converts between the BO version of Call to the DO version of call
    /// </summary>
    /// <param name="call">The original BO Call entity</param>
    /// <returns>The converted DO Call entity</returns>
    internal static DO.Call ConvertFromBdToD(BO.Call call)
    =>
        new DO.Call
        {
            Id = call.Id,
            Type = (DO.CallType)call.TypeOfCall,
            FullAddressCall = call.CallAddress,
            Latitude = call.Latitude,
            Longitude = call.Longitude,
            OpeningTime = AdminManager.Now,
            Description = call.Description,
            DeadLine = call.CallDeadLine
        };
}
