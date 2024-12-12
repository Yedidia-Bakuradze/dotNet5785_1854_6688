namespace BlImplementation;
using BlApi;
using Helpers;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal s_dal = DalApi.Factory.Get;
    public void AddCall(BO.Call call)
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
        
        //Create new Dal entity
        DO.Call newCall = new DO.Call
        {
            Id = call.Id,
            Type = (DO.CallType)call.TypeOfCall,
            FullAddressCall = call.CallAddress,
            Latitude = call.Latitude,
            Longitude = call.Longitude,
            OpeningTime = call.CallStartTime,
            Description = call.Description,
            DeadLine = call.CallDeadLine
        };
        s_dal.Call.Create(newCall);
    }

    public void DeleteCallRequest(int requestId)
    {
        try
        {
            if (s_dal.Assignment.ReadAll((DO.Assignment ass) => ass.CallId == requestId) != null)
            {
                throw new BO.BlAlreadyExistsException("BL Exception:", new DO.DalAlreadyExistsException("DAL Exception:"));
            }
            // Attempt to delete the callnew 
            s_dal.Call.Delete(requestId);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException("Bl Exception: id does not exist", ex) ; 
        }
    }

    public void EndOfCallStatusUpdate(int VolunteerId, int callId)
    {

        DO.Assignment? res = s_dal.Assignment.Read(ass => ass.Id == callId && ass.VolunteerId == VolunteerId) ?? throw new BO.BlDoesNotExistException("BL : Assignement does not exist", new DO.DalDoesNotExistException(""));
        if (res?.TypeOfEnding != null || res?.TimeOfEnding != null)
        {
            throw new BO.BlForbidenSystemActionExeption("BL: Cant update the assignment");
        }
        try
        {
            s_dal.Assignment.Update(res! with
            {
                TypeOfEnding = DO.TypeOfEnding.Treated,
                TimeOfEnding = ClockManager.Now
            });
        }
        catch(DO.DalDoesNotExistException ex) 
        {
            throw new BO.BlDoesNotExistException("Bl: ASsignement does not exist", ex);
        }
    }

    /// <summary>
    /// Retrieves a list of closed calls assigned to a specific volunteer, with optional filtering and sorting.
    /// </summary>
    /// <param name="VolunteerId">The ID of the volunteer.</param>
    /// <param name="callType">Optional filter for the type of call.</param>
    /// <param name="parameter">Optional parameter for sorting the resulting list.</param>
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

        // Step 5: Sort the list based on the specified parameter
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
                    throw new BO.BlInvalidEnumValueOperationException("Invalid sorting parameter");
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
            throw new BO.BoDoesNotExistException("BL: Call does not exist");
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


    public IEnumerable<BO.CallInList> GetListOfCalls(BO.CallInListFields? parameter = null, object? value = null, BO.CallInListFields? parameter1 = null)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteer(int id, BO.CallType? callType, BO.OpenCallFields? parameter)
    {
        throw new NotImplementedException();
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

    public void UpdateCall(BO.Call call)
    {
        throw new NotImplementedException();
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
