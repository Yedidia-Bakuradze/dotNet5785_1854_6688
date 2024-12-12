
namespace BlImplementation;
using BlApi;

using Helpers;
using System.Collections.Generic;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal s_dal = DalApi.Factory.Get;
    public void AddCall(BO.Call call)
    {

        //Check if the times are valid
        if (call.CallStartTime > call.CallDeadLine || call.CallDeadLine < ClockManager.Now)
        {
            throw new BO.BoInvalidEntityDetails("The deadline of the call cannot be before the start time of the call");
        }

        //Checks if the address is valid (if cordinates exist)
        (double? lat, double? lng) = VolunteerManager.GetGeoCordinates(call.CallAddress);
        if (lat == null || lng == null)
            throw new BO.BoInvalidEntityDetails($"BL: The given call address ({call.CallAddress}) is not a real address");
        
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
                throw new BO.BoAlreadyExistsException("BL Exception:", new DO.DalAlreadyExistsException("DAL Exception:"));
            }
            // Attempt to delete the callnew 
            s_dal.Call.Delete(requestId);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BoDoesNotExistException("Bl Exception: id does not exist", ex) ; 
        }
    }

    public void EndOfCallStatusUpdate(int VolunteerId, int callId)
    {

        DO.Assignment? res = s_dal.Assignment.Read(ass => ass.Id == callId && ass.VolunteerId == VolunteerId) ?? throw new BO.BoDoesNotExistException("BL : Assignement does not exist", new DO.DalDoesNotExistException(""));
        if (res?.TypeOfEnding != null || res?.TimeOfEnding != null)
        {
            throw new BO.BoForbidenSystemActionExeption("BL: Cant update the assignment");
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
            throw new BO.BoDoesNotExistException("Bl: ASsignement does not exist", ex);
        }
    }

    public IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int id, BO.CallType? callType, BO.ClosedCallInListFields? parameter)
    {
        
        throw new NotImplementedException();
    }

    public BO.Call GetDetielsOfCall(int callId)
    {
        throw new NotImplementedException();
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
    /// <exception cref="BO.BoDoesNotExistException">Thrown when the assignment doesn't exists</exception>
    /// <exception cref="BO.BoForbidenSystemActionExeption">Thrown when the opration is forbidden due to restriction and access level of the volunteer</exception>
    public void UpdateCallEnd(int VolunteerId, int callId)
    {
        //Check access (if the user which wants to change the call status is the same uesr which assigned to that call)
        DO.Assignment assignment = s_dal.Assignment.Read((assignment) => assignment.CallId == callId)
            ?? throw new BO.BoDoesNotExistException($"BL: Call (Id: {callId}) for Volunteer (Id: {VolunteerId}) doesn't exists");

        if (assignment.CallId != VolunteerId)
            throw new BO.BoForbidenSystemActionExeption($"BL: The volunteer (Id: {VolunteerId}) is not allowed to modify Call assinged to different volunteer (Id: {assignment.CallId})");

        //Check that the call is not ended (Cancled, Expiered or completed)
        if (assignment.TypeOfEnding != null || assignment.TypeOfEnding == DO.TypeOfEnding.Treated)
            throw new BO.BoForbidenSystemActionExeption($"BL: Unable to modify the call. Alrady ended with status of: {assignment.TypeOfEnding}, by volunteer Id: {assignment.VolunteerId})");

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
            throw new BO.BoDoesNotExistException($"BL: Assignment with Id: {newAssignment.Id} doesn't exists", ex);
        }

    }
}
