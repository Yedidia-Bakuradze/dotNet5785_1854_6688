
namespace Dal;
using DalApi;
sealed internal class DalList : IDal
{
    // A private instance which is initialized only when the DalList has been called at least one time (Lazy Singleton)
    private static DalList instance = null!;
    
    //an indicator for a lcoa
    private static readonly object lockObj = new object()!;
    
    //private ctor for class's use only
    private DalList(){}

    //Returns the same instance without craeating multiple instances (Singleton)
    public static DalList Instance {
        get { 
            if(instance == null)
            {
                //Thread safe: Only one thread creates the instance
                lock(lockObj){
                    if(instance == null)
                    {
                        instance = new DalList();
                    }
                }
            }
            return instance;
        }
    }
    
    public IAssignment Assignment { get; } = new AssignmentImplementation();

    public ICall Call { get; } = new CallImplementation();

    public IVolunteer Volunteer { get; } = new VolunteerImplementation();

    public IConfig Config { get; } = new ConfigImplementation();

    public void ResetDB()
    {
        Assignment.DeleteAll();
        Call.DeleteAll();
        Volunteer.DeleteAll();
        Config.Reset();
    }
}
