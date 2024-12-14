using BO;
using Helpers;
using System.Data.Common;
using System.Security.Cryptography.X509Certificates;

namespace BlTest;

internal class Program
{
    //Main BL Action manager for the bl layer
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    public enum MainMenuOperation { Exit, Call, Volunteer, Admin }
    public enum CallMenuOperation { Exit, AddCall, UpdateCall, SelectCallToDo, UpdateCallEnd, EndOfCallStatusUpdate, DeleteCallRequest, GetListOfCalls, GetDetielsOfCall, GetClosedCallsByVolunteer, GetOpenCallsForVolunteer, GetTotalCallsByStatus }
    public enum IVolunteerOperations { Exit, Add, Remove, Update, Read, ReadAll, Login}
    static void Main(string[] args)
    {
        MainMenu();
    }
    static public void MainMenu()
    {
        do
        {
            Console.WriteLine(@"
    ----------------------------------------------------------------
    Select Your Option:

    Press 1 To Use ICall Interface
    Press 2 To Use IVolunteer Interface
    Press 3 To Use IAdmin Interface
    Press 0 To Exit
    ----------------------------------------------------------------
    ");
            Console.Write(">>> ");
            string input;
            MainMenuOperation operation;
            input = Console.ReadLine() ?? "";
            if (!Enum.TryParse(input, out operation))
                throw new BO.BlInvalidEnumValueOperationException($"Bl: Enum value for the main window is not a valid operation");

            try
            {
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
                        break;
                }
            }
            catch (Exception ex)
            {
                ExceptionDisplay(ex);
            }

        } while (true);
    }

    public void ICallSubMenu()
    {

    }






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

>>> "
    );
                IVolunteerOperations operation;
                string input = Console.ReadLine() ?? "";

                if (!Enum.TryParse(input, out operation))
                    throw new BO.BlInvalidEnumValueOperationException($"Bl: Operation {input}, is not available");

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
            currentVolunteer.Password= Console.ReadLine();
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
            else {
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
            FullCurrentAddress =currentVolunteer.FullCurrentAddress,
            FullName = currentVolunteer.FullName,
            IsActive = currentVolunteer.IsActive,
            Latitude = null,
            Longitude = null,
            MaxDistanceToCall = currentVolunteer.MaxDistanceToCall,
            Password = currentVolunteer.Password,
            PhoneNumber =currentVolunteer.PhoneNumber,
            RangeType = currentVolunteer.RangeType,
            Role = currentVolunteer.Role
        };

        s_bl.Volunteer.UpdateVolunteerDetails(updaterId, newVolunteer,isPasswordBeenModifed);

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
    private static void LoginVolunteer()
    {
        Console.Write("Enter Your Username (Email Address): ");
        string emailAddress = Console.ReadLine() ?? "";
        Console.Write("Enter Your Password: ");
        string password = Console.ReadLine() ?? "";

        string userType = s_bl.Volunteer.Login(emailAddress, password);
        Console.WriteLine($"The account under the email address of: {emailAddress} is a {userType}");
    }

    static public void ExceptionDisplay(Exception ex)
    {
        Console.WriteLine(ex.Message);
        Console.WriteLine("To Continue: Please Press Enter");
        Console.Read();
    }

    static public bool RequestPremissionToChanged(string valueToRequest)
    => RequestBooleanAnswerFromUser($"Do You Want to Update The {valueToRequest}? (yes / no) ");
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


