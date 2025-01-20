namespace Helpers;

using BO;
using DO;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

//Remember: All the method shall be as internal static
//Remember: The class shall be internal static
internal static class VolunteerManager

{
    //API Configurations
    static readonly string URI = "https://maps.googleapis.com/maps/api/";
    static readonly string APIKEY = "AIzaSyDhFsDBvWYHUmKJ-aenR3jXGOV2USDKteU";

    private static DalApi.IDal s_dal = DalApi.Factory.Get; //stage 4

    internal static ObserverManager Observers = new(); //stage 5 

    /// <summary>
    /// Converts degrees to radian
    /// </summary>
    /// <param name="angle">The requested degress value</param>
    /// <returns>The radian representation of the given degree</returns>
    private static double ToRadians(double angle)
    {
        return angle * (Math.PI / 180);
    }

    /// <summary>
    /// This method recives an Http response value and converts it to a XElement
    /// </summary>
    /// <param name="httpResponse">The string result of the API call</param>
    /// <returns>The root XElement value</returns>
    /// <exception cref="BlHttpGetException">Throws an exception if the Http response was corrupted The response status was not OK</exception>
    private static XElement HttpGetXmlReponse(Uri requestUri)
    {
        using HttpClient client = new HttpClient();

        //Accepts the GET response
        string httpResponse = client.GetAsync(requestUri.AbsoluteUri)
                        .Result
                        .Content
                        .ReadAsStringAsync()
                        .Result;

        //Try to parse to Xml content value
        XElement root = XElement.Parse(httpResponse).Element("status")
                        ?? throw new BlHttpGetException("Http Exception: Response is unable to be converted to an XML file");

        //If the GET response content is not good then the address it coropted
        if (root?.Value != "OK")
        {
            throw new BlHttpGetException("Http Exception: The GeoCoding has been failed: Status GET request is not OK");
        }
        return XElement.Parse(httpResponse);
    }



    /// <summary>
    /// This method returns a tuple containing the cordinates (latitude, logitude) of a given street address if exsists, otherwise it would return tuple of null values
    /// </summary>
    /// <param name="streetAddress">The requested street address to convert to cordinates</param>
    /// <returns>Tuple containing the cordinates (latitude, logitude), if the address is not valid it would return tuple of null values</returns>
    internal static (double?, double?) GetGeoCordinates(string streetAddress)
    {
        //Builds the URL requests
        Uri requestUri = new Uri(URI + "geocode/" + FileFormat.xml.ToString() + $"?address={Uri.EscapeDataString(streetAddress)}" + $",+CA&key={APIKEY}");
        
        try
        {
            XElement res = HttpGetXmlReponse(requestUri);
            res = res
                    ?.Element("result")
                    ?.Element("geometry")
                    ?.Element("location")
                    ?? throw new BlXmlElementDoesntExsist("BL: There is not result->geometry->location tag in the given Http GET response");
            return ((double?)res?.Element("lat"), (double?)res?.Element("lng"));

        }
        catch (BO.BlHttpGetException ex)
        {
            Console.WriteLine(ex.Message);
            return (null, null);
        }
        catch (BO.BlXmlElementDoesntExsist ex)
        {
            Console.WriteLine(ex.Message);
            return (null, null);
        }


    }

    /// <summary>
    /// This static method checks if the given user id is valid
    /// </summary>
    /// <param name="id">The uesr's id</param>
    /// <returns>Boolean value whether its valid or not</returns>
    private static bool IsVolunteerIdValid(int id)
    {
        // Convert the integer ID to a string for easier manipulation
        string idStr = id.ToString();

        // Check if the ID has exactly 9 digits
        if (idStr.Length != 9)
            return false;

        // Convert the ID to an integer array
        int[] digits = idStr.Select(c => c - '0').ToArray();

        // Calculate the sum according to the algorithm
        int sum = 0;
        for (int i = 0; i < 8; i++) // Only process the first 8 digits
        {
            int value = digits[i] * (i % 2 == 0 ? 1 : 2);
            sum += value > 9 ? value - 9 : value;
        }

        // Calculate the control digit
        int controlDigit = (10 - (sum % 10)) % 10;

        // Check if the control digit matches the last digit
        return controlDigit == digits[8];
    }

    /// <summary>
    /// This static method checks if the given user name is valid
    /// </summary>
    /// <param name="name">The uesr's full name</param>
    /// <returns>Boolean value whether its valid or not</returns>
    /// <exception cref="BlUnimplementedMethodOrFunction">UnImplemented exception</exception>
    private static bool IsValidFullName(string name)
    {
        // Regex pattern to check if there is at least one blank space, every word is at least two letters long,
        // and all characters are either letters or blank spaces
        string pattern = @"^(?=.*\s)([A-Za-z]{2,}\s)+[A-Za-z]{2,}$";
        return Regex.IsMatch(name, pattern);
    }

