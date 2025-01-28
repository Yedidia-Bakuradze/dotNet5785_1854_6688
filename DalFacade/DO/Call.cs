namespace DO;
/// <summary>
/// This record entity represent the call request that has created by the manager and given to the volunteers to handle it.
/// </summary>
/// <param name="Id">Identifier for each call. Created automatically - provide 0 argument to initialize the property</param>
/// <param name="Type">The type of opened call</param>
/// <param name="FullAddressCall">The address which the call is referring to</param>
/// <param name="Latitude">The latitude which is related to the address that is related to the call - updated by the logical layer anytime that the address is updated</param>
/// <param name="Longitude">The longitude which is related to the address that is related to the call - updated by the logical layer anytime that the address is updated</param>
/// <param name="OpeningTime">The time which the call has been opened by the manager</param>
/// <param name="Description">[Optional] A description about the call</param>
/// <param name="DeadLine">[Optional] Last time for closing the call.
/// Given to some of the calls and used by the logical layer in order to define and calculated Risk Time Range in order to determinate whether the call is riscky or not
/// </param>
public record Call
(
    int Id,
    CallType Type,
    string FullAddressCall,
    double? Latitude,
    double? Longitude,
    DateTime OpeningTime,
    string? Description = null,
    DateTime? DeadLine = null
)
{
    public Call() : this(0, CallType.Undefined, "", 0.0, 0.0, DateTime.Now) { }
};
