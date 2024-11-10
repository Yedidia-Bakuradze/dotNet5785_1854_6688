
namespace DO;
/// <summary>
/// Represents an assignment entity for a volunteer-based system.
/// </summary>
/// <param name="Id">A unique identifier for each assignment, generated automatically. To initialize, provide the argument as 0.</param>
/// <param name="Called">Indicates if the assignment was initiated by a call (1) or another method (0).</param>
/// <param name="VolunteerId">The unique ID of the volunteer assigned to this task.</param>
/// <param name="TimeOfStarting">The start date and time of the assignment.</param>
/// <param name="TimeOfEnding">The end date and time of the assignment, if completed; otherwise, null.</param>
/// <param name="TypeOfEnding">Indicates the reason or status when the assignment ends, using a predefined ending type.</param>

public record Assignment
(
    int Id,
    int Called,
    int VolunteerId,
    DateTime TimeOfStarting,
    DateTime? TimeOfEnding = null,
    typeOfEnding? TypeOfEnding = null
)
{
    public Assignment() : this(0,0,0,DateTime.Now) { }
};
