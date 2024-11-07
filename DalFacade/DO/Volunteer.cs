namespace DO;
/// <summary>
///             
/// </summary>
/// <param name="id"></param>
/// <param name="Role"></param>
/// <param name="fullName"></param>
/// <param name="phoneNumber"></param>
/// <param name="email"></param>
/// <param name="maxDistancToCall"></param>
/// <param name="TypeOfRange"></param>
/// <param name="active"></param>
/// <param name="password"></param>
/// <param name="fullCurrentAdress"></param>
/// <param name="latitude"></param>
/// <param name="longtitude"></param>
public record Volunteer
(
int id,// Valid check digit
Roles Role,
string fullName,
string phoneNumber,//Starts with 0, 10 digit long
string email,
double? maxDistancToCall,
typeOfRange TypeOfRange,
bool active,
string? password = null,
string? fullCurrentAdress = null,
double? latitude = null,
double? longtitude = null
)
{
    public Volunteer() : this(0, Roles.Undefind, "","","",0, typeOfRange.Undefind,false) { }
};