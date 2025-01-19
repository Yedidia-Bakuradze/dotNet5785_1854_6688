
using Helpers;

namespace BO;

public class CallAssignInList
{
    // volunteerId - unique identifier for the volunteer // taken from DO.Assignment entity
    public int? VolunteerId { get; init; }

    // volunteerName - name of the volunteer // taken from DO.Volunteer entity
    public string? VolunteerName { get; set; }

    // entryTime - time when the call was assigned // taken from DO.Assignment entity
    public DateTime EntryTime { get; set; }

    // finishTime - time when the call was finished (if applicable) // taken from DO.Assignment entity
    public DateTime? FinishTime { get; set; }

    // typeOfClosedCall - type of closed call (if applicable) // taken from DO.Assignment entity
    public TypeOfEndingCall? TypeOfClosedCall { get; set; }

    public override string ToString() => this.ToStringProperty();
}
