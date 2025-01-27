namespace Helpers;

using BO;
using DalApi;
using DO;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

//Remember: All the method shall be as internal static
//Remember: The class shall be internal static
internal static class VolunteerManager
{
    #region Propeties
    //API Configurations
    static readonly string URI = "https://maps.googleapis.com/maps/api/";
    static readonly string APIKEY = "AIzaSyDhFsDBvWYHUmKJ-aenR3jXGOV2USDKteU";
    private static DalApi.IDal s_dal = DalApi.Factory.Get; //stage 4
    internal static ObserverManager Observers = new(); //stage 5 
    #endregion

    #region Help Methods
    /// <summary>
    /// Converts degrees to radian
    /// </summary>
    /// <param name="angle">The requested degress value</param>
    /// <returns>The radian representation of the given degree</returns>
    private static double ToRadians(double angle) => angle * (Math.PI / 180);

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

    #endregion

    #region HTTP Request Methods
    /// <summary>
    /// This method recives an Http response value and converts it to a XElement
    /// </summary>
    /// <param name="httpResponse">The string result of the API call</param>
    /// <returns>The root XElement value</returns>
    /// <exception cref="BlHttpGetException">Throws an exception if the Http response was corrupted The response status was not OK</exception>
    private static async Task<XElement> HttpGetXmlReponse(Uri requestUri)
    {
        using HttpClient client = new HttpClient();

        //Accepts the GET response
        string httpResponse = await client.GetAsync(requestUri.AbsoluteUri)
                        .Result
                        .Content
                        .ReadAsStringAsync();

        //Try to parse to Xml content value
        XElement root = XElement.Parse(httpResponse).Element("status")
                        ?? throw new BlHttpGetException("Http Exception: Response is unable to be converted to an XML file");

        //If the GET response content is not good then the address it coropted
        if (root?.Value != "OK")
            throw new BlHttpGetException("Http Exception: The GeoCoding has been failed: Status GET request is not OK");
        return XElement.Parse(httpResponse);
    }
    #endregion

    #region Volunteer Details Validation Methods
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
    private static async Task<bool> IsStreetAddressValid(string? streetAddress)
    {
        //If the user doesn't have a registered address - its ok because its optional
        if (streetAddress == null || streetAddress == "")
            return true;

        //If the user has any address - check if it has a valid cordinates
        (double? a, double? b) = await GetGeoCordinates(streetAddress);
        return a != null && b != null;
    }

    /// <summary>
    /// This static method checks if the given max distance is valid
    /// </summary>
    /// <param name="distance">The uesr's max distance</param>
    /// <returns>Boolean value whether its valid or not</returns>
    private static bool IsMaxDistnaceValid(double? distance) => distance == null || distance >= 0.0;

    /// <summary>
    /// This method checks if the volunteer's field values are valid and returns the proper boolean value
    /// </summary>
    /// <param name="volunteer">The Volunteer instance</param>
    /// <param name="isPasswordOk">[Optional] if true the method wont check the hashed password</param>
    /// <returns>a boolean value whether the volunteer is valid or not</returns>
    internal static async Task<bool> IsVolunteerValid(BO.Volunteer volunteer, bool isPasswordOk = false)
    =>
            IsVolunteerIdValid(volunteer.Id) &&
            IsValidFullName(volunteer.FullName) &&
            IsValidPhoneNumber(volunteer.PhoneNumber) &&
            IsValidEmailAddress(volunteer.Email) &&
            (isPasswordOk || IsValidPassword(volunteer.Password)) &&
            (await IsStreetAddressValid(volunteer.FullCurrentAddress)) &&
            IsMaxDistnaceValid(volunteer.MaxDistanceToCall);

    #endregion

    #region Geographic & Distance Location
    internal static bool AreCodinatesValid(params (double?, double?)[] vectors)
    {
        foreach (var vec in vectors)
            if (vec.Item1 is null || vec.Item2 is null)
                return false;
        return true;
    }

