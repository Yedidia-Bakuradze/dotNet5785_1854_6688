using DalApi;
namespace Dal;

/// <summary>
/// This class manages the data access layer via the XML file including all the CRUD methods
/// </summary>
 sealed internal class DalXml : IDal
{
    public static IDal Instance { get; } = new DalXml();    
    private DalXml() { }
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
