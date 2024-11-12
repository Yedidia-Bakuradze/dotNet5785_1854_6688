using Dal;
using DalApi;
using DO;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;

namespace DalTest
{
    internal class Program
    {
        private static IAssignment? s_dalAssignment = new AssignmentImplementation();
        private static IVolunteer? s_dalVolunteer = new VolunteerImplementation();
        private static ICall? s_dalCall = new CallImplementation();
        private static IConfig? s_dalConfig = new ConfigImplementation();

        public enum MainMenuEnum { FirstRun, Exit, ShowAssignmentMenu, ShowCallMenu, ShowVolunteerMenu, DbInit, ShowAllDbData, ShowConfigMenu, ResetSysAndDb }
        public enum ClassSubMenuEnum { FirstRun, Exit, Create, Read, ReadAll, Update, Delete, DeleteAll }
        public enum ConfigSubMenuEnum { FirstRun, Exit,AddMinute ,AddHour,AddMonth , AddYear, ShowCureentClock,SetValue,ShowValue,Reset }
        public enum ClassType { Assignment, Call, Volunteer 
        public enum ConfigVarieble {FirstRun,RiskRange,Clock }
        static void Main(string[] args)
        {
            MainMenu();
        }

        /// <summary>
        /// Displays the main menu and handles user input.
        /// </summary>
        private static void MainMenu()
        {
            MainMenuEnum operation = MainMenuEnum.FirstRun;
            while (operation != MainMenuEnum.Exit)
            {
                try
                {
                    Console.Write("Please choose an option: ");
                    string input = Console.ReadLine() ?? "";
                    if (Enum.TryParse(input, out operation))
                    {
                        switch (operation)
                        {
                            case MainMenuEnum.Exit:
                                Console.WriteLine("Leaving the lobby ... ");
                                return;
                            case MainMenuEnum.ShowAssignmentMenu:
                                ShowSubMenu(ClassType.Assignment);
                                break;
                            case MainMenuEnum.ShowCallMenu:
                                ShowSubMenu(ClassType.Call);
                                break;
                            case MainMenuEnum.ShowVolunteerMenu:
                                ShowSubMenu(ClassType.Volunteer);
                                break;
                            case MainMenuEnum.DbInit:
                                DbInit();
                                break;
                            case MainMenuEnum.ShowAllDbData:
                                ShowDbData();
                                break;
                            case MainMenuEnum.ShowConfigMenu:
                                ConfigSubMenu(s_dalConfig.RiskRange);
                                break;
                            case MainMenuEnum.ResetSysAndDb:
                                //ResetDbAndSystem();
                                break;
                            default:
                                Console.WriteLine("Invalid operation!");
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"{input} is not a valid operation! Please choose a valid operation.");
                    }

                }
                catch (Exception error)
                {
                    Console.WriteLine(error.Message);
                }

            }
        }
        /// <summary>
        /// Provides a submenu for configuring application settings. Users can adjust time parameters, view or change specific configurations,
        /// and reset all settings. The function loops until the "Exit" option is selected.
        /// i used chatgpt for the comments here.
        /// </summary>
        private static void ConfigSubMenu()
        {
            ConfigSubMenuEnum configSubMenu = ConfigSubMenuEnum.FirstRun;
            while (configSubMenu != ConfigSubMenuEnum.Exit)
            {
                try
                {
                    Console.Write("Please choose an option: ");
                    string input = Console.ReadLine() ?? "";

                    // Parse the user input to the enum, representing the chosen configuration option
                    if (Enum.TryParse(input, out configSubMenu))
                    {
                        switch (configSubMenu)
                        {
                            case ConfigSubMenuEnum.Exit:
                                // Exits the configuration submenu
                                Console.WriteLine("Leaving the lobby ... ");
                                return;
                            case ConfigSubMenuEnum.AddMinute:
                                // Adds one minute to the current clock setting
                                s_dalConfig?.Clock.AddMinutes(1);
                                break;
                            case ConfigSubMenuEnum.AddHour:
                                // Adds one hour to the current clock setting
                                s_dalConfig?.Clock.AddHours(1);
                                break;
                            case ConfigSubMenuEnum.AddMonth:
                                // Adds one month to the current clock setting
                                s_dalConfig?.Clock.AddMonths(1);
                                break;
                            case ConfigSubMenuEnum.AddYear:
                                // Adds one year to the current clock setting
                                s_dalConfig?.Clock.AddYears(1);
                                break;
                            case ConfigSubMenuEnum.ShowCureentClock:
                                // Displays the current clock setting
                                Console.WriteLine(s_dalConfig?.Clock);
                                break;
                            case ConfigSubMenuEnum.SetValue:
                                // Allows user to set a new value for either RiskRange or Clock
                                ConfigVarieble configVarieble = ConfigVarieble.FirstRun;
                                Console.WriteLine("What variable do you want to change (RiskRange/Clock)?");
                                string input1 = Console.ReadLine() ?? "";
                                Enum.TryParse(input1, out configVarieble);

                                switch (configVarieble)
                                {
                                    case ConfigVarieble.RiskRange:
                                        // Sets a new value for RiskRange
                                        Console.WriteLine("Enter the new value: ");
                                        TimeSpan timeSpan = TimeSpan.Parse(Console.ReadLine() ?? "");
                                        s_dalConfig!.RiskRange = timeSpan;
                                        break;
                                    case ConfigVarieble.Clock:
                                        // Sets a new value for Clock
                                        Console.WriteLine("Enter the new value: ");
                                        DateTime clock = DateTime.Parse(Console.ReadLine() ?? "");
                                        s_dalConfig!.Clock = clock;
                                        break;
                                    default:
                                        Console.WriteLine("Invalid value");
                                        break;
                                }
                                break;
                            case ConfigSubMenuEnum.ShowValue:
                                // Displays the current value for either RiskRange or Clock
                                ConfigVarieble configVarieble1 = ConfigVarieble.FirstRun;
                                Console.WriteLine("What variable do you want to view (RiskRange/Clock)?");
                                string input2 = Console.ReadLine() ?? "";
                                Enum.TryParse(input2, out configVarieble1);

                                switch (configVarieble1)
                                {
                                    case ConfigVarieble.RiskRange:
                                        // Shows the current RiskRange value
                                        Console.WriteLine(s_dalConfig?.RiskRange);
                                        break;
                                    case ConfigVarieble.Clock:
                                        // Shows the current Clock value
                                        Console.WriteLine(s_dalConfig?.Clock);
                                        break;
                                    default:
                                        Console.WriteLine("Invalid value");
                                        break;
                                }
                                break;
                            case ConfigSubMenuEnum.Reset:
                                // Resets all settings to their default values
                                s_dalConfig?.Reset();
                                break;
                            default:
                                Console.WriteLine("Invalid operation!");
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"{input} is not a valid operation! Please choose a valid operation.");
                    }

                }
                catch (Exception error)
                {
                    // Handles any unexpected exceptions during the execution of submenu options
                    Console.WriteLine(error.Message);
                }
            }
        }


