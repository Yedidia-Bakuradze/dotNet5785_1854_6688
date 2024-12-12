namespace BlImplementation;
using BlApi;
using BO;
using DO;
using Helpers;
using System.Security.Cryptography;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal s_dal = DalApi.Factory.Get;
    /// <summary>
    /// Adds a new call to the system after validating its details.
    /// </summary>
    /// <param name="call">The call object containing details to be added.</param>
    /// <exception cref="BO.BlInvalidEntityDetails">Thrown if the call's times are invalid or the address is not a real location.</exception>
    public void AddCall(BO.Call call)
    {
        //Check if the call entity is valid or not
        if(!CallManager.IsCallValid(call))
            throw new BO.BlInvalidEntityDetails($"BL: The call entity (Id: {call.Id}) doesn't contain valid values.");
        
        //Get Call cordinates
        (double? lat, double? lng) = VolunteerManager.GetGeoCordinates(call.CallAddress);
        if (lat == null || lng == null)
            throw new BO.BlInvalidEntityDetails($"BL: The given call address ({call.CallAddress}) is not a real address");

        //Create new Dal entity
        DO.Call newCall = CallManager.ConvertFromBdToD(call);

        // Add the new call to the database
        s_dal.Call.Create(newCall);
    }

    /// <summary>
    /// Deletes a call request by its request ID.
    /// </summary>
    /// <param name="requestId">The ID of the call request to delete.</param>
    /// <exception cref="BO.BlAlreadyExistsException">Thrown if there are assignments related to the call that prevent deletion.</exception>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the call with the given ID does not exist in the database.</exception>
    public void DeleteCallRequest(int requestId)
    {
        try
        {
            // Check if there are any assignments related to the call
            if (s_dal.Assignment.ReadAll((DO.Assignment ass) => ass.CallId == requestId) != null)
            {
                throw new BO.BlAlreadyExistsException("BL Exception:", new DO.DalAlreadyExistsException("DAL Exception:"));
            }
            // Attempt to delete the call
            s_dal.Call.Delete(requestId);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            // If the call does not exist, throw an exception
            throw new BO.BlDoesNotExistException("Bl Exception: id does not exist", ex);
        }
    }

    /// <summary>
    /// Updates the status of an assignment when the call ends.
    /// </summary>
    /// <param name="VolunteerId">The ID of the volunteer associated with the assignment.</param>
    /// <param name="callId">The ID of the call associated with the assignment.</param>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the assignment does not exist.</exception>
    /// <exception cref="BO.BlForbidenSystemActionExeption">Thrown if the assignment has already been ended or is not allowed to be updated.</exception>
    public void EndOfCallStatusUpdate(int VolunteerId, int callId)
    {
        // Retrieve the assignment details for the given volunteer and call ID
        DO.Assignment? res = s_dal.Assignment.Read(ass => ass.Id == callId && ass.VolunteerId == VolunteerId) ?? throw new BO.BlDoesNotExistException("BL : Assignment does not exist", new DO.DalDoesNotExistException(""));

        // Check if the assignment already has an ending type or time
        if (res?.TypeOfEnding != null || res?.TimeOfEnding != null)
        {
            throw new BO.BlForbidenSystemActionExeption("BL: Can't update the assignment");
        }

        try
        {
            // Update the assignment with the new ending type and time
            s_dal.Assignment.Update(res! with
            {
                TypeOfEnding = DO.TypeOfEnding.Treated,
                TimeOfEnding = ClockManager.Now
            });
        }
        catch (DO.DalDoesNotExistException ex)
        {
            // If the assignment does not exist in the database, throw an exception
            throw new BO.BlDoesNotExistException("Bl: Assignment does not exist", ex);
        }
    }

    /// <summary>
    /// Retrieves a list of closed calls assigned to a specific volunteer, with optional filtering and sorting.
    /// </summary>
    /// <param name="VolunteerId">The ID of the volunteer.</param>
    /// <param name="callType">Optional filter for the type of call.</param>
    /// <param name="parameter">Optional filterField for sorting the resulting list.</param>
    /// <returns>A sorted and filtered list of closed calls.</returns>
    public IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int VolunteerId, BO.CallType? callType, BO.ClosedCallInListFields? parameter)
    {
        // Step 1: Fetch all assignments for the given volunteer
        List<DO.Assignment> res = s_dal.Assignment.ReadAll(ass => ass.VolunteerId == VolunteerId).ToList();

        // Step 2: Fetch all calls related to the assignments where TypeOfEnding is not null
        List<DO.Call> finalRes = s_dal.Call.ReadAll(call => res.Any(ass => ass.CallId == call.Id && ass.TypeOfEnding != null)).ToList();

        // Step 3: Filter calls by type if callType is provided
        if (callType != null)
        {
            finalRes = finalRes.Where(call => call.Type == (DO.CallType)callType).ToList();
        }

        // Step 4: Create a list of ClosedCallInList objects
        List<BO.ClosedCallInList> closedCalls = finalRes.Select(call => new BO.ClosedCallInList
        {
            Id = call.Id,
            TypeOfCall = (BO.CallType)call.Type,
            CallAddress = call.FullAddressCall,
            CallStartTime = call.OpeningTime,
            EnteryTime = res.Find(ass => ass.CallId == call.Id)!.TimeOfStarting, // Access the start time from the corresponding assignment
            ClosingTime = res.Find(ass => ass.CallId == call.Id)?.TimeOfEnding, // Access the ending time from the corresponding assignment
            TypeOfClosedCall = (BO.ClosedCallType)(res.Find(ass => ass.CallId == call.Id))?.TypeOfEnding!// Default to Unknown if TypeOfEnding is null
        }).ToList();

        // Step 5: Sort the list based on the specified filterField
        if (parameter != null)
        {
            switch (parameter)
            {
                case BO.ClosedCallInListFields.Id:
                    closedCalls = closedCalls.OrderBy(call => call.Id).ToList();
                    break;
                case BO.ClosedCallInListFields.TypeOfCall:
                    closedCalls = closedCalls.OrderBy(call => call.TypeOfCall).ToList();
                    break;
                case BO.ClosedCallInListFields.CallAddress:
                    closedCalls = closedCalls.OrderBy(call => call.CallAddress).ToList();
                    break;
                case BO.ClosedCallInListFields.CallStartTime:
                    closedCalls = closedCalls.OrderBy(call => call.CallStartTime).ToList();
                    break;
                case BO.ClosedCallInListFields.EnteryTime:
                    closedCalls = closedCalls.OrderBy(call => call.EnteryTime).ToList();
                    break;
                case BO.ClosedCallInListFields.ClosingTime:
                    closedCalls = closedCalls.OrderBy(call => call.ClosingTime).ToList();
                    break;
                case BO.ClosedCallInListFields.TypeOfClosedCall:
                    closedCalls = closedCalls.OrderBy(call => call.TypeOfClosedCall).ToList();
                    break;
                default:
                    throw new BO.BlInvalidEnumValueOperationException("Invalid sorting filterField");
            }
        }

        // Step 6: Return the final list
        return closedCalls;
    }

    /// <summary>
    /// Retrieves the details of a call based on the provided call ID.
    /// </summary>
    public BO.Call GetDetielsOfCall(int callId)
    {
        // Fetch the call details from the data layer (DAL) by the given call ID.
        DO.Call call = s_dal.Call.Read(call => call.Id == callId)!;

        // Fetch all assignments related to the call ID.
        List<DO.Assignment> res = s_dal.Assignment.ReadAll(ass => ass.CallId == callId).ToList();

        // If the call does not exist, throw an exception with a relevant message.
        if (call == null)
        {
            throw new BO.BlDoesNotExistException("BL: Call does not exist");
        }

        // Map the fetched call details and assignments to a BO (Business Object) call.
        BO.Call boCall = new BO.Call
        {
            Id = call.Id,  // Set the call ID
            TypeOfCall = (BO.CallType)call.Type,  // Convert the call type to the corresponding BO enum
            Description = call.Description,  // Set the call description
            CallAddress = call.FullAddressCall,  // Set the full address of the call
            Latitude = call.Latitude,  // Set the latitude of the call
            Longitude = call.Longitude,  // Set the longitude of the call
            CallStartTime = call.OpeningTime,  // Set the opening time of the call
            CallDeadLine = call.DeadLine,  // Set the deadline for the call
            Status = CallManager.GetStatus(call.Id),  // Get the status of the call using the CallManager
            MyAssignments = res.Select((assignment) => new BO.CallAssignInList
            {
                VolunteerId = assignment.VolunteerId,  // Set the volunteer ID for the assignment
                VolunteerName = s_dal.Volunteer.Read(vol => vol.Id == assignment.VolunteerId)?.FullName,  // Get the volunteer's full name based on the ID
                EntryTime = assignment.TimeOfStarting,  // Set the start time of the assignment
                FinishTime = assignment.TimeOfEnding,  // Set the finish time of the assignment
                TypeOfClosedCall = (BO.ClosedCallType)(res.Find(ass => ass.CallId == call.Id))?.TypeOfEnding!  // Set the type of the closed call if found
            }).ToList()  // Convert the assignments to a list of BO.CallAssignInList objects
        };

        // Return the populated BO.Call object containing all relevant details.
        return boCall;
    }

    /// <summary>
    /// Retrieves a list of calls with optional filtering and sorting based on specified fields.
    /// </summary>
    /// <param name="filterField">The field by which to filter the calls. Optional.</param>
    /// <param name="filterValue">The value to filter by. Optional.</param>
    /// <param name="sortingField">The field by which to sort the calls. Optional.</param>
    /// <returns>An IEnumerable of CallInList objects representing the filtered and sorted list of calls.</returns>
    public IEnumerable<BO.CallInList> GetListOfCalls(BO.CallInListFields? filterField = null, object? filterValue = null, BO.CallInListFields? sortingField = null)
    {
        // Retrieve all calls and assignments from the database
        List<DO.Call> calls = s_dal.Call.ReadAll().ToList();
        List<DO.Assignment> assignments = s_dal.Assignment.ReadAll().ToList();

        // Build the initial list of CallInList objects based on the calls and their assignments
        IEnumerable<BO.CallInList> callsInlist = from call in calls
                                                 let callsAssignments = from assignment in assignments
                                                                        where assignment.CallId == call.Id
                                                                        orderby assignment.Id descending
                                                                        select assignment
                                                 select new BO.CallInList
                                                 {
                                                     // Set the properties for each CallInList object
                                                     Id = callsAssignments.First().Id,
                                                     CallId = call.Id,
                                                     Status = CallManager.GetStatus(call.Id),
                                                     OpenningTime = call.OpeningTime,
                                                     LastVolunteerName = s_dal.Volunteer.Read(vol => vol.Id == callsAssignments.First().VolunteerId)?.FullName,
                                                     TimeToEnd = call.DeadLine - ClockManager.Now,
                                                     TypeOfCall = (BO.CallType)call.Type,
                                                     TimeElapsed = (callsAssignments.First().TypeOfEnding != null)
                                                         ? callsAssignments.First().TimeOfEnding - callsAssignments.First().TimeOfStarting
                                                         : null,
                                                     TotalAlocations = callsAssignments.Count(),
                                                 };

        // Filtering the list based on the specified filter field and value
        if (filterField != null)
        {
            switch (filterField)
            {
                case CallInListFields.Id:
                    callsInlist = from call in callsInlist
                                  where call.Id == (int)filterValue!
                                  select call;
                    break;
                case CallInListFields.CallId:
                    callsInlist = from call in callsInlist
                                  where call.CallId == (int)filterValue!
                                  select call;
                    break;
                case CallInListFields.TypeOfCall:
                    callsInlist = from call in callsInlist
                                  where call.TypeOfCall == (BO.CallType)filterValue!
                                  select call;
                    break;
                case CallInListFields.OpenningTime:
                    callsInlist = from call in callsInlist
                                  where call.OpenningTime == (DateTime)filterValue!
                                  select call;
                    break;
                case CallInListFields.TimeToEnd:
                    callsInlist = from call in callsInlist
                                  where call.TimeToEnd == (TimeSpan)filterValue!
                                  select call;
                    break;
                case CallInListFields.LastVolunteerName:
                    callsInlist = from call in callsInlist
                                  where call.LastVolunteerName == (string)filterValue!
                                  select call;
                    break;
                case CallInListFields.TimeElapsed:
                    callsInlist = from call in callsInlist
                                  where call.TimeElapsed == (TimeSpan)filterValue!
                                  select call;
                    break;
                case CallInListFields.Status:
                    callsInlist = from call in callsInlist
                                  where call.Status == (CallStatus)filterValue!
                                  select call;
                    break;
                case CallInListFields.TotalAlocations:
                    callsInlist = from call in callsInlist
                                  where call.TotalAlocations == (int)filterValue!
                                  select call;
                    break;
                case null:
                    throw new BO.BlInvalidEnumValueOperationException($"Bl: Filter value is null");
                    break;
            }
        }

        // Sorting the list based on the specified sorting field
        switch (sortingField)
        {
            case CallInListFields.Id:
                return from call in callsInlist
                       orderby call.Id
                       select call;
            case CallInListFields.CallId:
                return from call in callsInlist
                       orderby call.CallId
                       select call;
            case CallInListFields.TypeOfCall:
                return from call in callsInlist
                       orderby call.TypeOfCall
                       select call;
            case CallInListFields.OpenningTime:
                return from call in callsInlist
                       orderby call.OpenningTime
                       select call;
            case CallInListFields.TimeToEnd:
                return from call in callsInlist
                       orderby call.TimeToEnd
                       select call;
            case CallInListFields.LastVolunteerName:
                return from call in callsInlist
                       orderby call.LastVolunteerName
                       select call;
            case CallInListFields.TimeElapsed:
                return from call in callsInlist
                       orderby call.TimeElapsed
                       select call;
            case CallInListFields.Status:
                return from call in callsInlist
                       orderby call.Status
                       select call;
            case CallInListFields.TotalAlocations:
                return from call in callsInlist
                       orderby call.TotalAlocations
                       select call;
            case null:
                break;
        }

        // If no sorting field is provided, return the list sorted by CallId as default
        return from call in callsInlist
               orderby call.CallId
               select call;
    }

    public IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteer(int VolunteerId, BO.CallType? callType, BO.OpenCallFields? sortingField)
    {
        DO.Volunteer volunteer = s_dal.Volunteer.Read(vol => vol.Id == VolunteerId)
            ?? throw new BO.BlDoesNotExistException($"Bl: Volunteer (Id: {VolunteerId} doesn't exists)");


        List<BO.OpenCallInList> openCalls = s_dal.Call
            .ReadAll(call => CallManager.GetStatus(call.Id) == CallStatus.Open || CallManager.GetStatus(call.Id) == CallStatus.OpenAndRisky)
            .Select(call => new BO.OpenCallInList
        {
            CallId = call.Id,
            CallFullAddress = call.FullAddressCall,
            Description = call.Description,
            LastTimeForClosingTheCall = call.DeadLine,
            TypeOfCall = (BO.CallType)call.Type,
            OpenningTime = call.OpeningTime,
            DistanceFromVolunteer = volunteer.FullCurrentAddress is null 
                ? -1 
                :VolunteerManager.CalculateDistanceFromVolunteerToCall(volunteer.FullCurrentAddress, call.FullAddressCall, volunteer.RangeType)
        }).ToList();

        if(callType != null)
        {
            openCalls = openCalls.Where(call => call.TypeOfCall == callType).ToList();
        }
        if(sortingField == null)
        {
            return openCalls.OrderBy(call => call.CallId);
        }
        switch (sortingField)
        {
            case OpenCallFields.CallId:
                openCalls = openCalls.OrderBy(call => call.CallId).ToList();
                break;
            case OpenCallFields.TypeOfCall:
                openCalls = openCalls.OrderBy(call => call.TypeOfCall).ToList();
                break;
            case OpenCallFields.Description:
                openCalls = openCalls.OrderBy(call => call.Description).ToList();
                break;
            case OpenCallFields.CallFullAddress:
                openCalls = openCalls.OrderBy(call => call.CallFullAddress).ToList();
                break;
            case OpenCallFields.OpenningTime:
                openCalls = openCalls.OrderBy(call => call.OpenningTime).ToList();
                break;
            case OpenCallFields.LastTimeForClosingTheCall:
                openCalls = openCalls.OrderBy(call => call.LastTimeForClosingTheCall).ToList();
                break;
            case OpenCallFields.DistanceFromVolunteer:
                openCalls = openCalls.OrderBy(call => call.DistanceFromVolunteer).ToList();
                break;
            default:
                throw new BO.BlInvalidEnumValueOperationException("Invalid sorting filterField");
                break;
        }
        return openCalls;
    }

    public int[] GetTotalCallsByStatus()
    {
        throw new NotImplementedException();
    }

    public void Read(int callId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<BO.CallInList> ReadAll()
    {
        throw new NotImplementedException();
    }

    public void SelectCallToDo(int VolunteerId, int callId)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// This method updates the DO Call with the past BO Call field values
    /// </summary>
    /// <param name="call">The new (BO) Call entity with the new values</param>
    /// <exception cref="BO.BlInvalidEntityDetails">Thrown if the Call values are not valid</exception>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if such call doesn't exist</exception>
    public void UpdateCall(BO.Call call)
    {
        if (!CallManager.IsCallValid(call))
            throw new BO.BlInvalidEntityDetails($"BL: Call (Id: {call.Id}) has invalid details");

        (double? lat, double? log) = VolunteerManager.GetGeoCordinates(call.CallAddress);

        call.Latitude = (double) lat!;
        call.Longitude= (double) log!;

        DO.Call newCall = CallManager.ConvertFromBdToD(call);

        try
        {
            s_dal.Call.Update(newCall);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"BL: Call with Id: {newCall.Id} doesn't exists");
        }
    }

    /// <summary>
    /// This method updates the Call status with the past callId
    /// The operation is allowed only if the call is opened and the volunteer which requests this modification is the assigned volunteer to that call
    /// </summary>
    /// <param name="VolunteerId">The volunteer which requests the modification</param>
    /// <param name="callId">The call if of the Call which is needed to be updated</param>
    /// <exception cref="BO.BlDoesNotExistException">Thrown when the assignment doesn't exists</exception>
    /// <exception cref="BO.BlForbidenSystemActionExeption">Thrown when the opration is forbidden due to restriction and access level of the volunteer</exception>
    public void UpdateCallEnd(int VolunteerId, int callId)
    {
        //Check access (if the user which wants to change the call status is the same uesr which assigned to that call)
        DO.Assignment assignment = s_dal.Assignment.Read((assignment) => assignment.CallId == callId)
            ?? throw new BO.BlDoesNotExistException($"BL: Call (Id: {callId}) for Volunteer (Id: {VolunteerId}) doesn't exists");

        if (assignment.CallId != VolunteerId)
            throw new BO.BlForbidenSystemActionExeption($"BL: The volunteer (Id: {VolunteerId}) is not allowed to modify Call assinged to different volunteer (Id: {assignment.CallId})");

        //Check that the call is not ended (Cancled, Expiered or completed)
        if (assignment.TypeOfEnding != null || assignment.TypeOfEnding == DO.TypeOfEnding.Treated)
            throw new BO.BlForbidenSystemActionExeption($"BL: Unable to modify the call. Alrady ended with status of: {assignment.TypeOfEnding}, by volunteer Id: {assignment.VolunteerId})");

        //Throw exception if access not granted or if there is Dal exception

        //Update the Dal entity with current system time and Closed status
        DO.Assignment newAssignment = assignment with
        {
            TypeOfEnding = DO.TypeOfEnding.Treated,
            TimeOfEnding = ClockManager.Now,
        };

        try
        {
            s_dal.Assignment.Update(newAssignment);
        }
        catch(DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"BL: Assignment with Id: {newAssignment.Id} doesn't exists", ex);
        }

    }
}
