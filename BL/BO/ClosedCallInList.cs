
namespace BO;

public class ClosedCallInList
{
    /// <summary>
    /// From DO.Call
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// From Do.Call
    /// </summary>
    public CallType TypeOfCall { get; set; }

    /// <summary>
    /// From Do.Call
    /// </summary>
    public string CallAddress { get; set; }

    /// <summary>
    /// From Do.Call
    /// </summary>
    public DateTime CallStartTime { get; set; }


    /// <summary>
    /// From Do.Assignment
    /// </summary>
    public DateTime EnteryTime { get; set; }


    /// <summary>
    /// From Do.Assignment
    /// </summary>
    public DateTime ClosingTime { get; set; }

    /// <summary>
    /// From Do.Assignment
    /// </summary>
    public TypeOfClosedCall TypeOfClosing{ get; set; }
}