        /// <summary>
        /// Prints the data of all the entities in the database
        /// </summary>
        private static void ShowDbData()
        {
            try
            {
                List<Assignment> listOfAssignments = s_dalAssignment?.ReadAll() ?? throw new Exception("Error: The list of Assignment instances is null");
                List<Call> listOfCalls = s_dalCall?.ReadAll() ?? throw new Exception("Error: The list of Call instances is null");
                List<Volunteer> listOfVolunteers = s_dalVolunteer?.ReadAll() ?? throw new Exception("Error: The list of Volunteer instances is null");

                Console.WriteLine("Assignment Entities: ");
                Console.Beep();
                foreach(Assignment assignment in listOfAssignments)
                {
                    Console.WriteLine($@"
                    ---------------------------------------------------------------
                    
                        Assignment ID: {assignment.Id}
                        Related Call ID: {assignment.Called}
                        Related Volunteer ID: {assignment.VolunteerId}
                        Start Time: {assignment.TimeOfStarting}
                        Finish Time: {assignment.TimeOfEnding}
                        Closed Call Type: {assignment.TypeOfEnding} 

                    ---------------------------------------------------------------
                    >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

                    ");

                }

                foreach(Call call in listOfCalls)
                {
                    Console.WriteLine($@"
                    ---------------------------------------------------------------
                    
                        Call ID: {call.Id}
                        Request Address: {call.FullAddressCall}
                        Request Address Latitude: {call.Latitude}
                        Request Address Longitude: {call.Longitude}
                        Call Description: {call.Description}
                        The Call Has Been Opened Since: {call.OpeningTime}
                        The Call Would Be Expired At: {call.DeadLine}

                    ---------------------------------------------------------------
                    >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

                    ");
                }

                foreach(Volunteer volunteer  in listOfVolunteers)
                {
                    Console.WriteLine($@"
                    ---------------------------------------------------------------
                    
                        Volunteer ID: {volunteer.Id}
                        Volunteer Role: {volunteer.Role}
                        Full Name: {volunteer.FullName}
                        Email Address: {volunteer.Email}
                        Max Distance From The Call: {volunteer.MaxDistanceToCall}
                        Type Of Distance Range: {volunteer.TypeOfRange}
                        Active: {volunteer.Active}
                        Password: {volunteer.Password}
                        Living Address: {volunteer.FullCurrentAddress}
                        Living Address Latitude: {volunteer.Latitude}
                        Living Address Longitude: {volunteer.Longitude}

                    ---------------------------------------------------------------
                    >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

                    ");
                }
            }catch(Exception error)
            {
                Console.WriteLine(error.Message);
            }
        }

        /// <summary>
        /// Displays the submenu for a specific class and handles user input.
        /// </summary>
        /// <param name="classType">The class type for the submenu.</param>
        private static void ShowSubMenu(ClassType classType)
        {
            ClassSubMenuEnum operation = ClassSubMenuEnum.FirstRun;
            while (operation == ClassSubMenuEnum.Exit)
            {
                Console.Write(@"Please Choose The Operation That You Would Like To Use: ");
                string input = Console.ReadLine() ?? "";
                if (Enum.TryParse(input, out operation))
                {
                    try
                    {
                        switch (operation)
                        {
                            case ClassSubMenuEnum.Exit:
                                Console.WriteLine("Logging out of the submenu ...");
                                break;
                            case ClassSubMenuEnum.Create:
                                CreateDbAction(classType);
                                break;
                            case ClassSubMenuEnum.Read:
                                ReadDbAction(classType);
                                break;
                            case ClassSubMenuEnum.ReadAll:
                                //TODO: ReadAllDbAction(classId);
                                break;
                            case ClassSubMenuEnum.Update:
                                //TODO: UpdateDbAction(classId);
                                break;
                            case ClassSubMenuEnum.Delete:
                                //TODO: DeleteDbAction(classId);
                                break;
                            case ClassSubMenuEnum.DeleteAll:
                                //TODO: DeleteAllDbAction(classId);
                                break;
                        }
                    }
                    catch (Exception error)
                    {
                        Console.WriteLine(error.Message);
                    }
                }
                else
                {
                    Console.WriteLine($"Invalid operation has been captured: {input}");
                }
            }
        }

        /// <summary>
        /// This method requests the id value from the user, locates the instance if existed and prints its values
        /// </summary>
        /// <param name="classType">The type of instance which is requested, either Assignment, Call or Volunteer</param>
        private static void ReadDbAction(ClassType classType)
        {
            bool requestedPreformed = false;
            do
            {
                try
                {
                    //Get The instance id
                    Console.Write($"Please enter the id for object of type {classType}: ");
                    bool isValid = Int32.TryParse(Console.ReadLine(), out int id);
                    while (!isValid)
                    {
                        Console.WriteLine("Please choose a valid number for the id.");
                        Console.Write($"Please enter the id for object of type {classType}: ");
                        isValid = Int32.TryParse(Console.ReadLine(), out id);
                    }
                    
                    //Locate the instance based on its type and id
                    switch (classType)
                    {
                        case ClassType.Assignment:
                            {
                                Assignment result = s_dalAssignment?.Read(id)!
                                    ?? throw new Exception($"The Assignment instance with id of {id} hasn't been found");
                                Console.WriteLine(result);
                                break;
                            }

                        case ClassType.Call:
                            {
                                Call result = s_dalCall?.Read(id)!
                                    ?? throw new Exception($"The Call instance with id of {id} hasn't been found");
                                Console.WriteLine(result);
                                break;
                            }
                        case ClassType.Volunteer:
                            {
                                Volunteer result = s_dalVolunteer?.Read(id)!
                                    ?? throw new Exception($"The Volunteer instance with id of {id} hasn't been found");
                                Console.WriteLine(result);
                                break;
                            }
                        default:
                            throw new Exception($"Internal Error: Class Type is {classType}. Is not a Assignment, Call or Volunteer");
                    }
                    
                    //If no exception has been thrown, the operation succussed.
                    requestedPreformed = true;
                }
                catch (Exception error)
                {
                    Console.WriteLine(error.Message);
                    Console.Beep();
                    Console.WriteLine("Press Enter to continue ... ");
                    Console.ReadKey();
                }
            } while (!requestedPreformed);
        }


        /// <summary>
        /// Performs the create action for a specific class.
        /// </summary>
        /// <param name="classType">The class type for the create action.</param>
        private static void CreateDbAction(ClassType classType)
        {
            //Getting data for the new instance
            switch (classType)
            {
                case ClassType.Assignment:
                    {
                        //TODO: Callid for the assignment, i dont understood that and i will ask my teacher in the next lecture

                        // Prompt for the id of the volunteer
                        Console.WriteLine("What is the id of the volunteer:");
                        int volunteerid = int.Parse(Console.ReadLine() ?? "");

                        // Prompt for the time of starting
                        Console.WriteLine("What is the time of starting:");
                        DateTime start = DateTime.Parse(Console.ReadLine() ?? "");

                        // Prompt for the time of ending
                        Console.WriteLine("What is the time of ending ");
                        DateTime end = DateTime.Parse(Console.ReadLine() ?? "");

                        // Prompt for the cause of ending
                        Console.WriteLine("What is the cause of ending:");
                        string input = Console.ReadLine() ?? "";
                        bool isValid = Enum.TryParse(input, out TypeOfEnding typeOfEnding);
                        Assignment newAsignment = new Assignment
                        {
                            VolunteerId = volunteerid,
                            TimeOfStarting = start,
                            TimeOfEnding = end,
                            TypeOfEnding = typeOfEnding,
                        };
                        if (s_dalAssignment.Read(newAsignment.Id) == null)
                        {
                            s_dalAssignment.Create(newAsignment);
                        }
                    }
                    break;
                case ClassType.Call:
                    {
                        // Prompt for the call type
                        Console.WriteLine("What is the call type:");
                        string input = Console.ReadLine() ?? "";
                        bool isValid = Enum.TryParse(input, out CallTypes callType);

                        // Prompt for the address
                        Console.WriteLine("What is the address");
                        string address = Console.ReadLine() ?? "";

                        // Prompt for the latitude of the address
                        Console.WriteLine("What is the latitude of the address:");
                        double latitude = double.Parse(Console.ReadLine() ?? "");

                        // Prompt for the longitude of the address
                        Console.WriteLine("What is the longitude of the address:");
                        double longitude = double.Parse(Console.ReadLine() ?? "");

                        // Prompt for the time the call starts
                        Console.WriteLine("What time does the call start:");
                        DateTime start = DateTime.Parse(Console.ReadLine() ?? "");

                        // Prompt for the description of the call
                        Console.WriteLine("Please add a description to the call:");
                        string description = Console.ReadLine() ?? "";

                        // Prompt for the deadline of the call
                        Console.WriteLine("What is the deadline for the call:");
                        DateTime deadLine = DateTime.Parse(Console.ReadLine() ?? "");

                        Call newcall = new Call
                        {
                            Type = callType,
                            FullAddressCall = address,
                            Latitude = latitude,
                            Longitude = longitude,
                            OpeningTime = start,
                            Description = description,
                            DeadLine = deadLine
                        };
                        if (s_dalCall.Read(newcall.Id) == null)
                        {
                            s_dalCall.Create(newcall);
                        }
                        break;
                    }
                case ClassType.Volunteer:
                    {
                        // Prompt for the volunteer id
                        Console.WriteLine("Enter the volunteer id:");
                        int id = int.Parse(Console.ReadLine() ?? "");

                        // Prompt for the volunteer full name
                        Console.WriteLine("Enter the volunteer full name:");
                        string name = Console.ReadLine() ?? "";

                        // Prompt for the role of the volunteer
                        Console.WriteLine("Enter the Role of the Volunteer (Admin/Volunteer):");
                        string input = Console.ReadLine() ?? "";
                        bool isValid = Enum.TryParse(input, out Roles role);

                        // Prompt for the volunteer phone number
                        Console.WriteLine("Enter the Volunteer phone number:");
                        string phoneNumber = Console.ReadLine() ?? "";

                        // Prompt for the volunteer email
                        Console.WriteLine("Enter the Volunteer email:");
                        string email = Console.ReadLine() ?? "";

                        // Prompt for the max distance to call the volunteer
                        Console.WriteLine("What is the max distance to call the volunteer:");
                        double maxDistance = double.Parse(Console.ReadLine() ?? "");

                        // Prompt for the volunteer's active status
                        Console.WriteLine("Is the volunteer active? (true/false)");
                        bool active = bool.Parse(Console.ReadLine() ?? "");

                        // Prompt for the volunteer's password
                        Console.WriteLine("Enter the volunteer's password:");
                        string password = Console.ReadLine() ?? "";

                        // Prompt for the volunteer's full current address
                        Console.WriteLine("Enter the volunteer's full current address:");
                        string address = Console.ReadLine() ?? "";

                        // Prompt for the type of range
                        Console.WriteLine("Enter the type of range (e.g., Local, Regional, National):");
                        string input1 = Console.ReadLine() ?? "";
                        isValid = Enum.TryParse(input1, out TypeOfRange typeOfRange);

                        Volunteer newVolunteer = new Volunteer
                        {
                            Id = id,
                            FullName = name,
                            Role = role,
                            PhoneNumber = phoneNumber,
                            Email = email,
                            MaxDistanceToCall = maxDistance,
                            Active = active,
                            Password = password,
                            FullCurrentAddress = address,
                            TypeOfRange = typeOfRange,
                        };
                        if (s_dalVolunteer?.Read(newVolunteer.Id) == null)
                        {
                            s_dalVolunteer?.Create(newVolunteer);
                        }

                        break;
                    }
            }
        }

        private static void DbInit() => DalTest.Initialization.Do(s_dalAssignment, s_dalCall, s_dalVolunteer, s_dalConfig);

    }
}