    /// <summary>
    /// This static method checks if the given phone number is valid
    /// </summary>
    /// <param name="phoneNumber">The uesr's phone number</param>
    /// <returns>Boolean value whether its valid or not</returns>
    /// <exception cref="BlUnimplementedMethodOrFunction">UnImplemented exception</exception>
    private static bool IsValidPhoneNumber(string phoneNumber)
    {
        string pattern = @"^0\d{9}$";
        return Regex.IsMatch(phoneNumber, pattern);
    }

    /// <summary>
    /// This static method checks if the given email address is valid
    /// </summary>
    /// <param name="email">The uesr's email address</param>
    /// <returns>Boolean value whether its valid or not</returns>
    /// <exception cref="Exception">UnImplemented exception</exception>
    private static bool IsValidEmailAddress(string email)
    {
        // Regex pattern to check if the email address is valid
        string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        return Regex.IsMatch(email, pattern);
    }

    /// <summary>
    /// This static method checks if the given password is valid
    /// </summary>
    /// <param name="pasword">The uesr's password</param>
    /// <returns>Boolean value whether its valid or not</returns>
    /// <exception cref="BlUnimplementedMethodOrFunction">UnImplemented exception</exception>
    private static bool IsValidPassword(string? password)
    {
        if (password == null || password == "")
            return true;

        // Check if the password is at least 8 characters long
        if (password.Length < 8)
            return false;

        // Check if the password contains at least two upper-case letters
        if (password.Count(char.IsUpper) < 2)
            return false;

        // Check if the password contains at least one special character
        string specialCharPattern = @"[!@#$%^&*(),.?""{}|<>]";
        if (!Regex.IsMatch(password, specialCharPattern))
            return false;

        // Check if the password contains at least two numbers
        if (password.Count(char.IsDigit) < 2)
            return false;

        return true;
    }

    /// <summary>
    /// This static method checks if the given street address is valid
    /// </summary>
    /// <param name="streetAddress">The uesr's street address</param>
    /// <returns>Boolean value whether its valid or not</returns>
    private static bool IsStreetAddressValid(string? streetAddress)
    {
        //If the user doesn't have a registered address - its ok because its optional
        if (streetAddress == null || streetAddress == "")
            return true;

        //If the user has any address - check if it has a valid cordinates
        (double? a,double?b) = GetGeoCordinates(streetAddress);
        return a != null && b != null;
    }

    /// <summary>
    /// This static method checks if the given max distance is valid
    /// </summary>
    /// <param name="distance">The uesr's max distance</param>
    /// <returns>Boolean value whether its valid or not</returns>
    private static bool IsMaxDistnaceValid(double? distance)
    {
        return distance == null || distance >= 0.0;
    }

    /// <summary>
    /// This method checks if the volunteer's field values are valid and returns the proper boolean value
    /// </summary>
    /// <param name="volunteer">The Volunteer instance</param>
    /// <param name="isPasswordOk">[Optional] if true the method wont check the hashed password</param>
    /// <returns>a boolean value whether the volunteer is valid or not</returns>
    internal static bool IsVolunteerValid(BO.Volunteer volunteer, bool isPasswordOk = false)
    =>
            IsVolunteerIdValid(volunteer.Id) &&
            IsValidFullName(volunteer.FullName) &&
            IsValidPhoneNumber(volunteer.PhoneNumber) &&
            IsValidEmailAddress(volunteer.Email) &&
            (isPasswordOk || IsValidPassword(volunteer.Password)) &&
            IsStreetAddressValid(volunteer.FullCurrentAddress) &&
            IsMaxDistnaceValid(volunteer.MaxDistanceToCall);
            

    /// <summary>
    /// This method calculates the air distance between the given streets
    /// </summary>
    /// <param name="origin">The departure street</param>
    /// <param name="destanation">The arrival street</param>
    /// <returns>The distance in KM</returns>
    private static double CalculatedAirDistance(string origin, string destanation)
    {
        (double? lat1, double? lon1) = GetGeoCordinates(origin);
        (double? lat2, double? lon2) = GetGeoCordinates(destanation);

        if (lat1 == null || lon1 == null || lat2 == null || lon2 == null)
        {
            throw new BO.BlInvalidEntityDetails("BL: One or both of the provided addresses could not be geocoded.");
        }

        double R = 6371; // Radius of the Earth in kilometers
        double dLat = ToRadians((double)(lat2 - lat1));
        double dLon = ToRadians((double)(lon2 - lon1));
        double lat1Rad = ToRadians((double)lat1);
        double lat2Rad = ToRadians((double)lat2);

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return R * c; // Distance in kilometers
    }

    /// <summary>
    /// This method calculates the walking distance between the given streets
    /// </summary>
    /// <param name="origin">The departure street</param>
    /// <param name="destanation">The arrival street</param>
    /// <returns>The distance in KM</returns>
    private static double CalculatedWalkingDistance(string origin, string destanation)
    {
        Uri requestUri = new Uri($"{URI}distancematrix/{FileFormat.xml}?destinations={Uri.EscapeDataString(destanation)},&mode={DistanceType.walking}&origins={Uri.EscapeDataString(origin)}&key={APIKEY}");
        XElement root = HttpGetXmlReponse(requestUri);

        //Issue 13: Fix the possible null value
        return (double)(Int32.Parse
            (root
                ?.Element("row")
                ?.Element("element")
                ?.Element("distance")
                ?.Element("value").Value.ToString()
            )) / 1000.0;
    }

