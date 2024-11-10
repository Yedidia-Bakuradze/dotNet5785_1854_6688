namespace DO;
/// <summary>
///  Volunteer entity           
/// </summary>
/// <param name="Id">unique ID</param>
/// <param name="Role"></param>
/// <param name="FullName">Volunteer full name </param>
/// <param name="PhoneNumber">Volunteer phone number</param>
/// <param name="Email">Volunteer email</param>
/// <param name="MaxDistancToCall"></param>
/// <param name="TypeOfRange"></param>
/// <param name="Active"></param>
/// <param name="Password"></param>
/// <param name="FullCurrentAdress"></param>
/// <param name="Latitude"></param>
/// <param name="Longitude"></param>
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
    public Volunteer() : this(0, Roles.Undefined, "","","",0, TypeOfRange.Undefined,false) { }
};