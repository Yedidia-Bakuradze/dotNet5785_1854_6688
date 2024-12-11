
namespace BlImplementation;
using BlApi;
using Helpers;
using System.Collections.Generic;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void AddCall(BO.Call call)
    {
        try
        {
            //check if the id is valid according to the format we chose in the CallManager
            if (!CallManager.IsCallIdValid(call.Id))
            {
                throw new ArgumentException("The ID of the call is invalid");
            }
            // check if the times are valid
            if (call.CallStartTime > call.CallDeadLine || call.CallDeadLine < ClockManager.Now)
            {
                throw new ArgumentException("The deadline of the call cannot be before the start time of the call");
            }
            //check if the address is valid
            //TODO:: using api.
            DO.Call newCall = new DO.Call
            {
                Id = call.Id,
                Type = (DO.CallType)call.TypeOfCall,
                FullAddressCall = call.CallAddress,
                Latitude = (double)call.Latitude,
                Longitude = (double)call.Longitude,
                OpeningTime = call.CallStartTime,
                Description = call.Description,
                DeadLine = call.CallDeadLine
            };
            _dal.Call.Create(newCall);
        }
        catch (Exception ex)
        {
            // Handle the exception here
            Console.WriteLine("An exception occurred: " + ex.Message);
            // You can also log the exception or perform any other necessary actions
            throw; // Rethrow the exception to propagate it further if needed
        }

    }

    public void DeleteCallRequest(int VolunteerId)
    {

        throw new NotImplementedException();
    }

    public void EndOfCallStatusUpdate(int VolunteerId, int callId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int id, BO.CallType? callType, BO.ClosedCallInListFields? parameter)
    {
        throw new NotImplementedException();
    }

    public BO.Call GetDetialsOfCall(int callId)
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
