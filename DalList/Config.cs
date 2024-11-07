
namespace Dal;

internal static class Config
{
    internal const int startAssignmentId = 0;
    private static int nextAssignmentId = startAssignmentId;
    internal static int NextAssignmentId { get => nextAssignmentId++; }

    internal static DateTime Clock { get; set; } = DateTime.Now;
    internal static void reset()
    {
        nextAssignmentId = startAssignmentId;

        Clock = DateTime.Now;
    }
}
