
using Helpers;

namespace BO;

public class CallInList
{
    // The unique identifier of the call in the list
    public int? Id { get; init; }

    // The identifier of the call
    public int CallId { get; init; }

    // The type of the call
    public CallType TypeOfCall { get; set; }

    // The opening time of the call
    public DateTime OpenningTime { get; set; }

    // The time remaining until the call ends
    public TimeSpan? TimeToEnd { get; set; }

    // The name of the last volunteer assigned to the call
    public string? LastVolunteerName { get; set; }

    // The elapsed time since the call was opened
    public TimeSpan? TimeElapsed { get; set; }

    // The status of the call
    public CallStatus Status { get; set; }

    // The total number of allocations made for the call
    public int TotalAlocations { get; set; }

    // Returns a string representation of the CallInList object
    public override string ToString() => this.ToStringProperty();
}