    /// <summary>
    /// This method calculates the driving distance between the given streets
    /// </summary>
    /// <param name="origin">The departure street</param>
    /// <param name="destanation">The arrival street</param>
    /// <returns>The distance in KM</returns>
    private static double CalculatedDrivingDistance(string origin, string destanation)
    {
        Uri requestUri = new Uri($"{URI}distancematrix/{FileFormat.xml}&destinations={Uri.EscapeDataString(destanation)},&mode={DistanceType.driving}&origins={Uri.EscapeDataString(origin)}&key={APIKEY}");
        XElement root = HttpGetXmlReponse(requestUri);

        //Issue 13: Fix the possible null value
        return (double)(Int32.Parse
            (root
                ?.Element("row") 
                ?.Element("element")
                ?.Element("distance")
                ?.Element("value").Value.ToString()
            ))/1000.0;
    }


    /// <summary>
    /// This method returns the distance between the volunteer to the call depending on the range type which is requested to be calculated with
    /// </summary>
    /// <param name="volunteerAddress">The Volunteer's address</param>
    /// <param name="callAddress">The call's address</param>
    /// <param name="typeOfRange">The specified range, either Air, Walking or Driving distance</param>
    /// <returns>The distnace in KM calculated as requested</returns>
    internal static double CalculateDistanceFromVolunteerToCall(string volunteerAddress, string callAddress, DO.TypeOfRange typeOfRange)
    //Issue 14: Switch from using the addresses to use the cordinates
        => typeOfRange switch
    {
        DO.TypeOfRange.WalkingDistance => CalculatedWalkingDistance(volunteerAddress, callAddress) ,
            DO.TypeOfRange.AirDistance => CalculatedAirDistance(volunteerAddress, callAddress) ,
            DO.TypeOfRange.DrivingDistance => CalculatedDrivingDistance(volunteerAddress, callAddress),
            _ => throw new BO.BlInvalidDistanceCalculationException("BL: Invalid type of distance calculation has been requested")
        };

    /// <summary>
    /// This method converts from DO version of Volunteer to its BO version
    /// </summary>
    /// <param name="volunteer">The original DO Volunteer variable</param>
    /// <param name="callInProgress">The assosiated call to that volunteer</param>
    /// <returns>a new BO Volunteer variable</returns>
    internal static BO.Volunteer ConvertDoVolunteerToBoVolunteer(DO.Volunteer volunteer, BO.CallInProgress? callInProgress)
        => new BO.Volunteer
        {
            Id = volunteer.Id,
            CurrentCall = callInProgress,
            Email = volunteer.Email,
            FullCurrentAddress = volunteer.FullCurrentAddress,
            Latitude = volunteer.Latitude,
            Longitude = volunteer.Longitude,
            MaxDistanceToCall = volunteer.MaxDistanceToCall,
            Password = volunteer.Password,
            PhoneNumber = volunteer.PhoneNumber,
            RangeType = (BO.TypeOfRange)volunteer.RangeType,
            Role = (BO.UserRole)volunteer.Role,
            FullName = volunteer.FullName,
            IsActive = volunteer.IsActive,
        };

    /// <summary>
    /// This method converts from BO version of Volunteer to its DO version
    /// </summary>
    /// <param name="volunteer">The original BO Volunteer variable</param>
    /// <returns>a new DO Volunteer variable</returns>
    internal static DO.Volunteer ConvertBoVolunteerToDoVolunteer(BO.Volunteer volunteer)
        => new DO.Volunteer
        {
            Id = volunteer.Id,
            Role = (DO.UserRole) volunteer.Role,
            FullName = volunteer.FullName,
            PhoneNumber = volunteer.PhoneNumber,
            Email = volunteer.Email,
            MaxDistanceToCall = volunteer.MaxDistanceToCall,
            RangeType = (DO.TypeOfRange) volunteer.RangeType,
            IsActive = volunteer.IsActive,
            Password  = volunteer.Password ,
            FullCurrentAddress  = volunteer.FullCurrentAddress ,
            Latitude  = volunteer.Latitude ,
            Longitude  = volunteer.Longitude
        };

    /// <summary>
    /// This method accepts a string value and converts it into a hashed value using SHA256 algorithm
    /// </summary>
    /// <param name="originalPassword">The password before hashing</param>
    /// <returns>The password after hashing</returns>
    internal static string GetSHA256HashedPassword(string originalPassword)
    {
        using (SHA256 sha256Hash = SHA256.Create())
            {
                //Converts the chars into bytes and hashes them
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(originalPassword));

                //Converts back the hashed bytes into a hex value 
                StringBuilder builder = new StringBuilder();
                string hashedPassword = "";
                bytes.ToList().ForEach((byte val) => hashedPassword += val.ToString("x2"));    
                return hashedPassword;
            }
    }
}
