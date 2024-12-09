
namespace BO;

public class VolunteerInList
{
    // Unique identifier for the volunteer // Taken from DO.Volunteer entity
    public int id { get; init; }

    // Full name of the volunteer  // Taken from DO.Volunteer entity
    public string fullName { get; set; }

    // Indicates whether the volunteer is active or not // Taken from DO.Volunteer entity
    public bool isActive { get; set; }

    // Total number of calls done by the volunteer
    public int totalCallsDoneByVolunteer { get; set; }

    // Total number of calls cancelled by the volunteer
    public int totalCallsCancelByVolunteer { get; set; }

    // Total number of calls expired by the volunteer
    public int totalCallsExpiredByVolunteer { get; set; }

    // Identifier of the current call assigned to the volunteer
    public int? callId { get; set; }

    // Type of the current call assigned to the volunteer // Taken from DO.Call entity
    public CallType callType { get; set; }
    public override string ToString() => this.ToStringProperty();



}
