namespace BlImplementation;
using BlApi;
using BO;
using Helpers;
using System.Reflection.Metadata.Ecma335;

internal class CallImplementation : ICall
{
    // The implemention of the function in the observer 
    #region Stage 5
    public void AddObserver(Action listObserver) =>
        CallManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
        CallManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
        CallManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
        CallManager.Observers.RemoveObserver(id, observer); //stage 5
    #endregion Stage 5

    private object lockObject = new();
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

        //Cordinates are updated
        call.Latitude = (double)lat!;
        call.Longitude= (double)lng!;

        //Create new Dal entity
        DO.Call newCall = CallManager.ConvertFromBdToD(call);

        // Add the new call to the database
        s_dal.Call.Create(newCall);

        //Notifies all observers that a call has been added
        CallManager.Observers.NotifyListUpdated();
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
            if (s_dal.Assignment.ReadAll(assignment => assignment.CallId == requestId).Any(assignment => assignment.VolunteerId != 0))
                throw new BlForbidenSystemActionExeption($"BL: Unable to delete call {requestId} since it has a record with other volunteers");

            if (CallManager.GetStatus(requestId) != CallStatus.Open && CallManager.GetStatus(requestId) != CallStatus.OpenAndRisky)
                throw new BlForbidenSystemActionExeption($"BL: Unable to delete call {requestId} since it's status is not open");

            // Attempt to delete the call
            s_dal.Call.Delete(requestId);

