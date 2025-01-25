
using System.Runtime.CompilerServices;

namespace Dal;

internal static class Config
{
    //Id generator for the Call model
    internal const int StartCallId = 1;
    private static int s_nextCallId = StartCallId;
    internal static int NextCallId { get => s_nextCallId++; }


    //Id generator for the Assignment model
    internal const int StartAssignmentId = 0;
    private static int s_nextAssignmentId = StartAssignmentId;
    internal static int NextAssignmentId { get => s_nextAssignmentId++; }

    //RiskRange handler
    internal static TimeSpan RiskRange { get; set; } = TimeSpan.Zero;
    internal static DateTime Clock { get; set; } = DateTime.Now;
    //Reset all the values
    [MethodImpl(MethodImplOptions.Synchronized)]
    internal static void Reset()
    {
        s_nextAssignmentId = StartAssignmentId;
        s_nextCallId = StartCallId;
        RiskRange = TimeSpan.Zero;
        Clock = DateTime.Now;
    }
}
