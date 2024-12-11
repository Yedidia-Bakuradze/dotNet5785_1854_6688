namespace Helpers;

using BO;
using DO;
using System.Text.RegularExpressions;
using System.Xml.Linq;

//Remember: All the method shall be as internal static
//Remember: The class shall be internal static
public static class VolunteerManager
{
    //API Configurations
    static readonly string URI = "https://maps.googleapis.com/maps/api/geocode/";
    static readonly string APIKEY = "AIzaSyBjTxg6_sTOtBKHkaBJ7bkCpk8ylmjLMGk";

    //private static DalApi.IDal s_dal = DalApi.Factory.Get; //stage 4
    public static (double?, double?) GetGeoCordinates(string streetAddress)
    {
        //Dealocates the memory after exiting the scope
        using HttpClient client = new HttpClient();

        //Builds the URL requests
        Uri requestUri = new Uri(URI + FileFormat.xml.ToString() + $"?address={Uri.EscapeDataString(streetAddress)}" + $",+CA&key={APIKEY}");
        
        //Accepts the GET response
        string xmlTree = new HttpClient().GetAsync(requestUri.AbsoluteUri)
                        .Result
                        .Content
                        .ReadAsStringAsync()
                        .Result;

        //Checks the status of the call
        XElement status;
        try
        {
            status = XElement.Parse(xmlTree).Element("status")
                ?? throw new BoHttpGetException("Http Exception: Response is unable to be converted to an XML file");
            //If the GET response content is not good then the address it coropted
            if(status?.Value != "OK")
            {
                throw new BoHttpGetException("Http Exception: The GeoCoding has been failed: Status GET request is not OK");
            }
        }
        catch(Exception ex)
        {
            return (null, null);
        }


        //Gets the location element which holds the cordinates
        XElement? locationElement = XElement.Parse(xmlTree)
                                        ?.Element("result")
                                        ?.Element("geometry")
                                        ?.Element("location");

        
        return ((double?)locationElement?.Element("lat"), (double?)locationElement?.Element("lng"));
    }

    /// <summary>
    /// This static method checks if the given user id is valid
    /// </summary>
    /// <param name="id">The uesr's id</param>
    /// <returns>Boolean value whether its valid or not</returns>
    public static bool IsVolunteerIdValid(int id)
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
    /// <exception cref="BoUnimplementedMethodOrFunction">UnImplemented exception</exception>
    public static bool IsValidFullName(string name)
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
    /// <exception cref="BoUnimplementedMethodOrFunction">UnImplemented exception</exception>
    public static bool IsValidPhoneNumber(string phoneNumber)
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
    public static bool IsValidEmailAddress(string email)
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
    /// <exception cref="BoUnimplementedMethodOrFunction">UnImplemented exception</exception>
    public static bool IsValidPassword(string? password)
    {
        //TODO: [Bonus]: eyncrypt the password using SHA256
        
        if (password == null)
            return true;

        // Check if the password is at least 8 characters long
        if (password.Length < 8)
            return false;

        // Regex pattern to check if the password contains at least two special characters
        string specialCharPattern = @"[^a-zA-Z0-9]{2,}";
        if (!Regex.IsMatch(password, specialCharPattern))
            return false;

        // Regex pattern to check if the password contains at least two numbers
        string numberPattern = @"\d{2,}";
        if (!Regex.IsMatch(password, numberPattern))
            return false;

        return true;
    }

    /// <summary>
    /// This static method checks if the given street address is valid
    /// </summary>
    /// <param name="streetAddress">The uesr's street address</param>
    /// <returns>Boolean value whether its valid or not</returns>
    public static bool IsStreetAddressValid(string? streetAddress)
    {
        //If the user doesn't have a registered address - its ok because its optional
        if (streetAddress == null)
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
    public static bool IsMaxDistnaceValid(double? distance)
    {
        return (distance == null) ? true : distance >= 0.0;
    }

    /// <summary>
    /// This method checks if the volunteer's field values are valid and returns the proper boolean value
    /// </summary>
    /// <param name="volunteer">The Volunteer instance</param>
    /// <returns>a boolean value whether the volunteer is valid or not</returns>
    public static bool IsVolunteerValid(BO.Volunteer volunteer)
    {
        return
            IsVolunteerIdValid(volunteer.Id) &&
            IsValidFullName(volunteer.FullName) &&
            IsValidPhoneNumber(volunteer.PhoneNumber) &&
            IsValidEmailAddress(volunteer.Email) &&
            IsValidPassword(volunteer.Password) &&
            IsStreetAddressValid(volunteer.FullCurrentAddress) &&
            IsMaxDistnaceValid(volunteer.MaxDistanceToCall);
            
    }
}
