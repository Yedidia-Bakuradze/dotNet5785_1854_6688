namespace BlImplementation;
using BlApi;

/// <summary>
/// This class managers all the BL optional actions using the defined interfaces 
/// </summary>
internal class Bl : IBl
{
    public ICall Call { get; } = new CallImplementation();
    public IVolunteer Volunteer { get; } = new VolunteerImplementation();
    public IAdmin Admin { get; } = new AdminImplementation();
}
