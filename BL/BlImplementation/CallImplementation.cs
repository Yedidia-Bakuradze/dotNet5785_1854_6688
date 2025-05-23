﻿namespace BlImplementation;
using BlApi;
using BO;
using Helpers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

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

    #region CRUD

    /// <summary>
    /// Adds a new call to the system after validating its details.
    /// </summary>
    /// <param name="call">The call object containing details to be added.</param>
    /// <exception cref="BO.BlInvalidEntityDetails">Thrown if the call's times are invalid or the address is not a real location.</exception>
    public void AddCall(BO.Call call)
    {
        AdminManager.ThrowOnSimulatorIsRunning();

        CallManager.IsCallValid(call);

        //Create new Dal entity
        DO.Call? newCall = CallManager.ConvertFromBdToD(call);

        // Add the new call to the database
        lock (AdminManager.BlMutex)
        {
            s_dal.Call.Create(newCall);
            newCall = s_dal.Call.ReadAll().LastOrDefault() 
                ?? throw new BlUnWantedNullValueException($"Bl Says: The new created call hasn't been found");
        }

        //Updates the cordinates later
        CallManager.UpdateCallCordinates(newCall.Id, newCall.FullAddressCall, true);
        
        //Sends the email notification
        AddCallSendEmailAsync(newCall);
        
        //Notifies all observers that a call has been added
        CallManager.Observers.NotifyListUpdated();
        CallManager.Observers.NotifyItemUpdated(newCall.Id);
    }

    /// <summary>
    /// Deletes a call request by its request ID.
    /// </summary>
    /// <param name="callId">The ID of the call request to delete.</param>
    /// <exception cref="BO.BlAlreadyExistsException">Thrown if there are assignments related to the call that prevent deletion.</exception>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the call with the given ID does not exist in the database.</exception>
    public void DeleteCallRequest(int callId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();

        //Throw an exception if call is not allowed to be deleted
        CallManager.VerifyCallDeletionAttempt(callId);
        
        try
        {
            // Attempt to delete the call
            lock (AdminManager.BlMutex)
            { 
                s_dal.Call.Delete(callId);
            }
            
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
    /// This method updates the DO Call with the past BO Call field values
    /// </summary>
    /// <param name="call">The new (BO) Call entity with the new values</param>
    /// <exception cref="BO.BlInvalidEntityDetails">Thrown if the Call values are not valid</exception>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if such call doesn't exist</exception>
    public void UpdateCall(BO.Call call)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        DO.Call updatedCall;

        //Throws an exception if the values are invalid
        CallManager.IsCallValid(call);

        //Convert values to DO entity
        DO.Call newCall = CallManager.ConvertFromBdToD(call);

        
        try
        {
            lock (AdminManager.BlMutex)
            {
                s_dal.Call.Update(newCall);
                updatedCall = s_dal.Call.ReadAll().LastOrDefault()
                    ?? throw new BlUnWantedNullValueException($"Bl Says: Can't read old call {call.Id} after modification");
            }

        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"BL: Call with Id: {newCall.Id} doesn't exists");
        }

        CallManager.Observers.NotifyItemUpdated(updatedCall.Id);
        CallManager.Observers.NotifyListUpdated();

        //Start calculating the new cordinates
        CallManager.UpdateCallCordinates(updatedCall.Id,call.CallAddress,false);

        //TODO: Should call the email system again?
    }

    /// <summary>
    /// Retrieves the details of a call based on the provided call ID.
    /// </summary>
    public BO.Call GetDetielsOfCall(int callId)
    {
        List<DO.Assignment> res;
        DO.Call call;
        lock (AdminManager.BlMutex)
        {
            // Fetch the call details from the data layer (DAL) by the given call ID.
            call = s_dal.Call.Read(call => call.Id == callId)
                 ?? throw new BlDoesNotExistException($"BL: Call with id {callId} doesn't exists");

            // Fetch all assignments related to the call ID.
            res = s_dal.Assignment.ReadAll(ass => ass.CallId == callId).ToList();
        }
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
   
    #region Get Calls
    /// <summary>
    /// Retrieves a list of closed calls assigned to a specific volunteer, with optional filtering and sorting.
    /// </summary>
    /// <param name="VolunteerId">The ID of the volunteer.</param>
    /// <param name="callType">Optional filter for the type of call.</param>
    /// <param name="parameter">Optional filterField for sorting the resulting list.</param>
    /// <returns>A sorted and filtered list of closed calls.</returns>
    public IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int VolunteerId, BO.CallType? callType, BO.ClosedCallInListFields? parameter)
    {
        IEnumerable<DO.Assignment> assignments;
        IEnumerable<DO.Call> closedCalls;

        // Step 1: Fetch all assignments for the given volunteer
        lock (AdminManager.BlMutex)
        {
            assignments = s_dal.Assignment.ReadAll(ass => ass.VolunteerId == VolunteerId && ass.TypeOfEnding is not null);
            closedCalls = s_dal.Call.ReadAll(call => assignments.Any(ass => ass.CallId == call.Id && ass.TypeOfEnding != null));
        }

        closedCalls = callType is not null
            ? closedCalls.Where(call => call.Type == (DO.CallType)callType)
            : closedCalls;

        // Step 4: Create a list of ClosedCallInList objects
        IEnumerable<BO.ClosedCallInList> result = from assign in assignments
                                               let call = closedCalls.Where(call => call.Id == assign.CallId).FirstOrDefault()
                                               where call is not null
                                               select new BO.ClosedCallInList
                                               {
                                                   Id = call.Id,
                                                   TypeOfCall = (BO.CallType)call.Type,
                                                   CallAddress = call.FullAddressCall,
                                                   CallStartTime = call.OpeningTime,
                                                   EnteryTime = assign.TimeOfStarting, // Access the start time from the corresponding assignment
                                                   ClosingTime = (DateTime)assign.TimeOfEnding!, // Access the ending time from the corresponding assignment
                                                   TypeOfClosedCall = (BO.TypeOfEndingCall)assign.TypeOfEnding! // Default to Unknown if TypeOfEnding is null
                                               };

        // Step 5: Sort the list based on the specified filterField
        if (parameter != null)
        {
            result = parameter switch
            {
                BO.ClosedCallInListFields.Id => result.OrderBy(call => call.Id),
                BO.ClosedCallInListFields.TypeOfCall => result.OrderBy(call => call.TypeOfCall),
                BO.ClosedCallInListFields.CallAddress => result.OrderBy(call => call.CallAddress),
                BO.ClosedCallInListFields.CallStartTime => result.OrderBy(call => call.CallStartTime),
                BO.ClosedCallInListFields.EnteryTime => result.OrderBy(call => call.EnteryTime),
                BO.ClosedCallInListFields.ClosingTime => result.OrderBy(call => call.ClosingTime),
                BO.ClosedCallInListFields.TypeOfClosedCall => result.OrderBy(call => call.TypeOfClosedCall),
                _ => throw new BO.BlInvalidOperationException("Invalid sorting filterField"),
            };
        }

        // Step 6: Return the final list
        return result;
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
        List<DO.Assignment> assignments;
        List<DO.Call> calls;
        lock (AdminManager.BlMutex)
        {
            // Retrieve all calls and assignments from the database
            calls = s_dal.Call.ReadAll().ToList();
            assignments = s_dal.Assignment.ReadAll().ToList();
        }
        IEnumerable<BO.CallInList> callsInlist;
        lock (AdminManager.BlMutex)
        {
            // Build the initial list of CallInList objects based on the calls and their assignments
            callsInlist = source is not null
                                             ? source
                                             :
                                             (from call in calls
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
                                             }).ToList();
        }

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
        DO.Volunteer? volunteer;
        IEnumerable<BO.OpenCallInList> openCalls;
        lock (AdminManager.BlMutex)
        {
            // Retrieve the volunteer details. Throw an exception if the volunteer does not exist.
            volunteer = s_dal.Volunteer.Read(vol => vol.Id == VolunteerId);
        }

        if (volunteer is null)
            throw new BO.BlDoesNotExistException($"Bl: Volunteer (Id: {VolunteerId} doesn't exist)");

        // Retrieve all open or risky calls and map them to BO.OpenCallInList objects.
        openCalls = from call in s_dal.Call.ReadAll()
                    where
                    (CallManager.GetStatus(call.Id) == BO.CallStatus.Open
                    || CallManager.GetStatus(call.Id) == BO.CallStatus.OpenAndRisky
                    )
                    && (
                        volunteer.MaxDistanceToCall is null
                        || volunteer.FullCurrentAddress is null
                        || VolunteerManager.CalculateDistanceFromVolunteerToCall((volunteer.Latitude, volunteer.Longitude), (call.Latitude, call.Longitude), volunteer.RangeType) <= volunteer.MaxDistanceToCall
                    )
                    select new BO.OpenCallInList
                    
                    {
                        CallId = call.Id,  // ID of the call
                        CallFullAddress = call.FullAddressCall,  // Full address of the call
                        Description = call.Description,  // Description of the call
                        LastTimeForClosingTheCall = call.DeadLine,  // Deadline for closing the call
                        TypeOfCall = (BO.CallType)call.Type,  // Type of the call
                        OpenningTime = call.OpeningTime,  // Opening time of the call
                        DistanceFromVolunteer = volunteer.FullCurrentAddress is null
                             ? -1  // If the volunteer's address is unknown, set distance to -1
                             : VolunteerManager.CalculateDistanceFromVolunteerToCall((volunteer.Latitude, volunteer.Longitude), (call.Latitude, call.Longitude), volunteer.RangeType)
                    };
        
        // If a call type filter is provided, filter the list by the specified type.
        if (callType != null)
            openCalls = openCalls.Where(call => call.TypeOfCall == callType);

        // If no sorting field is provided, sort the calls by their ID.
        if (sortingField == null)
            return openCalls.OrderBy(call => call.CallId);

        // Sort the list based on the specified sorting field.
        openCalls = sortingField switch
        {
            BO.OpenCallFields.CallId => openCalls.OrderBy(call => call.CallId),
            BO.OpenCallFields.TypeOfCall => openCalls.OrderBy(call => call.TypeOfCall),
            BO.OpenCallFields.Description => openCalls.OrderBy(call => call.Description),
            BO.OpenCallFields.CallFullAddress => openCalls.OrderBy(call => call.CallFullAddress),
            BO.OpenCallFields.OpenningTime => openCalls.OrderBy(call => call.OpenningTime),
            BO.OpenCallFields.LastTimeForClosingTheCall => openCalls.OrderBy(call => call.LastTimeForClosingTheCall),
            BO.OpenCallFields.DistanceFromVolunteer => openCalls.OrderBy(call => call.DistanceFromVolunteer),
            _ => throw new BO.BlInvalidOperationException("BL: Invalid sorting filterField"),// Throw an exception if the sorting field is invalid.
        };

        // Return the sorted list of open calls.
        return openCalls;
    }

    /// <summary>
    /// This method returns an array that contains the number of calls for eaach status, each status on its index value in the cell
    /// </summary>
    /// <returns>Array containing the call count per status</returns>
    public int[] GetTotalCallsByStatus()
    {
        lock (AdminManager.BlMutex)
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
    }

    #endregion
    
    #endregion

    #region Assignment Create & Close

    /// <summary>
    /// Updates the status of an assignment when the call ends.
    /// </summary>
    /// <param name="VolunteerId">The ID of the volunteer associated with the assignment.</param>
    /// <param name="assignmentId">The ID of the call associated with the assignment.</param>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the assignment does not exist.</exception>
    /// <exception cref="BO.BlForbidenSystemActionExeption">Thrown if the assignment has already been ended or is not allowed to be updated.</exception>
    public void FinishAssignement(int VolunteerId, int assignmentId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();

        //Throws an exception if the assignment is unfinishble
        CallManager.VerifyAssignmentFinishAttept(VolunteerId,assignmentId, out DO.Assignment res);
        
        try
        {
            // Update the assignment with the new ending type and time
            lock (AdminManager.BlMutex)
            {
                s_dal.Assignment.Update(res with
                {
                    TypeOfEnding = DO.TypeOfEnding.Treated,
                    TimeOfEnding = AdminManager.Now
                });
            }

            CallManager.Observers.NotifyListUpdated();
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException("Bl: Assignment does not exist", ex);
        }
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
        AdminManager.ThrowOnSimulatorIsRunning();

        //Throws an exception if the call-volunteer allocation is not allowed
        CallManager.VerifyAssignmentAllocationAttempt(VolunteerId,callId);

        //Create DO Assignment entity with current clock time, and starting time and the CallType shall be null (?)
        lock (AdminManager.BlMutex)
        {
            s_dal.Assignment.Create(new DO.Assignment
            {
                Id = -1, //Temp id, the real id is assigned in the Dal layer
                CallId = callId,
                VolunteerId = VolunteerId,
                TimeOfEnding = null,
                TimeOfStarting = AdminManager.Now,
                TypeOfEnding = null,
            });
        }
        CallManager.Observers.NotifyListUpdated();
    }

    /// <summary>
    /// This method updates the Call status with the past callId
    /// The operation is allowed only if the call is opened and the volunteer which requests this modification is the assigned volunteer to that call
    /// </summary>
    /// <param name="VolunteerId">The volunteer which requests the modification</param>
    /// <param name="assignmentId">The call if of the Call which is needed to be updated</param>
    /// <exception cref="BO.BlDoesNotExistException">Thrown when the assignment doesn't exists</exception>
    /// <exception cref="BO.BlForbidenSystemActionExeption">Thrown when the opration is forbidden due to restriction and access level of the volunteer</exception>
    public void CancelAssignement(int VolunteerId, int assignmentId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();

        //Throws an exception if the assignment canceltion is not allowed
        CallManager.VerifyAssignmentCancelAttempt(VolunteerId, assignmentId,out DO.Assignment assignment);

        try
        {
            //Update the Dal entity with current system time and Closed status
            lock (AdminManager.BlMutex)
            {
                s_dal.Assignment.Update(assignment with
                {
                    TypeOfEnding = (assignment.VolunteerId != VolunteerId) ? DO.TypeOfEnding.AdminCanceled : DO.TypeOfEnding.SelfCanceled,
                    TimeOfEnding = AdminManager.Now,
                });
            }
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Bl Says: {ex.Message}");
        }

        //Notifies all observers that a call has been added
        CallManager.Observers.NotifyListUpdated();
        CallManager.Observers.NotifyItemUpdated(assignmentId);
    }

    #endregion

    #region Email System
    //Sync email system
    //public void AddCallSendEmail(BO.Call c)
    //{
    //    List<DO.Volunteer> activeVolunteers;

    //    lock (AdminManager.BlMutex)
    //    {
    //        // Retrieve all volunteers and filter only the active ones
    //        activeVolunteers = s_dal.Volunteer.ReadAll()
    //            .Where(volunteer => volunteer.IsActive && !string.IsNullOrWhiteSpace(volunteer.Email))
    //            .ToList();
    //    }

    //    // Subject of the email
    //    string subject = "A new call has opened near your location";

    //    // Iterate over all volunteers
    //    int emailsSent = 0; // Counter for successful emails
    //    foreach (var volunteer in activeVolunteers)
    //    {
    //        try
    //        {
    //            // Calculate the distance between the call address and the volunteer's address
    //            double distance = VolunteerManager.CalculateDistanceFromVolunteerToCall(
    //                c.CallAddress,
    //                volunteer.FullCurrentAddress!,
    //                (DO.TypeOfRange)volunteer.RangeType);

    //            // Check if the distance is within the volunteer's range
    //            if (distance <= volunteer.MaxDistanceToCall)
    //            {
    //                // Generate dynamic HTML body for the email
    //                string body = $@"
    //                <html>
    //                <body style='font-family: Arial, sans-serif;'>
    //                    <h3 style='color: #4CAF50;'>Hello {volunteer.FullName},</h3>
    //                    <p>A new call has been opened near your location:</p>
    //                    <ul style='line-height: 1.6;'>
    //                        <li><strong>Call ID:</strong> {c.Id}</li>
    //                        <li><strong>Type:</strong> {c.TypeOfCall}</li>
    //                        <li><strong>Description:</strong> {c.Description}</li>
    //                        <li><strong>Start Time:</strong> {c.CallStartTime}</li>
    //                        <li><strong>Deadline:</strong> {c.CallDeadLine?.ToString() ?? "N/A"}</li>
    //                        <li><strong>Address:</strong> {c.CallAddress}</li>
    //                    </ul>
    //                    <p>For more details, please log into the system to view the call details.</p>
    //                    <br>
    //                    <p>Best regards,<br>The System Team<br>Meir@Yedidia</p>
    //                </body>
    //                </html>";

    //                // Send email
    //                Tools.SendEmail(volunteer.Email, subject, body);
    //                emailsSent++; // Increment the counter
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            // Log the error for debugging purposes
    //            Console.WriteLine($"Failed to send email to {volunteer.Email}. Error: {ex.Message}");
    //        }
    //    }

    //    // Log summary
    //    Console.WriteLine($"Email notifications sent: {emailsSent}/{activeVolunteers.Count}");
    //}
    public async Task AddCallSendEmailAsync(DO.Call c)
    {
        List<DO.Volunteer> activeVolunteers;

        lock (AdminManager.BlMutex)
        {
            // Retrieve all volunteers and filter only the active ones
            activeVolunteers = s_dal.Volunteer.ReadAll()
                .Where(volunteer => volunteer.IsActive && !string.IsNullOrWhiteSpace(volunteer.Email))
                .ToList();
        }

        // Subject of the email
        string subject = "A new call has opened near your location";

        // Iterate over all volunteers
        int emailsSent = 0; // Counter for successful emails
        foreach (var volunteer in activeVolunteers)
        {
            try
            {
                // Calculate the distance between the call address and the volunteer's address
                double distance = VolunteerManager.CalculateDistanceFromVolunteerToCall(
                    (volunteer.Latitude,volunteer.Longitude),
                    (c.Latitude, c.Longitude),
                    (DO.TypeOfRange)volunteer.RangeType);

                // Check if the distance is within the volunteer's range
                if (distance <= volunteer.MaxDistanceToCall)
                {
                    // Generate dynamic HTML body for the email
                    string body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h3 style='color: #4CAF50;'>Hello {volunteer.FullName},</h3>
                    <p>A new call has been opened near your location:</p>
                    <ul style='line-height: 1.6;'>
                        <li><strong>Call ID:</strong> {c.Id}</li>
                        <li><strong>Type:</strong> {c.Type}</li>
                        <li><strong>Description:</strong> {c.Description}</li>
                        <li><strong>Start Time:</strong> {c.OpeningTime}</li>
                        <li><strong>Deadline:</strong> {c.DeadLine?.ToString() ?? "N/A"}</li>
                        <li><strong>Address:</strong> {c.FullAddressCall}</li>
                    </ul>
                    <p>For more details, please log into the system to view the call details.</p>
                    <br>
                    <p>Best regards,<br>The System Team<br>Meir@Yedidia</p>
                </body>
                </html>";

                    // Send email
                    await Tools.SendEmail(volunteer.Email, subject, body);
                    emailsSent++; // Increment the counter
                }
            }
            catch (Exception ex)
            {
                // Log the error for debugging purposes
                Console.WriteLine($"Failed to send email to {volunteer.Email}. Error: {ex.Message}");
            }
        }

        // Log summary
        Console.WriteLine($"Email notifications sent: {emailsSent}/{activeVolunteers.Count}");
    }

    //public void CancleCallSendEmail(BO.CallInList c)
    //{
    //    List<DO.Assignment> listAss;
    //    List<DO.Volunteer> matchingVolunteers;
    //    string subject = "Your call assignment has been canceled";
    //    string body = "Hello, your call assignment has been canceled by the manager";

    //    lock (AdminManager.BlMutex)
    //    {
    //        listAss = s_dal.Assignment.ReadAll()
    //            .Where(a => a.CallId == c.CallId)
    //            .ToList();
    //    }
    //    DO.Assignment? assignment = listAss.FirstOrDefault();

    //    // Assumption: s_dal.Volunteer.ReadAll() returns a collection of volunteers
    //    lock (AdminManager.BlMutex)
    //    {
    //            matchingVolunteers = s_dal.Volunteer.ReadAll()
    //            .Where(v => v.Id == assignment!.VolunteerId) // Filter by ID
    //            .ToList();
    //    }

    //    DO.Volunteer? volunteer = matchingVolunteers.FirstOrDefault();
    //    Tools.SendEmail(volunteer!.Email, subject, body);
    //}
    public async Task CancleCallSendEmailAsync(BO.CallInList c)
    {
        List<DO.Assignment> listAss;
        List<DO.Volunteer> matchingVolunteers;
        string subject = "Your call assignment has been canceled";
        
        lock (AdminManager.BlMutex)
        {
            listAss = s_dal.Assignment.ReadAll()
                .Where(a => a.CallId == c.CallId)
                .ToList();
        }
        DO.Assignment? assignment = listAss.FirstOrDefault();

        // Assumption: s_dal.Volunteer.ReadAll() returns a collection of volunteers
        lock (AdminManager.BlMutex)
        {
            matchingVolunteers = s_dal.Volunteer.ReadAll()
                .Where(v => v.Id == assignment!.VolunteerId) // Filter by ID
                .ToList();
            matchingVolunteers = s_dal.Volunteer.ReadAll()
            .Where(v => v.Id == assignment!.VolunteerId) // Filter by ID
            .ToList();
        }

        DO.Volunteer? volunteer = matchingVolunteers.FirstOrDefault();
        string body = $@"
    <html>
    <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
        <div style='background-color: #ffffff; padding: 20px; border-radius: 8px; max-width: 600px; margin: auto; box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);'>
            <h3 style='color: #4CAF50;'>Hello {volunteer.FullName},</h3>
            <p>We regret to inform you that the assignment you were associated with has been canceled. Below are the details of the canceled assignment:</p>
            <ul style='line-height: 1.6; list-style-type: none; padding: 0;'>
                <li><strong>Call ID:</strong> {c.Id}</li>
                <li><strong>Type:</strong> {c.TypeOfCall}</li>
                <li><strong>Start Time:</strong> {c.OpenningTime}</li>
                <li><strong>Deadline:</strong> {c.TimeToEnd?.ToString() ?? "N/A"}</li>
                <li><strong>TotalAlocations:</strong> {c.TotalAlocations}</li>
            </ul>
            <p>If you have any questions or need further assistance, please feel free to contact us.</p>
            <br>
            <p>Best regards,<br>The System Team<br>Meir@Yedidia</p>
        </div>
    </body>
    </html>";
        if (volunteer != null)
        {
            try
            {
                await Tools.SendEmail(volunteer.Email, subject, body);
                Console.WriteLine($"Cancellation email sent to {volunteer.Email}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send cancellation email to {volunteer.Email}. Error: {ex.Message}");
            }
        }
    }
    #endregion

    #region Convertors
    public IEnumerable<(double, double)> GetListOfOpenCallsForVolunteerCordinates(int volunteerId) => ConvertOpenCallsToCordinates(GetOpenCallsForVolunteer(volunteerId, null, null));
    public IEnumerable<(double, double)> ConvertClosedCallsIntoCordinates(IEnumerable<ClosedCallInList> listOfCalls)
    {
        lock (AdminManager.BlMutex)
        {
            return (from closedCall in listOfCalls
                    let call = s_dal.Call.Read(closedCall.Id)
                    where (call.Latitude, call.Longitude) is not (null, null)
                    select ((double)call.Latitude!, (double)call.Longitude!)).ToList();
        }
    }
    public IEnumerable<(double, double)> ConvertOpenCallsToCordinates(IEnumerable<OpenCallInList> listOfCalls)
    {
        lock (AdminManager.BlMutex)
        {
            return (from openCall in listOfCalls
                   let call = s_dal.Call.Read(openCall.CallId)
                   where (call.Latitude,call.Longitude) is not (null,null)
                    select ((double)call.Latitude!, (double)call.Longitude!));
        }
    }
    #endregion
}
