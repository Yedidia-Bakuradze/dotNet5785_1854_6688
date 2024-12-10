namespace Helpers;

using BO;
using DalApi;
//Remember: All the method shall be as internal static
internal static class VolunteerManager
{
    private static IDal s_dal = Factory.Get; //stage 4

    /// <summary>
    /// This static method checks if the given user name is valid
    /// </summary>
    /// <param name="name">The uesr's full name</param>
    /// <returns>Boolean value whether its valid or not</returns>
    /// <exception cref="BoUnimplementedMethodOrFunction">UnImplemented exception</exception>
    public static bool IsValidFullName(string name)
    {
        //TODO: check using regex if the full name is valid
        //Check if has two words saparated with a blank space
        //Check if the name doesnt contain characters such as ',@# etc
        throw new BoUnimplementedMethodOrFunction("IsValidFullName at VolunteerManager class hasn't been implemented");
    }

    /// <summary>
    /// This static method checks if the given phone number is valid
    /// </summary>
    /// <param name="phoneNumber">The uesr's phone number</param>
    /// <returns>Boolean value whether its valid or not</returns>
    /// <exception cref="BoUnimplementedMethodOrFunction">UnImplemented exception</exception>
    public static bool IsValidPhoneNumber(string phoneNumber)
    {
        //TODO: check if the phone number is valid
        //Check if it doesn't contain any letters
        //Check if it starts with + sign
        //Check that the length is valid
        //[Optional]: Check if the first digits are a valid prefix for a number

        throw new BoUnimplementedMethodOrFunction("IsValidPhoneNumber at VolunteerManager class hasn't been implemented");
    }
    
    /// <summary>
    /// This static method checks if the given email address is valid
    /// </summary>
    /// <param name="email">The uesr's email address</param>
    /// <returns>Boolean value whether its valid or not</returns>
    /// <exception cref="Exception">UnImplemented exception</exception>
    public static bool IsValidEmailAddress(string email)
    {
        //TODO: check using regex if the email is valid
        //Check if it ends with .somthing
        //Check if it contains @ sign
        throw new BoUnimplementedMethodOrFunction("IsValidEmailAddress at VolunteerManager class hasn't been implemented");
    }

    /// <summary>
    /// This static method checks if the given password is valid
    /// </summary>
    /// <param name="pasword">The uesr's password</param>
    /// <returns>Boolean value whether its valid or not</returns>
    /// <exception cref="BoUnimplementedMethodOrFunction">UnImplemented exception</exception>
    public static bool IsValidPassword(string? pasword)
    {
        //TODO: check using regex if the password is valid
        //Check if it ends doesn't contain any blank spaces

        //TODO: [Bonus]: check if the password is strong 
        //Check that it contains some special characters
        //Check that it contains some numbers

        //TODO: [Bonus]: eyncrypt the password using SHA256

        throw new BoUnimplementedMethodOrFunction("IsValidPassword at VolunteerManager class hasn't been implemented");
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
            IsValidFullName(volunteer.FullName) &&
            IsValidPhoneNumber(volunteer.PhoneNumber) &&
            IsValidEmailAddress(volunteer.Email) &&
            IsValidPassword(volunteer.Password) &&
            IsStreetAddressValid(volunteer.FullCurrentAddress) &&
            IsMaxDistnaceValid(volunteer.MaxDistanceToCall);
            
    }
}
