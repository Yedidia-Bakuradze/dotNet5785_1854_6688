

namespace Dal;

/// <summary>
/// List containing list of all types 
/// </summary>
internal static class DalList
{
    internal static List<DO.Assignment> Assignments { get;}= new(); 
    internal static List<DO.Call> Calls {get;}= new();
    internal static List<DO.Volunteer> Volunteers { get; } = new();
}
