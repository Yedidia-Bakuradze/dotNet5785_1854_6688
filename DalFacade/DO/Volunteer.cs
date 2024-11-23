using System.ComponentModel;

namespace DO;
/// <summary>
/// Represents a volunteer entity with personal details and preferences.
/// </summary>
/// <param name="Id">A unique identifier for each volunteer. Must include a valid check digit.</param>
/// <param name="Role">The role or position of the volunteer, specified using predefined roles.</param>
/// <param name="FullName">The full name of the volunteer.</param>
/// <param name="PhoneNumber">The volunteer's phone number, which should start with 0 and be exactly 10 digits long.</param>
/// <param name="Email">The volunteer's email address for communication.</param>
/// <param name="MaxDistanceToCall">The maximum distance (in kilometers) the volunteer is willing to travel when called for assignments. Null if no limit is set.</param>
/// <param name="TypeOfRange">The type of range preference for assignments (e.g., local, regional, etc.).</param>
/// <param name="Active">Indicates whether the volunteer is currently active (true) or inactive (false) in the system.</param>
/// <param name="Password">An optional password for the volunteer's access, null if not set.</param>
/// <param name="FullCurrentAddress">The full current address of the volunteer, null if not provided.</param>
/// <param name="Latitude">The latitude of the volunteer's location, for mapping purposes; null if not specified.</param>
/// <param name="Longitude">The longitude of the volunteer's location, for mapping purposes; null if not specified.</param>
public record Volunteer
(
    int Id,// Valid check digit
    Roles Role,
    string FullName,
    string PhoneNumber,//Starts with 0, 10 digit long
    string Email,
    double? MaxDistanceToCall,
    TypeOfRange TypeOfRange,
    bool Active,
    string? Password = null,
    string? FullCurrentAddress = null,
    double? Latitude = null,
    double? Longitude = null
)
{
    public Volunteer() : this(0, Roles.Undefined, "","","",0, TypeOfRange.AirDistance,false) { }
};