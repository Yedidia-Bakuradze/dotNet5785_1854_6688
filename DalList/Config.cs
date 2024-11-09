
namespace Dal;

internal static class Config
{
    //Id generator for the Call model
    internal const int StartCallId = 1;
    private static int nextCallId = StartCallId;
    internal static int NextCallId { get => nextCallId++; }


    //Id generator for the Assignment model
    internal const int StartAssignmentId = 0;
    private static int nextAssignmentId = StartAssignmentId;
    internal static int NextAssignmentId { get => nextAssignmentId++; }

    //RiskRange handler
    internal static TimeSpan RiskRange { get; set; } = TimeSpan.Zero;
    internal static DateTime Clock { get; set; } = DateTime.Now;

    internal static void reset()
    {
        nextAssignmentId = StartAssignmentId;
        nextCallId = StartCallId;
        RiskRange = TimeSpan.Zero;
        Clock = DateTime.Now;
    }
}
