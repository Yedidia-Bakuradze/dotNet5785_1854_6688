namespace Helpers;

using BO;
using System.Text.RegularExpressions;

//Remember: All the method shall be as internal static
internal static class VolunteerManager
{
    private static DalApi.IDal s_dal = DalApi.Factory.Get; //stage 4

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
    /// <exception cref="BoUnimplementedMethodOrFunction">UnImplemented exception</exception>
    public static bool IsStreetAddressValid(string? streetAddress)
    {
        //TODO: check if the street address is valid
        //Check logics: Check if it has a valid cordinates in the world
        //Check Formating: Check if the format is valid [Street address number, city]
        throw new BoUnimplementedMethodOrFunction("IsStreetAddressValid at VolunteerManager class hasn't been implemented");
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
