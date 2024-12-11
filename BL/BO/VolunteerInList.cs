
using Helpers;

namespace BO;

public class VolunteerInList
{
    // Unique identifier for the volunteer // Taken from DO.Volunteer entity
    public int Id { get; init; }

    // Full name of the volunteer  // Taken from DO.Volunteer entity
    public string FullName { get; set; }

    // Indicates whether the volunteer is active or not // Taken from DO.Volunteer entity
    public bool IsActive { get; set; }

    // Total number of calls done by the volunteer
    public int TotalCallsDoneByVolunteer { get; }

    // Total number of calls cancelled by the volunteer
    public int TotalCallsCancelByVolunteer { get;}

    // Total number of calls expired by the volunteer
    public int TotalCallsExpiredByVolunteer { get;}

    // Identifier of the current call assigned to the volunteer
    public int? CallId { get; set; }

    // Type of the current call assigned to the volunteer // Taken from DO.Call entity
    public CallType CallType { get; set; }
    public override string ToString() => this.ToStringProperty();



}