            //Notifies all observers that a call has been added
            CallManager.Observers.NotifyListUpdated();
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
    public void FinishAssignement(int VolunteerId, int callId)
    {
        // Retrieve the assignment details for the given volunteer and call ID
        DO.Assignment? res = s_dal.Assignment.Read(ass => ass.Id == callId && ass.VolunteerId == VolunteerId)
            ?? throw new BO.BlDoesNotExistException("BL : Assignment does not exist");

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
                TimeOfEnding = AdminManager.Now
            });
            CallManager.Observers.NotifyListUpdated();
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
            EnteryTime = res.FirstOrDefault(ass => ass.CallId == call.Id).TimeOfStarting, // Access the start time from the corresponding assignment
            ClosingTime = (DateTime) res.Find(ass => ass.CallId == call.Id)!.TimeOfEnding!, // Access the ending time from the corresponding assignment
            TypeOfClosedCall = (BO.TypeOfEndingCall)(res.Find(ass => ass.CallId == call.Id))?.TypeOfEnding!// Default to Unknown if TypeOfEnding is null
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
                    throw new BO.BlInvalidOperationException("Invalid sorting filterField");
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
        DO.Call call = s_dal.Call.Read(call => call.Id == callId)
            ?? throw new BlDoesNotExistException($"BL: Call with id {callId} doesn't exists");

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
                TypeOfClosedCall = (BO.TypeOfEndingCall?)(res.Find(ass => ass.CallId == call.Id))?.TypeOfEnding  // Set the type of the closed call if found
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
    public IEnumerable<BO.CallInList> GetListOfCalls(BO.CallInListFields? filterField = null, object? filterValue = null, BO.CallInListFields? sortingField = null, IEnumerable<BO.CallInList>? source = null)
    {
        // Retrieve all calls and assignments from the database
        List<DO.Call> calls = s_dal.Call.ReadAll().ToList();
        List<DO.Assignment> assignments = s_dal.Assignment.ReadAll().ToList();

        // Build the initial list of CallInList objects based on the calls and their assignments
        IEnumerable<BO.CallInList> callsInlist = source is not null
                                                ? source
                                                :
                                                from call in calls
                                                 let callsAssignments = assignments
                                                     .Where(ass => ass.CallId == call.Id)
                                                     .OrderByDescending(ass => ass.Id)
                                                     .ToList()
                                                 let firstAssignment = callsAssignments.FirstOrDefault()
                                                 select new BO.CallInList
                                                 {
                                                     Id = firstAssignment?.Id, // Default to 0 if no assignment exists
                                                     CallId = call.Id,
                                                     Status = CallManager.GetStatus(call.Id),
                                                     OpenningTime = call.OpeningTime,
                                                     LastVolunteerName = firstAssignment != null
                                                         ? s_dal.Volunteer.Read(vol => vol.Id == firstAssignment.VolunteerId)?.FullName
                                                         : null,
                                                     TimeToEnd = call.DeadLine - AdminManager.Now,
                                                     TypeOfCall = (BO.CallType)call.Type,
                                                     TimeElapsed = firstAssignment?.TypeOfEnding != null
                                                         ? firstAssignment.TimeOfEnding - firstAssignment.TimeOfStarting
                                                         : null,
                                                     TotalAlocations = callsAssignments.Count(),
                                                 };

        // Filtering the list based on the specified filter field and value
        if (filterField != null && filterValue != null)
        {
            switch (filterField)
            {
                case BO.CallInListFields.Id:
                    callsInlist = (IEnumerable<CallInList>)(from call in callsInlist
                                  where call.Id == Convert.ToInt32(filterValue)
                                  select call).ToList();
                    break;
                case BO.CallInListFields.CallId:
                    callsInlist = from call in callsInlist
                                  where call.CallId == Convert.ToInt32(filterValue)
                                  select call;
                    break;
                case BO.CallInListFields.TypeOfCall:
                    callsInlist = from call in callsInlist
                                  where call.TypeOfCall == (BO.CallType)Enum.Parse(typeof(BO.CallType), filterValue!.ToString()!)
                                  select call;
                    break;
                case BO.CallInListFields.OpenningTime:
                    callsInlist = from call in callsInlist
                                  where call.OpenningTime == Convert.ToDateTime(filterValue)
                                  select call;
                    break;
                case BO.CallInListFields.TimeToEnd:
                    callsInlist = from call in callsInlist
                                  where call.TimeToEnd == TimeSpan.Parse(filterValue!.ToString()!)
                                  select call;
                    break;
                case BO.CallInListFields.LastVolunteerName:
                    callsInlist = from call in callsInlist
                                  where call.LastVolunteerName == (string)filterValue!
                                  select call;
                    break;
                case BO.CallInListFields.TimeElapsed:
                    callsInlist = from call in callsInlist
                                  where call.TimeElapsed == TimeSpan.Parse(filterValue!.ToString()!)
                                  select call;
                    break;
                case BO.CallInListFields.Status:
                    callsInlist = from call in callsInlist
                                  where call.Status == (BO.CallStatus)Enum.Parse(typeof(BO.CallStatus), filterValue!.ToString()!)
                                  select call;
                    break;
                case BO.CallInListFields.TotalAlocations:
                    callsInlist = from call in callsInlist
                                  where call.TotalAlocations == Convert.ToInt32(filterValue)
                                  select call;
                    break;
                case null:
                    throw new BO.BlInvalidOperationException($"Bl: Filter value is null");
                    
            }
        }


        // Sorting the list based on the specified sorting field
        switch (sortingField)
        {
            case BO.CallInListFields.Id:
                return from call in callsInlist
                       orderby call.Id
                       select call;
            case BO.CallInListFields.CallId:
                return from call in callsInlist
                       orderby call.CallId
                       select call;
            case BO.CallInListFields.TypeOfCall:
                return from call in callsInlist
                       orderby call.TypeOfCall
                       select call;
            case BO.CallInListFields.OpenningTime:
                return from call in callsInlist
                       orderby call.OpenningTime
                       select call;
            case BO.CallInListFields.TimeToEnd:
                return from call in callsInlist
                       orderby call.TimeToEnd
                       select call;
            case BO.CallInListFields.LastVolunteerName:
                return from call in callsInlist
                       orderby call.LastVolunteerName
                       select call;
            case BO.CallInListFields.TimeElapsed:
                return from call in callsInlist
                       orderby call.TimeElapsed
                       select call;
            case BO.CallInListFields.Status:
                return from call in callsInlist
                       orderby call.Status
                       select call;
            case BO.CallInListFields.TotalAlocations:
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

    /// <summary>
    /// Retrieves a list of open calls available for a specific volunteer, 
    /// optionally filtered by call type and sorted by a specified field.
    /// </summary>
    /// <param name="VolunteerId">The ID of the volunteer for whom the open calls are fetched.</param>
    /// <param name="callType">An optional filter to include only calls of a specific type.</param>
    /// <param name="sortingField">An optional sorting field to order the returned calls.</param>
    /// <returns>A list of open calls formatted as <see cref="BO.OpenCallInList"/>.</returns>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the volunteer does not exist.</exception>
    /// <exception cref="BO.BlInvalidOperationException">Thrown if an invalid sorting field is provided.</exception>
    public IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteer(int VolunteerId, BO.CallType? callType, BO.OpenCallFields? sortingField)
    {
        // Retrieve the volunteer details. Throw an exception if the volunteer does not exist.
        DO.Volunteer volunteer = s_dal.Volunteer.Read(vol => vol.Id == VolunteerId)
            ?? throw new BO.BlDoesNotExistException($"Bl: Volunteer (Id: {VolunteerId} doesn't exist)");


        // Retrieve all open or risky calls and map them to BO.OpenCallInList objects.
        List<BO.OpenCallInList> openCalls = s_dal.Call
            .ReadAll(call => (CallManager.GetStatus(call.Id) == BO.CallStatus.Open || CallManager.GetStatus(call.Id) == BO.CallStatus.OpenAndRisky))
            .AsParallel()
            .Where(call => volunteer.MaxDistanceToCall is null || volunteer.FullCurrentAddress is null || VolunteerManager.CalculateDistanceFromVolunteerToCall(volunteer.FullCurrentAddress,call.FullAddressCall,volunteer.RangeType) <= volunteer.MaxDistanceToCall)
            .Select(call => new BO.OpenCallInList
            {
                CallId = call.Id,  // ID of the call
                CallFullAddress = call.FullAddressCall,  // Full address of the call
                Description = call.Description,  // Description of the call
                LastTimeForClosingTheCall = call.DeadLine,  // Deadline for closing the call
                TypeOfCall = (BO.CallType)call.Type,  // Type of the call
                OpenningTime = call.OpeningTime,  // Opening time of the call
                DistanceFromVolunteer = volunteer.FullCurrentAddress is null
                    ? -1  // If the volunteer's address is unknown, set distance to -1
                    : VolunteerManager.CalculateDistanceFromVolunteerToCall(volunteer.FullCurrentAddress, call.FullAddressCall, volunteer.RangeType)
            }).ToList();

        // If a call type filter is provided, filter the list by the specified type.
        if (callType != null)
        {
            openCalls = openCalls.Where(call => call.TypeOfCall == callType).ToList();
        }

        // If no sorting field is provided, sort the calls by their ID.
        if (sortingField == null)
        {
            return openCalls.OrderBy(call => call.CallId);
        }

        // Sort the list based on the specified sorting field.
        switch (sortingField)
        {
            case BO.OpenCallFields.CallId:
                openCalls = openCalls.OrderBy(call => call.CallId).ToList();
                break;
            case BO.OpenCallFields.TypeOfCall:
                openCalls = openCalls.OrderBy(call => call.TypeOfCall).ToList();
                break;
            case BO.OpenCallFields.Description:
                openCalls = openCalls.OrderBy(call => call.Description).ToList();
                break;
            case BO.OpenCallFields.CallFullAddress:
                openCalls = openCalls.OrderBy(call => call.CallFullAddress).ToList();
                break;
            case BO.OpenCallFields.OpenningTime:
                openCalls = openCalls.OrderBy(call => call.OpenningTime).ToList();
                break;
            case BO.OpenCallFields.LastTimeForClosingTheCall:
                openCalls = openCalls.OrderBy(call => call.LastTimeForClosingTheCall).ToList();
                break;
            case BO.OpenCallFields.DistanceFromVolunteer:
                openCalls = openCalls.OrderBy(call => call.DistanceFromVolunteer).ToList();
                break;
            default:
                // Throw an exception if the sorting field is invalid.
                throw new BO.BlInvalidOperationException("BL: Invalid sorting filterField");
        }

        // Return the sorted list of open calls.
        return openCalls;
    }

    /// <summary>
    /// This method returns an array that contains the number of calls for eaach status, each status on its index value in the cell
    /// </summary>
    /// <returns>Array containing the call count per status</returns>
    public int[] GetTotalCallsByStatus()
    {
        var groupedCallAndStatuses = s_dal.Call
                .ReadAll()
                .GroupBy(
                    call => CallManager.GetStatus(call.Id),
                    (key, group) => new { Status = key, Count = group.ToList().Count }
                    );

        int maxStatusValue = Enum.GetValues(typeof(BO.CallStatus)).Cast<int>().Max();
        int[] statusCount = new int[maxStatusValue + 1];

        groupedCallAndStatuses.ToList().ForEach(item => statusCount[(int)item.Status] = item.Count);
        return statusCount;
    }

    /// <summary>
    /// This methods assignes a call to a volunteer if the call is free to be alocated
    /// </summary>
    /// <param name="VolunteerId">The volunteer which requests a new call</param>
    /// <param name="callId">The requested call to be assigned</param>
    /// <exception cref="BO.BlDoesNotExistException">Thrown when there is not such a call</exception>
    /// <exception cref="BO.BlForbidenSystemActionExeption">Thrown when the action is forbidden due to the call being taken or finished</exception>
    public void SelectCallToDo(int VolunteerId, int callId)
    {
        //Check if there is such call
        DO.Call call = s_dal.Call.Read((call) => call.Id == callId)
            ?? throw new BO.BlDoesNotExistException($"Bl: Call with Id: {callId} doesn't exists");

        //Check if the call hasn't been taken by someone else
        DO.Assignment? callAssignment = s_dal.Assignment.Read((assignment) => assignment.CallId == callId);

        //Check if call already been taken
        if (callAssignment != null)
            throw new BO.BlForbidenSystemActionExeption($"Bl: Call {callId} already taken by other volunteer (Id: {callAssignment.VolunteerId})");

        //Check if there is time to complete the call
        if (call.DeadLine - AdminManager.Now <= TimeSpan.Zero)
            throw new BO.BlForbidenSystemActionExeption($"Bl: Call {callId} already expired");

        //Create DO Assignment entity with current clock time, and starting time and the CallType shall be null (?)
        DO.Assignment newAssignment = new DO.Assignment
        {
            Id = -1, //Temp id, the real id is assigned in the Dal layer
            CallId = callId,
            VolunteerId = VolunteerId,
            TimeOfEnding = null,
            TimeOfStarting = AdminManager.Now,
            TypeOfEnding = null,
        };
        s_dal.Assignment.Create(newAssignment);
        CallManager.Observers.NotifyListUpdated();
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
            //Lock has been added so the update method wont be done simonteniassly
            //which then would lead to the TakeLast method to take the wrong new object from the end of the enumarable
            lock (lockObject)
            {
                s_dal.Call.Update(newCall);

                //Notifies all observers that a call has been modified
                CallManager.Observers.NotifyItemUpdated(s_dal.Call.ReadAll().TakeLast(1).First().Id);
                CallManager.Observers.NotifyListUpdated();
            }
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
    public void CancelAssignement(int VolunteerId, int callId)
    {
        //Check access (if the user which wants to change the call status is the same uesr which assigned to that call)
        DO.Assignment assignment = s_dal.Assignment.Read((assignment) => assignment.CallId == callId)
            ?? throw new BO.BlDoesNotExistException($"BL: Call (Id: {callId}) for Volunteer (Id: {VolunteerId}) doesn't exists");

        if (assignment.VolunteerId != VolunteerId && (BO.UserRole)s_dal.Volunteer.Read(VolunteerId)!.Role! != BO.UserRole.Admin)
            throw new BO.BlForbidenSystemActionExeption($"BL: The volunteer (Id: {VolunteerId}) is not allowed to modify Call assinged to different volunteer (Id: {assignment.CallId})");

        //Check that the call is not ended (Cancled, Expiered or completed)
        if (assignment.TypeOfEnding != null || assignment.TypeOfEnding == DO.TypeOfEnding.Treated)
            throw new BO.BlForbidenSystemActionExeption($"BL: Unable to modify the call. Alrady ended with status of: {assignment.TypeOfEnding}, by volunteer Id: {assignment.VolunteerId})");

        //Throw exception if access not granted or if there is Dal exception

        //Update the Dal entity with current system time and Closed status
        DO.Assignment newAssignment = assignment with
        {
            TypeOfEnding = (assignment.VolunteerId != VolunteerId) ? DO.TypeOfEnding.AdminCanceled : DO.TypeOfEnding.SelfCanceled,
            TimeOfEnding = AdminManager.Now,
        };

        try
        {
            s_dal.Assignment.Update(newAssignment);
            //Notifies all observers that a call has been added
            CallManager.Observers.NotifyListUpdated();
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"BL: Assignment with Id: {newAssignment.Id} doesn't exists", ex);
        }

    }
    public void AddCallSendEmail(BO.Call c)
    {
        // Retrieve all volunteers and filter only the active ones
        var listVolunteer = s_dal.Volunteer.ReadAll()
            .Where(volunteer => volunteer.IsActive)
            .ToList();

        // Subject and body of the email
        string subject = "A new call has opened near your location";
        string body = $@"
             Hello,
             <br>
             A new call has been opened near your location:
             <br>
             <br>
             For more details, log into the system to view the call details.
             <br><br>
             Best regards,<br>
             The System Team
             Meir@Yedidia
             ";

        // Iterate over all volunteers
        foreach (var volunteer in listVolunteer)
        {
            // Calculate the distance between the call address and the volunteer's address
            double distance = VolunteerManager.CalculateDistanceFromVolunteerToCall(
                c.CallAddress,
                volunteer.FullCurrentAddress!,
                (DO.TypeOfRange)volunteer.RangeType);

            // Check if the distance is less than or equal to the maximum distance the volunteer is willing to cover
            if (distance <= volunteer.MaxDistanceToCall)
            {
                // Send an email to the volunteer
                Tools.SendEmail(volunteer.Email, subject, body);
            }
        }
    }

    public void CancleCallSendEmail(BO.CallInList c)
    {
        string subject = "Your call assignment has been canceled";
        string body = "Hello, your call assignment has been canceled by the manager";

        var listAss = s_dal.Assignment.ReadAll()
            .Where(a => a.CallId == c.CallId)
            .ToList();
        var assignment = listAss.FirstOrDefault();

        // Assumption: s_dal.Volunteer.ReadAll() returns a collection of volunteers
        var matchingVolunteers = s_dal.Volunteer.ReadAll()
            .Where(v => v.Id == assignment!.VolunteerId) // Filter by ID
            .ToList();

        var volunteer = matchingVolunteers.FirstOrDefault();
        Tools.SendEmail(volunteer!.Email, subject, body);
    }

    public IEnumerable<(double, double)> GetListOfOpenCallsForVolunteerCordinates(int volunteerId)
    {
        return from call in GetOpenCallsForVolunteer(volunteerId,null,null)
               let currentCall = s_dal.Call.Read(call.CallId)
               select (currentCall.Latitude, currentCall.Longitude);
    }

    public IEnumerable<(double,double)> ConvertClosedCallsIntoCordinates(IEnumerable<ClosedCallInList>listOfCalls)
        =>  from closedCall in listOfCalls
            let call = s_dal.Call.Read(closedCall.Id)
            select (call.Latitude, call.Longitude);

    public IEnumerable<(double, double)> ConvertOpenCallsToCordinates(IEnumerable<OpenCallInList> listOfCalls)
        =>     from openCall in listOfCalls
               let call = s_dal.Call.Read(openCall.CallId)
               select (call.Latitude, call.Longitude);
}
