
using Helpers;

namespace BO;

public class OpenCallInList
{
    // callId - unique identifier for the call // taken from Do.call entity
    public int CallId { get; init; }

    // TypeOfCall - the type of the call // taken from Do.call entity
    public CallType TypeOfCall { get; set; }

    // Description - description of the call // taken from Do.call entity
    public string? Description { get; set; }

    // callFullAddress - the full address of the call // taken from Do.call entity
    public string CallFullAddress { get; set; }

    // OpenningTime - the time when the call was opened // taken from Do.call entity
    public DateTime OpenningTime { get; set; }

    // LastTimeForClosingTheCall - the last time for closing the call // taken from Do.call entity
    public DateTime? LastTimeForClosingTheCall { get; set; }

    // DistanceFromVolunteer - the distance from the volunteer to the call // taken from Do.call entity
    public double DistanceFromVolunteer { get; set; }
    
    public override string ToString() => this.ToStringProperty();
}
