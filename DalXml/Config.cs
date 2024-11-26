
namespace Dal;

static internal class Config
{
    internal const string dataConfigFileName = "data-config";
    internal const string assignmentFileName = "assignments";
    internal const string callFileName = "calls";
    internal const string volunteerFileName = "volunteers";

    internal static DateTime Clock { 
        get =>  XMLTools.GetConfigDateVal(dataConfigFileName, "Clock");
        set => XMLTools.SetConfigDateVal(dataConfigFileName, "Clock", value);
    }

    internal static int NextAssignmentId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(assignmentFileName, "NextAssignmentId");
        private set => XMLTools.SetConfigIntVal(assignmentFileName, "NextAssignmentId", value);
    }
    internal static int NextCallId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(callFileName, "NextCallId");
        private set => XMLTools.SetConfigIntVal(callFileName, "NextCallId", value);
    }

    internal static TimeSpan RiskRange
    {
        get => XMLTools.GetConfigTimeSpan(dataConfigFileName, "RiskRange");
        set => XMLTools.SetConfigTimeSpan(dataConfigFileName, "RiskRange",value);
    }

    internal static void Reset()
    {
        NextAssignmentId = 1;
        NextCallId = 1;
        Clock = DateTime.Now;   
        RiskRange = TimeSpan.Zero;
    }
}
