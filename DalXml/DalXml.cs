using DalApi;
using System.Runtime.CompilerServices;
namespace Dal;

/// <summary>
/// This class manages the data access layer via the XML file including all the CRUD methods
/// </summary>
 sealed internal class DalXml : IDal
{
    // A private instance which is initialized only when the DalXml has been called at least one time (Lazy Singleton)
    private static IDal instance = null!;

    //an indicator for a thread safty lock
    private static readonly object lockObj = new object()!;

    //private ctor for class's use only
    [MethodImpl(MethodImplOptions.Synchronized)]
    private DalXml() { }

    //Returns the same instance without craeating multiple instances (Singleton)
    public static IDal Instance
    {
    [MethodImpl(MethodImplOptions.Synchronized)]
        get
        {
            if (instance == null)
            {
                instance = new DalXml();
            }
            return instance;
        }
    }


    public IAssignment Assignment { get; } = new AssignmentImplementation();

    public ICall Call { get; } = new CallImplementation();

    public IVolunteer Volunteer { get; } = new VolunteerImplementation();

    public IConfig Config { get; } = new ConfigImplementation();

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void ResetDB()
    {
        Assignment.DeleteAll();
        Call.DeleteAll();
        Volunteer.DeleteAll();
        Config.Reset();
    }
}
