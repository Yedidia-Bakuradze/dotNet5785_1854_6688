using DalApi;
namespace Dal;

/// <summary>
/// This class manages the data access layer via the XML file including all the CRUD methods
/// </summary>
public class DalXml : IDal
{
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
