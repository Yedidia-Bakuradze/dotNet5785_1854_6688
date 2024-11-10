
namespace DO;
/// <summary>
/// 
/// </summary>
/// <param name="Id"></param>
/// <param name="Called"></param>
/// <param name="VolunteerId"></param>
/// <param name="TimeOfStarting"></param>
/// <param name="TimeOfEnding"></param>
/// <param name="TypeOfEnding"></param>
public record Assignment
(
    int Id,
    int Called,
    int VolunteerId,
    DateTime TimeOfStarting,
    DateTime? TimeOfEnding = null,
    TypeOfEnding? TypeOfEnding = null
)
{
    public Assignment() : this(0,0,0,DateTime.Now) { }
};
