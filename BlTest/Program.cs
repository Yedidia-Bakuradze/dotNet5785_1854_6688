using BO;
using Helpers;

namespace BlTest;

internal class Program
{
    //Main BL Action manager for the bl layer
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    public enum MainMenuOperation { Exit, Call, Volunteer, Admin }
    public enum CallMenuOperation { Exit = 1, AddCall, UpdateCall, SelectCallToDo, UpdateCallEnd, EndOfCallStatusUpdate, DeleteCallRequest, GetListOfCalls, GetDetielsOfCall, GetClosedCallsByVolunteer, GetOpenCallsForVolunteer, GetTotalCallsByStatus }
    public enum IVolunteerOperations { Exit, Add, Remove, Update, Read, ReadAll, Login }
    public enum IAdminOperations { Exit, GetClock, UpdateClock, GetRiskRange, SetRiskRange, DbReset, DbInit }
    static void Main(string[] args)
    {
        MainMenu();
    }
    static public void MainMenu()
    {
        do
        {
            try
            {
                Console.Write(@"
----------------------------------------------------------------
Select Your Option:

Press 1 To Use ICall Interface
Press 2 To Use IVolunteer Interface
Press 3 To Use IAdmin Interface
Press 0 To Exit
----------------------------------------------------------------

>>> ");
                string input;
                MainMenuOperation operation;
                input = Console.ReadLine() ?? "";
                if (!Enum.TryParse(input, out operation))
                    throw new BO.BlInvalidOperationException($"Bl: Enum value for the main window is not a valid operation");

                switch (operation)
                {
                    case MainMenuOperation.Exit:
                        return;
                    case MainMenuOperation.Call:
                        break;
                    case MainMenuOperation.Volunteer:
                        IVolunteerSubMenu();
                        break;
                    case MainMenuOperation.Admin:
                        IAdminSubMenu();
                        break;
                }
            }
            catch (Exception ex)
            {
                ExceptionDisplay(ex);
            }

        } while (true);
    }

    private static void IAdminSubMenu()
    {
        do
        {
            try
            {
                //Get operation from user
                Console.Write(
                @"
----------------------------------------------------------------
Admin's SubMenu: Please Select One Of The Presented Options

Press 1: To Print The Current System Clock
Press 2: To Update System's Clock
Press 3: To Print The Current System Risk Range Value
Press 4: To Update System's System Risk Range
Press 5: To Reset The Database
Press 6: To Initialize The Database
----------------------------------------------------------------

>>> ");
                IAdminOperations operation;
                string input = Console.ReadLine() ?? "";

                if (!Enum.TryParse(input, out operation))
                    throw new BO.BlInvalidOperationException($"Bl: Operation {input}, is not available");

                switch (operation)
                {
                    case IAdminOperations.Exit:
                        return;
                    case IAdminOperations.GetClock:
                        ShowSystemClock();
                        break;
                    case IAdminOperations.UpdateClock:
                        UpdateSystemClock();
                        break;
                    case IAdminOperations.GetRiskRange:
                        ShowSystemRiskRange();
                        break;
                    case IAdminOperations.SetRiskRange:
                        UpdateSystemRiskRange();
                        break;
                    case IAdminOperations.DbReset:
                        ResetSystemDatabase();
                        break;
                    case IAdminOperations.DbInit:
                        InitializeSystemDatabase();
                        break;
                }
            }
            catch (Exception ex)
            {
                ExceptionDisplay(ex);
            }
        } while (true);
    }

    /// <summary>
    /// This method initializes the database with the premade values
    /// </summary>
    private static void InitializeSystemDatabase() => s_bl.Admin.DbInit();

    /// <summary>
    /// This method resets the database and the configuration values
    /// </summary>
    private static void ResetSystemDatabase() => s_bl.Admin.DbReset();

    /// <summary>
    /// This method updates the current risk range value by the value provided by the user
    /// </summary>
    /// <exception cref="BO.BlInputValueUnConvertableException"></exception>
    private static void UpdateSystemRiskRange()
    {
        Console.Write("Enter the new Risk Range (in this format: DD:HH:MM:SS): ");
        string input = Console.ReadLine() ?? "";
        if (!TimeSpan.TryParse(input, out TimeSpan newRiskRange))
            throw new BO.BlInputValueUnConvertableException($"Bl: The value {input}, is not a valid TimeSpan value");

        s_bl.Admin.SetRiskRange(newRiskRange);
    }

    /// <summary>
    /// This method displays the current risk range value
    /// </summary>
    private static void ShowSystemRiskRange() => Console.WriteLine($"Current System RiskRange: {s_bl.Admin.GetRiskRange()}");

    /// <summary>
    /// This method updates the current system clock by one unit according the provided Time Unit type by the user
    /// </summary>
    /// <exception cref="BO.BlInputValueUnConvertableException"></exception>
    private static void UpdateSystemClock()
    {
        Console.Write(
 @$"
    ------------------------------------------------------------------------------
    Please Select the Time Unit that You are willing to forward to time with:
    Press 1: To Forward By One {TimeUnit.Second}
    Press 2: To Forward By One {TimeUnit.Minute}
    Press 3: To Forward By One {TimeUnit.Hour}
    Press 4: To Forward By One {TimeUnit.Day}
    Press 5: To Forward By One {TimeUnit.Week}
    Press 6: To Forward By One {TimeUnit.Month}
    Press 7: To Forward By One {TimeUnit.Year}
    ------------------------------------------------------------------------------
    ");
        string input = Console.ReadLine() ?? "";
        if (!Enum.TryParse(input, out TimeUnit option))
            throw new BO.BlInputValueUnConvertableException($"Bl: The value {input}, is not a vaid IAdminOperations value");
        s_bl.Admin.UpdateClock(option);
    }

    /// <summary>
    /// This method displays the current system clock
    /// </summary>
    private static void ShowSystemClock() => Console.WriteLine($"Current System Clock: {s_bl.Admin.GetClock()}");

    public void ICallSubMenu()
    {
        Console.WriteLine(@"
----------------------------------------------------------------
Select Your Option:
Press 1 To Exit
Press 2 To AddCall
Press 3 To UpdateCall
Press 4 To SelectCallToDo
Press 5 To UpdateCallEnd
Press 6 To EndOfCallStatusUpdate
Press 7 To DeleteCallRequest
Press 8 To GetListOfCalls
Press 9 To GetDetielsOfCall
Press 10 To GetClosedCallsByVolunteer
Press 11 To GetOpenCallsForVolunteer
Press 12 To GetTotalCallsByStatus

----------------------------------------------------------------                
");
        Console.Write(">>> ");
        string input;
        CallMenuOperation Calloperation;
        do
        {
            input = Console.ReadLine() ?? "";
            if (!Enum.TryParse(input, out Calloperation))
                throw new BO.BlInputValueUnConvertableException($"Bl: Enum value for the main window is not a valid operation");
            switch (Calloperation)
            {
                case CallMenuOperation.Exit:
                    return;
                case CallMenuOperation.AddCall:

                    BO.CallType callType;
                    string CallAddress;
                    string Description;
                    DateTime DeadLine;
                    Console.WriteLine("Pls enter the type of the call:");
                    input = Console.ReadLine() ?? "";
                    if (!Enum.TryParse(input, out callType))
                    {
                        throw new BO.BlInputValueUnConvertableException($"Bl: Enum value for the main window is not a valid operation");
                    }
                    Console.WriteLine("Pls describe your call [optional]:");
                    Description = Console.ReadLine() ?? "";
                    Console.WriteLine("Pls enter the address of the call:");
                    CallAddress = Console.ReadLine() ?? "";
                    Console.WriteLine("What is the deadline for the call:");
                    DeadLine = DateTime.Parse(Console.ReadLine() ?? "");
                    if (DeadLine < s_bl.Admin.GetClock())
                    {
                        throw new BO.BlInputValueUnConvertableException($"Bl: The deadline for the call is invalid");
                    }
                    BO.Call call = new BO.Call()
                    {
                        TypeOfCall = callType,
                        Description = Description,
                        CallAddress = CallAddress,
                        CallStartTime = s_bl.Admin.GetClock(),
                        CallDeadLine = DeadLine
                    };
                    s_bl.Call.AddCall(call);
                    break;
                case CallMenuOperation.UpdateCall:
                    BO.CallType callType1;
                    string CallAddress1;
                    string Description1;
                    DateTime DeadLine1;
                    string answer;
                    Console.WriteLine("Please give me the call ID you want to update:");
                    int callId = int.Parse(Console.ReadLine() ?? "");
                    BO.Call call1 = s_bl.Call.GetDetielsOfCall(callId);
                    Console.WriteLine("do you want to change the type of the call: Y/N");
                    answer = Console.ReadLine() ?? "";
                    if (answer == "Y")
                    {
                        Console.WriteLine("Pls enter the type of the call:");
                        input = Console.ReadLine() ?? "";
                        if (!Enum.TryParse(input, out callType1))
                        {
                            throw new BO.BlInputValueUnConvertableException($"Bl: Enum value for the main window is not a valid operation");
                        }
                        call1.TypeOfCall = callType1;
                    }
                    Console.WriteLine("do you want to change the description of the call: Y/N");
                    answer = Console.ReadLine() ?? "";
                    if (answer == "Y")
                    {
                        Console.WriteLine("Pls describe your call [optional]:");
                        Description1 = Console.ReadLine() ?? "";
                        call1.Description = Description1;
                    }
                    Console.WriteLine("Do you want to change the call adress: Y/N");
                    answer = Console.ReadLine() ?? "";
                    if (answer == "Y")
                    {
                        Console.WriteLine("Pls enter the address of the call:");
                        CallAddress1 = Console.ReadLine() ?? "";
                        call1.CallAddress = CallAddress1;
                    }
                    Console.WriteLine("Do you want to change the deadline of the call: Y/N");
                    answer = Console.ReadLine() ?? "";
                    if (answer == "Y")
                    {
                        Console.WriteLine("What is the deadline for the call:");
                        DeadLine1 = DateTime.Parse(Console.ReadLine() ?? "");
                        if (DeadLine1 < s_bl.Admin.GetClock())
                        {
                            throw new BO.BlInputValueUnConvertableException($"Bl: The deadline for the call is invalid");
                        }
                        call1.CallDeadLine = DeadLine1;
                    }
                    s_bl.Call.UpdateCall(call1);
                    break;
                case CallMenuOperation.SelectCallToDo:
                    int callId1;
                    int VolunteerId;
                    Console.WriteLine("Please give me the call ID you want to select:");
                    callId1 = int.Parse(Console.ReadLine() ?? "");
                    Console.WriteLine("Please give me the volunteer ID you want to select:");
                    VolunteerId = int.Parse(Console.ReadLine() ?? "");
                    s_bl.Call.SelectCallToDo(VolunteerId, callId1);

                    break;
                case CallMenuOperation.UpdateCallEnd:
                    int callId2;
                    int VolunteerId1;
                    Console.WriteLine("Please give me the call ID you want to update:");
                    callId2 = int.Parse(Console.ReadLine() ?? "");
                    Console.WriteLine("Please give me the volunteer ID you want to update:");
                    VolunteerId1 = int.Parse(Console.ReadLine() ?? "");
                    s_bl.Call.UpdateCallEnd(VolunteerId1, callId2);
                    break;
                case CallMenuOperation.EndOfCallStatusUpdate:
                    int callId3;
                    int VolunteerId2;
                    Console.WriteLine("Please give me the call ID you want to update:");
                    callId3 = int.Parse(Console.ReadLine() ?? "");
                    Console.WriteLine("Please give me the volunteer ID you want to update:");
                    VolunteerId2 = int.Parse(Console.ReadLine() ?? "");

                    break;
                case CallMenuOperation.DeleteCallRequest:
                    break;
                case CallMenuOperation.GetListOfCalls:
                    break;
                case CallMenuOperation.GetDetielsOfCall:
                    break;
                case CallMenuOperation.GetClosedCallsByVolunteer:
                    break;
                case CallMenuOperation.GetOpenCallsForVolunteer:
                    break;
                case CallMenuOperation.GetTotalCallsByStatus:
                    break;
            }
        }
        while (true);
    }

    /// <summary>
    /// Volunteer's action hub which the user is able to preform all the IVolunteer action
    /// </summary>
    /// <exception cref="BO.BlInvalidOperationException"></exception>
    static public void IVolunteerSubMenu()
    {
        do
        {
            try
            {
                //Get operation from user
                Console.Write(
@"
----------------------------------------------------------------
Volunteer SubMenu: Please Select One Of The Presented Options

Press 1: To Add a New Volunteer
Press 2: To Remove an Volunteer
Press 3: To Update a Volunteer
Press 4: To Read a Volunteer
Press 5: To Read All Volunteers 
Press 6: To Login as a Volunteer
Press 0: To Exit
----------------------------------------------------------------

>>> ");
                IVolunteerOperations operation;
                string input = Console.ReadLine() ?? "";

                if (!Enum.TryParse(input, out operation))
                    throw new BO.BlInvalidOperationException($"Bl: Operation {input}, is not available");

                switch (operation)
                {
                    case IVolunteerOperations.Exit:
                        return;
                    case IVolunteerOperations.Add:
                        AddVolunteer();
                        break;
                    case IVolunteerOperations.Remove:
                        RemoveVolunteer();
                        break;
                    case IVolunteerOperations.Update:
                        UpdateVolunteer();
                        break;
                    case IVolunteerOperations.Read:
                        ReadVolunteer();
                        break;
                    case IVolunteerOperations.ReadAll:
                        ReadAllVolunteers();
                        break;
                    case IVolunteerOperations.Login:
                        LoginVolunteer();
                        break;
                }
            }
            catch (Exception ex)
            {
                ExceptionDisplay(ex);
            }
        } while (true);

    }

    /// <summary>
    /// This method adds a new Volunteer to the database using the user's input values
    /// </summary>
    private static void AddVolunteer()
    {
        int id;
        string fullName;
        string phoneNumber;
        string? email;
        string? password;
        string? fullCurrentAddress;
        BO.UserRole role;
        bool isActive;
        double maxDistanceToCall;
        BO.TypeOfRange rangeType;

        Console.Write("Enter Your Id: ");
        Int32.TryParse(Console.ReadLine() ?? "", out id);

        Console.Write("Enter Your Full Name: ");
        fullName = Console.ReadLine() ?? "";

        Console.Write("Enter Your Phone Number: ");
        phoneNumber = Console.ReadLine() ?? "";

        Console.Write("Enter Your Email Address: ");
        email = Console.ReadLine() ?? "";

        Console.Write("[Optional] Enter Your Password: ");
        password = Console.ReadLine() ?? null;

        Console.Write("[Optional] Enter Your Current Address: ");
        fullCurrentAddress = Console.ReadLine() ?? null;

        Console.Write($"Enter Your User Mode ({BO.UserRole.Volunteer} / {BO.UserRole.Admin}): ");
        Enum.TryParse(Console.ReadLine() ?? "", out role);

        Console.Write("Do You Want To Be an Active Volunteer? (yes / no)\n>>> ");
        bool.TryParse(Console.ReadLine() ?? "", out isActive);

        Console.Write("Enter Your Max Distance For Taking a Call: ");
        Double.TryParse(Console.ReadLine() ?? "", out maxDistanceToCall);

        Console.Write($"Enter Your Distance Range Type ({BO.TypeOfRange.AirDistance} / {BO.TypeOfRange.WalkingDistance} / {BO.TypeOfRange.DrivingDistance}): ");
        Enum.TryParse(Console.ReadLine() ?? "", out rangeType);

        s_bl.Volunteer.AddVolunteer(new BO.Volunteer
        {
            Id = id,
            Email = email,
            CurrentCall = null,
            FullCurrentAddress = fullCurrentAddress,
            FullName = fullName,
            IsActive = isActive,
            Latitude = null,
            Longitude = null,
            MaxDistanceToCall = maxDistanceToCall,
            Password = password,
            PhoneNumber = phoneNumber,
            RangeType = rangeType,
            Role = role
        });
    }

    /// <summary>
    /// This method requests from the user an volunteer's id and removes it from the database
    /// </summary>
    /// <exception cref="BO.BlInputValueUnConvertableException"></exception>
    private static void RemoveVolunteer()
    {
        Console.Write("Enter the Id of the volunteer to remove: ");
        string input = Console.ReadLine() ?? "";
        if (!Int32.TryParse(input, out int id))
            throw new BO.BlInputValueUnConvertableException($"Bl: The value {input}, is not a integer");
        else
            s_bl.Volunteer.DeleteVolunteer(id);
    }

    /// <summary>
    /// This method requests from the user new values for the modified volunteer entity together with the actor Id value which wants to preform the modification
    /// </summary>
    /// <exception cref="BO.BlInputValueUnConvertableException"></exception>
    private static void UpdateVolunteer()
    {
        int updaterId;
        int volunteerId;
        int newVolunteerId = -1;
        bool isPasswordBeenModifed = false;
        Console.WriteLine("Volunteer Update Mode:");
        Console.Write("Enter Your Id: ");
        Int32.TryParse(Console.ReadLine() ?? "", out updaterId);

        Console.WriteLine("Volunteer's New Field Values:");

        Console.Write("Enter the Id of the Volunteer that you are willing to modify: ");
        Int32.TryParse(Console.ReadLine() ?? "", out volunteerId);

        BO.Volunteer currentVolunteer = s_bl.Volunteer.GetVolunteerDetails(volunteerId);

        //Get new name
        if (RequestPremissionToChanged("Full Name"))
        {
            Console.Write("Enter new Full Name: ");
            currentVolunteer.FullName = Console.ReadLine() ?? "";
        }

        //Get new Phone number
        if (RequestPremissionToChanged("Phone Number"))
        {
            Console.Write("Enter new Phone Number: ");
            currentVolunteer.PhoneNumber = Console.ReadLine() ?? "";
        }

        //Get new email address
        if (RequestPremissionToChanged("Email Address"))
        {
            Console.Write("Enter new Email Address: ");
            currentVolunteer.Email = Console.ReadLine() ?? "";
        }

        //Get new password
        if (RequestPremissionToChanged("Password"))
        {
            isPasswordBeenModifed = true;
            Console.Write("Enter new Password (If you don't want any password, just hit enter): ");
            string? oldPassword = currentVolunteer.Password;
            currentVolunteer.Password = Console.ReadLine();
        }

        //Get new address
        if (RequestPremissionToChanged("Current Address"))
        {
            Console.Write("Enter new Current Address (If you don't want any password, just hit enter): ");
            currentVolunteer.FullCurrentAddress = Console.ReadLine();
        }

        //Get new user role
        if (RequestPremissionToChanged("User Role"))
        {
            string input;
            Console.Write($"Enter new User Role ({UserRole.Volunteer}, {UserRole.Admin}): ");
            input = Console.ReadLine() ?? "";
            if (!Enum.TryParse(input, out UserRole userRole))
                throw new BO.BlInputValueUnConvertableException($"Bl: There is no such a role as {input}");
            else
                currentVolunteer.Role = userRole;
        }

        //Get new active status
        if (RequestPremissionToChanged("Is Active"))
        {
            currentVolunteer.IsActive = RequestBooleanAnswerFromUser("Do you wanna be Active? (yes / no): ");
        }

        //Get new max distance for call 
        if (RequestPremissionToChanged("Max Distance for Accepting a Call"))
        {
            string? input;
            Console.Write($"Enter new MaxDistance (If you don't want any password, just hit enter): ");
            input = Console.ReadLine() ?? null;
            if (input == null)
                currentVolunteer.MaxDistanceToCall = null;
            else
            {
                if (!double.TryParse(input, out double res))
                    throw new BO.BlInputValueUnConvertableException($"Bl: Unable to convert user input. The value {input}, is not a double");
                else
                    currentVolunteer.MaxDistanceToCall = res;
            }
        }

        //Get new range type
        if (RequestPremissionToChanged("Type of Range"))
        {
            string input;
            Console.Write($"Enter new RangeType ({TypeOfRange.AirDistance}, {TypeOfRange.WalkingDistance}, {TypeOfRange.DrivingDistance}): ");
            input = Console.ReadLine() ?? "";
            if (!Enum.TryParse(input, out TypeOfRange res))
                throw new BO.BlInputValueUnConvertableException($"Bl: Unable to convert user input. The value {input}, is not either {TypeOfRange.AirDistance}, {TypeOfRange.WalkingDistance} or {TypeOfRange.DrivingDistance}");
            else
                currentVolunteer.RangeType = res;

        }

        BO.Volunteer newVolunteer = new BO.Volunteer
        {
            Id = (newVolunteerId == -1) ? currentVolunteer.Id : newVolunteerId,
            CurrentCall = currentVolunteer.CurrentCall,
            Email = currentVolunteer.Email,
            FullCurrentAddress = currentVolunteer.FullCurrentAddress,
            FullName = currentVolunteer.FullName,
            IsActive = currentVolunteer.IsActive,
            Latitude = null,
            Longitude = null,
            MaxDistanceToCall = currentVolunteer.MaxDistanceToCall,
            Password = currentVolunteer.Password,
            PhoneNumber = currentVolunteer.PhoneNumber,
            RangeType = currentVolunteer.RangeType,
            Role = currentVolunteer.Role
        };

        s_bl.Volunteer.UpdateVolunteerDetails(updaterId, newVolunteer, isPasswordBeenModifed);

    }

    /// <summary>
    /// This method requests from the user an id and prints out all the Volunteer's field values
    /// </summary>
    /// <exception cref="BO.BlInputValueUnConvertableException"></exception>
    private static void ReadVolunteer()
    {
        Console.Write("Enter the Id of the volunteer to read: ");
        string input = Console.ReadLine() ?? "";
        if (!Int32.TryParse(input, out int id))
            throw new BO.BlInputValueUnConvertableException($"Bl: The value {input}, is not a integer");
        else
            Console.WriteLine(s_bl.Volunteer.GetVolunteerDetails(id));
    }

    /// <summary>
    /// This method prints out all the Volunteer's that the database contains
    /// </summary>
    private static void ReadAllVolunteers()
    {
        Console.WriteLine("Displaying all the volunteers");
        foreach (BO.VolunteerInList volunteer in s_bl.Volunteer.GetVolunteers(null, null))
        {
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine(volunteer);
            Console.WriteLine("-------------------------------------------");
        }
    }

    /// <summary>
    /// This method logs into the account with the givne details
    /// </summary>
    private static void LoginVolunteer()
    {
        Console.Write("Enter Your Username (Email Address): ");
        string emailAddress = Console.ReadLine() ?? "";
        Console.Write("Enter Your Password: ");
        string password = Console.ReadLine() ?? "";

        string userType = s_bl.Volunteer.Login(emailAddress, password);
        Console.WriteLine($"The account under the email address of: {emailAddress} is a {userType}");
    }

    /// <summary>
    /// An help method for waiting for the user to prsss enter to continue
    /// </summary>
    /// <param name="ex"></param>
    static public void ExceptionDisplay(Exception ex)
    {
        Console.WriteLine(ex.Message);
        Console.WriteLine("Press enter to continue");
        var tmp = Console.ReadLine();
    }

    /// <summary>
    /// An help method which requests from the user whether he wants to modify the given field value
    /// </summary>
    /// <param name="valueToRequest"></param>
    /// <returns>The user's answer (yes = true, no = false)</returns>
    static public bool RequestPremissionToChanged(string valueToRequest)
        => RequestBooleanAnswerFromUser($"Do You Want to Update The {valueToRequest}? (yes / no) ");

    /// <summary>
    /// An help method which handles boolean input from the user
    /// </summary>
    /// <param name="msg">The display presented to the user</param>
    /// <returns>The user's answer (yes = true, no = false)</returns>
    static private bool RequestBooleanAnswerFromUser(string msg)
    {
        string input;
        do
        {
            Console.WriteLine(msg);
            input = Console.ReadLine() ?? "";
            if (input != "yes" && input != "no")
                Console.WriteLine($"Please Choose Either 'yes' or 'no'");
            else
                break;
        } while (true);
        return input == "yes";
    }



}



