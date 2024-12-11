
namespace BlImplementation;
using BlApi;

using Helpers;
using System.Collections.Generic;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal s_dal = DalApi.Factory.Get;
    public void AddCall(BO.Call call)
    {
        try
        {

            // check if the times are valid
            if (call.CallStartTime > call.CallDeadLine || call.CallDeadLine < ClockManager.Now)
            {
                throw new BO.BoInvalidEntityDetails("The deadline of the call cannot be before the start time of the call");
            }
            //check if the address is valid
            //TODO:: using api.
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
        catch (BO.BoInvalidEntityDetails ex)
        {
            // Handle the exception here
            Console.WriteLine("An exception occurred: " + ex.Message);
            // You can also log the exception or perform any other necessary actions
            throw; // Rethrow the exception to propagate it further if needed
        }

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
            throw new BO.BoForbidenActionExeption("BL: Cant update the assignment");
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

    public void UpdateCallEnd(int VolunteerId, int callId)
    {
        throw new NotImplementedException();
    }
}
