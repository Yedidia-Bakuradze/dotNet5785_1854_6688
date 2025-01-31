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

    #region Volunteer Logic Methods
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
    internal static bool IsVolunteerValid(BO.Volunteer volunteer, bool isPasswordOk = false)
    =>
            IsVolunteerIdValid(volunteer.Id) &&
            IsValidFullName(volunteer.FullName) &&
            IsValidPhoneNumber(volunteer.PhoneNumber) &&
            IsValidEmailAddress(volunteer.Email) &&
            (isPasswordOk || IsValidPassword(volunteer.Password)) &&
            IsMaxDistnaceValid(volunteer.MaxDistanceToCall);


    internal static async Task UpdateVolunteerCordinates(int volunteerId, string? address, bool isNewVolunteer)
    {
        if (address is null || address == "")
            return;

        (double?, double?) coridnates = await VolunteerManager.GetGeoCordinates(address);

        if (!CordinatesValidator(coridnates))
        {
            if (isNewVolunteer)
            {
                lock (AdminManager.BlMutex)
                {
                    s_dal.Volunteer.Delete(volunteerId);
                }
                Observers.NotifyItemUpdated(volunteerId);
                Observers.NotifyListUpdated();
            }
            throw new BlInvalidCordinatesConversionException(address);

        }

        lock (AdminManager.BlMutex)
        {
            DO.Volunteer volunteer = s_dal.Volunteer.Read(volunteerId)
                ?? throw new BlDoesNotExistException($"Bl Says: Call with ID {volunteerId} does not exist");

            s_dal.Volunteer.Update(volunteer with
            {
                Latitude = (double)coridnates.Item1!,
                Longitude = (double)coridnates.Item2!,
            });
        }

        Observers.NotifyItemUpdated(volunteerId);
        Observers.NotifyListUpdated();
    }

    internal static void VertifyVolunteerDeletionAttempt(int volunteerId)
    {
        DO.Volunteer volunteer;
        lock (AdminManager.BlMutex)
        {
            //Tries to find such volunteer
            volunteer = s_dal.Volunteer.Read(volunteerId)
                ?? throw new BO.BlDoesNotExistException($"BL: Error while tyring to remove the volunteer {volunteerId}");
        }


        lock (AdminManager.BlMutex)
        {
            //Checks if the volunteer is in any records of assignments
            if (s_dal.Assignment.Read((DO.Assignment assignment) => assignment.VolunteerId == volunteerId) != null)
                throw new BO.BlEntityRecordIsNotEmpty($"BL: Unable to remove the volunteer {volunteerId} due to that it has references in other assignment records");
        }
    }

    internal static void VerifyVolunteerModificationAttempt(BO.Volunteer modifiedVolunteer, int id, bool isPasswordBeenModified)
    {
        DO.Volunteer volunteerToModify;
        DO.Volunteer volunteerActor;

        //Check if allowed to modify
        lock (AdminManager.BlMutex)
        {
            //Get original Volunteer for comparing
            volunteerToModify = s_dal.Volunteer.Read((DO.Volunteer oldVolunteer) => oldVolunteer.Id == modifiedVolunteer.Id)
            ?? throw new BO.BlDoesNotExistException($"BL: Volunteer with Id {modifiedVolunteer.Id} doesn't exsits");

            volunteerActor = s_dal.Volunteer.Read((DO.Volunteer volunteer) => volunteer.Id == id)
                ?? throw new BO.BlForbidenSystemActionExeption($"Bl Says: there is not volunteer with ID {id}");
        }

        //Check if actor volunteer is allowed to modify the modify volunteer
        if (id != modifiedVolunteer.Id && volunteerActor.Role != DO.UserRole.Admin)
            throw new BO.BlForbidenSystemActionExeption($"BL: Un granted access volunteer (Id:{id}) tries to modify the volunteer Id: {modifiedVolunteer.Id} values");

        //Check if logics are correct
        if (!VolunteerManager.IsVolunteerValid(modifiedVolunteer, !isPasswordBeenModified))
            throw new BO.BlInvalidEntityDetails($"BL: volunteer's fields (Id: {modifiedVolunteer.Id}) are invalid");

        //Checks what fields are requested to be modified - The role is modifable by only the manager
        if (modifiedVolunteer.Role != (BO.UserRole)volunteerToModify.Role && volunteerActor.Role != DO.UserRole.Admin)
            throw new BO.BlForbidenSystemActionExeption($"BL: Non-admin volunteer (Id: {id}) attemts to modify volunteer's Role (Id: {modifiedVolunteer.Id})");

        //Checks if the user tries to be inactive while running a call
        if (modifiedVolunteer.IsActive == false && modifiedVolunteer.CurrentCall is not null)
            throw new BO.BlForbidenSystemActionExeption($"BL: Volunteer cannot deactivate while having an active call, please close the current call and try again");
    }
    #endregion

    #region Geographic & Distance Location

    internal static bool CordinatesValidator(params (double?, double?)[] vectors) => !vectors.Any(val => val.Item1 is null || val.Item2 is null);


    /// <summary>
    /// This method returns a tuple containing the cordinates (latitude, logitude) of a given street address if exsists, otherwise it would return tuple of null values
    /// </summary>
    /// <param name="streetAddress"></param>
    /// <returns></returns>
    /// <exception cref="BlXmlElementDoesntExsist"></exception>
    internal static async Task<(double, double)> GetGeoCordinates(string streetAddress)
    {
        //Builds the URL requests
        Uri requestUri = new(URI + "geocode/" + FileFormat.xml.ToString() + $"?address={Uri.EscapeDataString(streetAddress)}" + $",+CA&key={APIKEY}");

        try
        {
            XElement res = await HttpGetXmlReponse(requestUri);
            if (res?.Element("status")?.Value != "OK")
                throw new BlHttpGetException($"Bl Says: Couldn't fetch locations data from server for street address: {streetAddress}");

            XElement resultTag = res
                ?.Elements("result")
                ?.FirstOrDefault(tag => tag
                        ?.Element("geometry")
                        ?.Element("location") is not null
                )
                ?? throw new BlInvalidCordinatesConversionException($"Bl Says: Status OK but no proper tag has been found");

            XElement result = resultTag
                            ?.Element("geometry")
                            ?.Element("location")!;
            
            (double,double) resCord = ((double)result.Element("lat")!, (double)result.Element("lng")!);
            return resCord;
        }
        catch (Exception ex)
        {
            throw new BlInvalidCordinatesConversionException(ex.Message);
        }


    }

    /// <summary>
    /// This method calculates the air distance between the given streets
    /// </summary>
    /// <param name="origin">The departure street</param>
    /// <param name="destination">The arrival street</param>
    /// <returns>The distance in KM</returns>
    private static double CalculatedAirDistance((double?, double?) origin, (double?, double?) destination)
    {
        if (!CordinatesValidator(origin, destination))
            return 0;

        double R = 6371; // Radius of the Earth in kilometers
        double dLat = ToRadians((double)(destination.Item1 - origin.Item1)!);
        double dLon = ToRadians((double)(destination.Item2 - origin.Item2)!);
        double lat1Rad = ToRadians((double)origin.Item1!);
        double lat2Rad = ToRadians((double)destination.Item1!);

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
        if (!CordinatesValidator(origin, destination))
            return 0;

        Uri requestUri = new Uri($"{URI}distancematrix/{FileFormat.xml}?&origins={origin.Item1},{origin.Item2}&destinations={destination.Item1},{destination.Item2}&mode={DistanceType.walking}&key={APIKEY}");
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
        if (!CordinatesValidator(origin, destination))
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
    internal static double CalculateDistanceFromVolunteerToCall((double?, double?) volunteer, (double?, double?) call, DO.TypeOfRange typeOfRange)
        => typeOfRange switch
        {
            DO.TypeOfRange.WalkingDistance => CalculatedWalkingDistance(volunteer, call),
            DO.TypeOfRange.AirDistance => CalculatedAirDistance(volunteer, call),
            DO.TypeOfRange.DrivingDistance => CalculatedDrivingDistance(volunteer, call),
            _ => throw new BO.BlInvalidDistanceCalculationException("BL: Invalid type of distance calculation has been requested")
        };


    #endregion

    #region Convertors

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
            Role = (DO.UserRole)volunteer.Role,
            FullName = volunteer.FullName,
            PhoneNumber = volunteer.PhoneNumber,
            Email = volunteer.Email,
            MaxDistanceToCall = volunteer.MaxDistanceToCall,
            RangeType = (DO.TypeOfRange)volunteer.RangeType,
            IsActive = volunteer.IsActive,
            Password = volunteer.Password,
            FullCurrentAddress = volunteer.FullCurrentAddress,
            Latitude = null,
            Longitude = null
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
        if (randomCall == null)
        {
            return;
        }
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

    internal static void FinishOrCancelAssignmentCallToVolunteerSimulator(DO.Volunteer volunteer, DO.Assignment assignment)
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