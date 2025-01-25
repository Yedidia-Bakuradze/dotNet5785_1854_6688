
using System.Runtime.CompilerServices;

namespace Dal;
/// <summary>
/// This class manages the id's and other system configuration settings via the XML file data-config.xml
/// </summary>
static internal class Config
{
    //List of XML file names for access
    internal const string dataConfigFileName = "data-config.xml";
    internal const string assignmentFileName = "assignments.xml";
    internal const string callFileName = "calls.xml";
    internal const string volunteerFileName = "volunteers.xml";

    internal static DateTime Clock { 
        [MethodImpl(MethodImplOptions.Synchronized)]
        get =>  XMLTools.GetConfigDateVal(dataConfigFileName, "Clock");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigDateVal(dataConfigFileName, "Clock", value);
    }
    internal static int NextAssignmentId
    {
    [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetAndIncreaseConfigIntVal(dataConfigFileName, "NextAssignmentId");
    [MethodImpl(MethodImplOptions.Synchronized)]
        private set => XMLTools.SetConfigIntVal(dataConfigFileName, "NextAssignmentId", value);
    }
    internal static int NextCallId
    {
    [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetAndIncreaseConfigIntVal(dataConfigFileName, "NextCallId");
    [MethodImpl(MethodImplOptions.Synchronized)]
        private set => XMLTools.SetConfigIntVal(dataConfigFileName, "NextCallId", value);
    }

    internal static TimeSpan RiskRange
    {
    [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigTimeSpan(dataConfigFileName, "RiskRange");
    [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigTimeSpan(dataConfigFileName, "RiskRange",value);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    internal static void Reset()
    {
        NextAssignmentId = 1;
        NextCallId = 1;
        Clock = DateTime.Now;   
        RiskRange = TimeSpan.Zero;
    }
}
