
using Helpers;

namespace BO;

public class CallInProgress
{

    /// <summary>
    /// From the DO.Assignment by the volunteer's id and the propery call status (Not completed)
    /// NFD - Not For Displayment
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// From Do.Assignment
    /// </summary>
    public int CallId { get; set; }

    /// <summary>
    /// From DO.Call
    /// </summary>
    public CallType TypeOfCall { get; set; }

    /// <summary>
    /// From DO.Call
    /// </summary>
    public string? Description{ get; set; }

    /// <summary>
    /// From DO.Call
    /// </summary>
    public string EmailAddress { get; set; }

    /// <summary>
    /// From DO.Call
    /// </summary>
    public DateTime OpenningTime { get; set; }

    /// <summary>
    /// From DO.Call
    /// </summary>
    public DateTime? LastTimeForClosingTheCall { get; set; }

    /// <summary>
    /// From DO.Assignemnt
    /// </summary>
    public DateTime EntryTime { get; set; }

    /// <summary>
    /// The distance would be calculated from the area which the volunteer took the call
    /// </summary>
    public double DistanceFromAssignedVolunteer { get; set; }

    /// <summary>
    /// Either: Taken, or TakenAndInRisk if the time that left is in the RiskRange area
    /// </summary>
    public CallStatus Status { get; set; }

    public override string ToString() => this.ToStringProperty();

}