    /// <summary>
    /// This method returns a tuple containing the cordinates (latitude, logitude) of a given street address if exsists, otherwise it would return tuple of null values
    /// </summary>
    /// <param name="streetAddress">The requested street address to convert to cordinates</param>
    /// <returns>Tuple containing the cordinates (latitude, logitude), if the address is not valid it would return tuple of null values</returns>
    internal static async Task<(double?, double?)> GetGeoCordinates(string streetAddress)
    {
        //Builds the URL requests
        Uri requestUri = new Uri(URI + "geocode/" + FileFormat.xml.ToString() + $"?address={Uri.EscapeDataString(streetAddress)}" + $",+CA&key={APIKEY}");

        try
        {
            XElement res = await HttpGetXmlReponse(requestUri);
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
    /// This method calculates the air distance between the given streets
    /// </summary>
    /// <param name="origin">The departure street</param>
    /// <param name="destanation">The arrival street</param>
    /// <returns>The distance in KM</returns>
    private static double CalculatedAirDistance((double?, double?) origin, (double?, double?) destanation)
    {
        if (origin is (null, null))
            return 0;

        double R = 6371; // Radius of the Earth in kilometers
        double dLat = ToRadians((double)(destanation.Item1 - origin.Item1));
        double dLon = ToRadians((double)(destanation.Item2 - origin.Item2));
        double lat1Rad = ToRadians((double)origin.Item1);
        double lat2Rad = ToRadians((double)destanation.Item1);

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return R * c; // Distance in kilometers
    }

    /// <summary>
    /// This method calculates the walking distance between the given coordinates
    /// </summary>
    /// <param name="origin">The departure coordinates</param>
    /// <param name="destination">The arrival coordinates</param>
    /// <returns>The distance in KM</returns>
    private static double CalculatedWalkingDistance((double?, double?) origin, (double?, double?) destination)
    {
        if (origin is (null, null))
            return 0;

        Uri requestUri = new Uri($"{URI}distancematrix/{FileFormat.xml}?destinations={destination.Item1},{destination.Item2}&mode={DistanceType.walking}&origins={origin.Item1},{origin.Item2}&key={APIKEY}");
        XElement root = HttpGetXmlReponse(requestUri).Result;

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
    private static double CalculatedDrivingDistance((double?, double?) origin, (double?, double?) destination)
    {
        if (origin is (null, null))
            return 0;

        Uri requestUri = new Uri($"{URI}distancematrix/{FileFormat.xml}?destinations={destination.Item1},{destination.Item2}&mode={DistanceType.driving}&origins={origin.Item1},{origin.Item2}&key={APIKEY}");
        XElement root = HttpGetXmlReponse(requestUri).Result;

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
    /// This method returns the distance between the volunteer to the call depending on the range type which is requested to be calculated with
    /// </summary>
    /// <param name="volunteerAddress">The Volunteer's address</param>
    /// <param name="callAddress">The call's address</param>
    /// <param name="typeOfRange">The specified range, either Air, Walking or Driving distance</param>
    /// <returns>The distnace in KM calculated as requested</returns>
    internal static double CalculateDistanceFromVolunteerToCall((double,double) volunteer, (double, double) call, DO.TypeOfRange typeOfRange)
        => typeOfRange switch
        {
            DO.TypeOfRange.WalkingDistance => CalculatedWalkingDistance(volunteer, call),
            DO.TypeOfRange.AirDistance => CalculatedAirDistance(volunteer, call),
            DO.TypeOfRange.DrivingDistance => CalculatedDrivingDistance(volunteer, call),
            _ => throw new BO.BlInvalidDistanceCalculationException("BL: Invalid type of distance calculation has been requested")
        };

    /// <summary>
    /// This method converts from DO version of Volunteer to its BO version
    /// </summary>
    /// <param name="volunteer">The original DO Volunteer variable</param>
    /// <param name="callInProgress">The assosiated call to that volunteer</param>
    /// <returns>a new BO Volunteer variable</returns>
    #endregion

    #region Convertors
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
            Role = (DO.UserRole)volunteer.Role,
            FullName = volunteer.FullName,
            PhoneNumber = volunteer.PhoneNumber,
            Email = volunteer.Email,
            MaxDistanceToCall = volunteer.MaxDistanceToCall,
            RangeType = (DO.TypeOfRange)volunteer.RangeType,
            IsActive = volunteer.IsActive,
            Password = volunteer.Password,
            FullCurrentAddress = volunteer.FullCurrentAddress,
            Latitude = volunteer.Latitude,
            Longitude = volunteer.Longitude
        };

    #endregion

    #region Simulator's Methods
    internal static void SystemSimulatorVolunteer()
    {
        try
        {
            IEnumerable<DO.Volunteer> ActiveVolunteers;
            DO.Assignment? volunteersCurrentCallAssignment;
            ActiveVolunteers = from volunteer in s_dal.Volunteer.ReadAll()
                                where volunteer.IsActive
                                select volunteer;

            foreach (DO.Volunteer volunteer in ActiveVolunteers)
            {
                lock (AdminManager.BlMutex)
                {
                    volunteersCurrentCallAssignment = (from assign in s_dal.Assignment.ReadAll()
                                                    where assign.VolunteerId == volunteer.Id && assign.TypeOfEnding is null
                                                    orderby assign.Id descending
                                                    select assign).FirstOrDefault();

                    if (volunteersCurrentCallAssignment is not null && volunteersCurrentCallAssignment?.TypeOfEnding is null)
                        FinishOrCancelAssignmentCallToVolunteerSimulator(volunteer, volunteersCurrentCallAssignment!);
                    else
                        AssignCallToVolunteerSimulator(volunteer);
                }
            }

        }
        catch (Exception ex)
        {
            throw new BlDoesNotExistException(ex.Message);
        }
    }

    [Obsolete]
    internal static bool ShouldFinishACall(DO.Call call, DO.Volunteer volunteer)
    {
        if (call.DeadLine is not null)
        {
            TimeSpan remainingTime = (DateTime)call.DeadLine - AdminManager.Now;
            switch (volunteer.RangeType)
            {
                case DO.TypeOfRange.AirDistance:
                    return (remainingTime + AdminManager.RiskRange).TotalMinutes < 21;
                case DO.TypeOfRange.DrivingDistance:
                    return (remainingTime + AdminManager.RiskRange).TotalMinutes < 42;
                case DO.TypeOfRange.WalkingDistance:
                    return (remainingTime + AdminManager.RiskRange).TotalMinutes < 50;
            }
        }
        switch (volunteer.RangeType)
        {
            case DO.TypeOfRange.AirDistance:
                return true;
            case DO.TypeOfRange.DrivingDistance:
                return false;
            case DO.TypeOfRange.WalkingDistance:
                return true;
        }
        return true;
    }
    internal static bool ShouldFinishACall() => new Random().Next(0, 2) == 0;

    internal static void AssignCallToVolunteerSimulator(DO.Volunteer volunteer)
    {

        DO.Call randomCall;
        IEnumerable<DO.Call> openCalls;
        openCalls = from call in s_dal.Call.ReadAll()
                        let status = CallManager.GetStatus(call.Id)
                        where status == CallStatus.OpenAndRisky || status == CallStatus.Open
                        select call;
        //If not enough calls to take
        if (openCalls.Count() == 0)
            return;

        if (openCalls.Count() == 1)
            randomCall = openCalls.FirstOrDefault()!;
        else
            randomCall = openCalls.ToList()[new Random().Next(0, openCalls.Count() - 1)];

        lock (AdminManager.BlMutex)
        {
            s_dal.Assignment.Create(new DO.Assignment
            {
                Id = -1, //Temp id, the real id is assigned in the Dal layer
                CallId = randomCall.Id,
                VolunteerId = volunteer.Id,
                TimeOfEnding = null,
                TimeOfStarting = AdminManager.Now,
                TypeOfEnding = null,
            });
        }

        CallManager.Observers.NotifyListUpdated();
    }

    internal static void FinishOrCancelAssignmentCallToVolunteerSimulator(DO.Volunteer volunteer,DO.Assignment assignment)
    {
        DO.Call call;
        lock (AdminManager.BlMutex)
        {
            if (assignment is null)
                return;

            call = s_dal.Call.Read(c => c.Id == assignment.CallId)
                ?? throw new BO.BlDoesNotExistException($"BL Says: Call {assignment.CallId} does not exist");
        }

        if (ShouldFinishACall())
        {
            try
            {
                lock (AdminManager.BlMutex)
                {
                    s_dal.Assignment.Update(assignment with
                    {
                        TimeOfEnding = AdminManager.Now,
                        TypeOfEnding = DO.TypeOfEnding.Treated,
                    });
                }
                CallManager.Observers.NotifyListUpdated();
            }
            catch (DO.DalDoesNotExistException ex)
            {
                throw new BO.BlDoesNotExistException($"BL: Assignment with Id: {assignment.Id} doesn't exists", ex);
            }
        }
        else
        {
            try
            {
                lock (AdminManager.BlMutex)
                {
                    s_dal.Assignment.Update(assignment with
                    {
                        TypeOfEnding = (assignment.VolunteerId != volunteer.Id) ? DO.TypeOfEnding.AdminCanceled : DO.TypeOfEnding.SelfCanceled,
                        TimeOfEnding = AdminManager.Now,
                    });
                }

                CallManager.Observers.NotifyListUpdated();
            }
            catch (DO.DalDoesNotExistException ex)
            {
                throw new BO.BlDoesNotExistException($"BL: Assignment with Id: {assignment.Id} doesn't exists", ex);
            }
        }
        
    }

    #endregion
}