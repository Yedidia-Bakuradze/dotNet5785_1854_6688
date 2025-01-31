using BO;
using DalApi;
using DO;
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
                ?? throw new BO.BlDoesNotExistException($"Bl Says: There is no call with ID of {callID}");

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
    internal static void IsCallValid(BO.Call call)
    {
        //Check if the times are valid
        if (call.CallStartTime > call.CallDeadLine || call.CallDeadLine < AdminManager.Now)
            throw new BO.BlInvalidEntityDetails("BL: The deadline of the call cannot be before the start time of the call");
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
            Latitude = null,
            Longitude = null,
            OpeningTime = AdminManager.Now,
            Description = call.Description,
            DeadLine = call.CallDeadLine
        };

    internal static async Task UpdateCallCordinates(int callId, string address, bool isNewCall)
    {
        (double,double) coridnates = await VolunteerManager.GetGeoCordinates(address);

        if (!VolunteerManager.CordinatesValidator(coridnates))
        {
            if (isNewCall)
            {
                lock (AdminManager.BlMutex)
                {
                    s_dal.Call.Delete(callId);
                }
                Observers.NotifyItemUpdated(callId);
                Observers.NotifyListUpdated();
            }
            throw new BlInvalidCordinatesConversionException(address);

        }
        
        lock (AdminManager.BlMutex)
        {
            DO.Call call = s_dal.Call.Read(callId)
                ?? throw new BlDoesNotExistException($"Bl Says: Call with ID {callId} does not exist");

            s_dal.Call.Update(call with
            {
                Latitude = (double)coridnates.Item1!,
                Longitude = (double)coridnates.Item2!,
            });
        }

        Observers.NotifyItemUpdated(callId);
        Observers.NotifyListUpdated();
    }

    internal static void VerifyCallDeletionAttempt(int callId)
    {
        lock (AdminManager.BlMutex)
        {
            // Check if there are any assignments related to the call
            if (s_dal.Assignment.ReadAll(assignment => assignment.CallId == callId).Any(assignment => assignment.VolunteerId != 0))
                throw new BlForbidenSystemActionExeption($"BL: Unable to delete call {callId} since it has a record with other volunteers");
        }

        if (CallManager.GetStatus(callId) != CallStatus.Open && CallManager.GetStatus(callId) != CallStatus.OpenAndRisky)
            throw new BlForbidenSystemActionExeption($"BL: Unable to delete call {callId} since it's status is not open");
    }

    #region Assignment Logic Methods
    internal static void VerifyAssignmentFinishAttept(int VolunteerId, int aassignmentId, out DO.Assignment res)
    {
        // Retrieve the assignment details for the given volunteer and call ID
        lock (AdminManager.BlMutex)
        {
            res = s_dal.Assignment.Read(ass => ass.Id == aassignmentId && ass.VolunteerId == VolunteerId)
                ?? throw new BO.BlDoesNotExistException("BL : Assignment does not exist");
        }

        // Check if the assignment already has an ending type or time
        if (res?.TypeOfEnding != null || res?.TimeOfEnding != null)
            throw new BO.BlForbidenSystemActionExeption("BL: Can't update the assignment");
        

    }

    internal static void VerifyAssignmentAllocationAttempt(int VolunteerId, int callId)
    {
        DO.Call call;
        DO.Assignment? callAssignment;
        lock (AdminManager.BlMutex)
        {
            //Check if there is such call
            call = s_dal.Call.Read((call) => call.Id == callId)
                ?? throw new BO.BlDoesNotExistException($"Bl: Call with Id: {callId} doesn't exists");
            //Check if the call hasn't been taken by someone else
            callAssignment = s_dal.Assignment.ReadAll((assignment) => assignment.CallId == callId).LastOrDefault();
        }
        lock (AdminManager.BlMutex)
        {
            var existingAssignment = s_dal.Assignment.Read(assing => assing.VolunteerId == VolunteerId && assing.TypeOfEnding is null);
            if (existingAssignment is not null)
                throw new BO.BlForbidenSystemActionExeption($"BL Says: Cann't assigned call {callId} to volunteer {VolunteerId} because it has a running call ({existingAssignment.CallId})");

        }

        //Check if call already been taken
        if (callAssignment != null && callAssignment.TypeOfEnding is null)
            throw new BO.BlForbidenSystemActionExeption($"Bl: Call {callId} already taken by other volunteer (Id: {callAssignment.VolunteerId})");

        //Check if there is time to complete the call
        if (call.DeadLine - AdminManager.Now <= TimeSpan.Zero)
            throw new BO.BlForbidenSystemActionExeption($"Bl: Call {callId} already expired");
    }
    
    internal static void VerifyAssignmentCancelAttempt(int VolunteerId, int assignmentId,out DO.Assignment assignment)
    {
        DO.Assignment? tmp;
        DO.Volunteer? assosiatedVolunteer;
        lock (AdminManager.BlMutex)
        {
            tmp = s_dal.Assignment.Read(assignmentId);
        }

        //Check access (if the user which wants to change the call status is the same uesr which assigned to that call)
        if(tmp is null)
            throw new BO.BlDoesNotExistException($"BL: Assignment (Id: {assignmentId}) for Volunteer (Id: {VolunteerId}) doesn't exists");
        
        lock (AdminManager.BlMutex)
        {
            assosiatedVolunteer = s_dal.Volunteer.Read(VolunteerId);
        }
        
        //Check if there is such as user
        if (assosiatedVolunteer is null)
            throw new BlDoesNotExistException($"Bl Says: No volunteer with ID {VolunteerId} has been found");
        
        //Throw exception if access not granted
        if (tmp.VolunteerId != VolunteerId && (BO.UserRole)assosiatedVolunteer.Role is not BO.UserRole.Admin)
            throw new BO.BlForbidenSystemActionExeption($"BL: The volunteer (Id: {VolunteerId}) is not allowed to modify Call assinged to different volunteer (Id: {tmp.CallId})");

        //Check that the call is not ended (Cancled, Expiered or completed)
        if (tmp.TypeOfEnding == DO.TypeOfEnding.Treated)
            throw new BO.BlForbidenSystemActionExeption($"BL: Unable to modify the call. Alrady ended with status of: {tmp.TypeOfEnding}, by volunteer Id: {tmp.VolunteerId})");

        assignment = tmp;
    }
    #endregion
}
