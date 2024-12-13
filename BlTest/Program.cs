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
                Console.WriteLine(
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

        >>> 
        "
    );
                IVolunteerOperations operation;
                string input = Console.ReadLine() ?? "";

                if (!Enum.TryParse(input, out operation))
                    throw new BO.BlInvalidEnumValueOperationException($"Bl: There is not such a opereation as {input}");

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

    }
    private static void RemoveVolunteer()
    {
        Console.Write("Enter the Id of the volunteer to remove: ");
        string input = Console.ReadLine() ?? "";
        if (!Int32.TryParse(input, out int id))
            throw new BO.BlInvalidValueTypeToFormatException($"Bl: The value {input}, is not a integer");
        else
            s_bl.Volunteer.DeleteVolunteer(id);
    }
    private static void UpdateVolunteer()
    {
        int updaterId;
        int id;
        Console.WriteLine("Volunteer Update Mode:");
        Console.Write("Enter Your Id: ");
        Int32.TryParse(Console.ReadLine() ?? "", out updaterId);

        Console.WriteLine("Volunteer's New Field Values:");

        Console.Write("Enter new Id: ");
        Int32.TryParse(Console.ReadLine() ?? "", out id);

        BO.Volunteer currentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);

        if (RequestPremissionToChanged("Full Name"))
        {
            Console.Write("Enter new Full Name: ");
            currentVolunteer.FullName = Console.ReadLine() ?? "";
        }

        if (RequestPremissionToChanged("Phone Number"))
        {
            Console.Write("Enter new Phone Number: ");
            currentVolunteer.PhoneNumber = Console.ReadLine() ?? "";
        }

        if (RequestPremissionToChanged("Email Address"))
        {
            Console.Write("Enter new Email Address: ");
            currentVolunteer.Email = Console.ReadLine() ?? "";
        }
    }
    private static void ReadVolunteer()
    {
        Console.Write("Enter the Id of the volunteer to read: ");
        string input = Console.ReadLine() ?? "";
        if (!Int32.TryParse(input, out int id))
            throw new BO.BlInvalidValueTypeToFormatException($"Bl: The value {input}, is not a integer");
        else
            s_bl.Volunteer.GetVolunteerDetails(id);
    }
    private static void ReadAllVolunteers()
    {
        Console.WriteLine("-------------------------------------------");
        Console.WriteLine("Displaying all the volunteers");
        foreach (BO.VolunteerInList volunteer in s_bl.Volunteer.GetVolunteers(null, null))
        {
            Console.WriteLine(volunteer);
        }
        Console.WriteLine("-------------------------------------------");
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
}


