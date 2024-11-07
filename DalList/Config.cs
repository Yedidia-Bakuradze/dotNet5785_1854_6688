
namespace Dal;

internal static class Config
{
    //Id generator for the Call model
    internal const int startCallId = 1;
    private static int nextCallId = startCallId;
    internal static int NextCallId { get => nextCallId++; }


    //RiskRange handler
    internal static TimeSpan RiskRange { get; set; } = TimeSpan.Zero;

    internal static void Reset()
    {
        nextCallId = startCallId;
        RiskRange = TimeSpan.Zero;
    }
}
