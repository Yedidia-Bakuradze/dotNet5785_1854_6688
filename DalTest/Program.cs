using Dal;
using DalApi;
using DO;
using System.Runtime.CompilerServices;
using static DalTest.Program;

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
        public enum ConfigSubMenuEnum { FirstRun, Exit,AddMinute ,AddHour,AddMonth , AddYear, ShowCurrentClock,SetValue,ShowValue,Reset }
        public enum ClassType { Assignment, Call, Volunteer };
        public enum ConfigVariable { FirstRun,RiskRange,Clock }
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
                    Console.WriteLine(@"
---------------------------------------------------

Please choose an option -
- 1 To Exit
- 2 To Open The Assignment Menu
- 3 To Open The Call Menu
- 4 To Open The Volunteer Menu
- 5 To Initialize The Database
- 6 To Print All The Data From The Database
- 7 To Open The Configuration Menu
- 8 To Reset The System and The Database's Records

---------------------------------------------------
                    ");
                    Console.Write(">>> ");
                    string input = Console.ReadLine() ?? "";
                    
                    if (Enum.TryParse(input, out operation))
                    {
                        switch (operation)
                        {
                            case MainMenuEnum.Exit:
                                Console.WriteLine("Leaving The Main Hub ... ");
                                break;
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
                                ReadAllEntitiesByDb();
                                break;
                            case MainMenuEnum.ShowConfigMenu:
                                ConfigSubMenu();
                                break;
                            case MainMenuEnum.ResetSysAndDb:
                                ResetDbAndSystem();
                                break;
                            default:
                                Console.Beep();
                                Console.WriteLine($"{input} Is Not a Valid Operation For The Main Menu, Please Try Again!");
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"{input} Is Not a Valid Operation For The Main Menu, Please Try Again!");
                    }

                }
                catch (Exception error)
                {
                    Console.WriteLine(error.Message);
                }

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
                try
                {
                    Console.Write(@"Please Choose The Operation That You Would Like To Use: ");
                    string input = Console.ReadLine() ?? "";
                    if (Enum.TryParse(input, out operation))
                    {

                        switch (operation)
                        {
                            case ClassSubMenuEnum.Exit:
                                Console.WriteLine("Logging out of the submenu ...");
                                break;
                            case ClassSubMenuEnum.Create:
                                CreateDbEntity(classType);
                                break;
                            case ClassSubMenuEnum.Read:
                                ReadSingleDbEntity(classType);
                                break;
                            case ClassSubMenuEnum.ReadAll:
                                ReadAllEntitiesByType(classType);
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
                    else
                    {
                        Console.WriteLine($"Invalid operation has been captured: {input}");
                    }
                }catch(Exception error)
                {
                    Console.WriteLine(error.Message);
                }
            }
        }


        /// <summary>
        /// This method when call resets the system including the database records
        /// </summary>
        private static void ResetDbAndSystem()
        {
            s_dalAssignment?.DeleteAll();
            s_dalCall?.DeleteAll();
            s_dalVolunteer?.DeleteAll();
            s_dalConfig?.Reset();
        }
        

        /// <summary>
        /// Provides a submenu for configuring application settings. Users can adjust time parameters, view or change specific configurations,
        /// and reset all settings. The function loops until the "Exit" option is selected.
        /// i used Chat GPT for the comments here.
        /// </summary>
        private static void ConfigSubMenu()
        {
            ConfigSubMenuEnum operation = ConfigSubMenuEnum.FirstRun;
            while (operation != ConfigSubMenuEnum.Exit)
            {
                try
                {
                    Console.Write(@"
---------------------------------------------------------------------------------

    Please choose an option:

    Exit             - To Exit The Sub Menu
    AddMinute        - To Add Minutes To System Clock
    AddHour          - To Add Hours To System Clock
    AddMonth         - To Add Months To System Clock
    AddYear          - To Add Years To System Clock
    ShowCurrentClock - To Show Current Time
    SetValue         - To Set a Value to System Variables (RisRange, Clock)
    ShowValue        - To Display System Variables Values (RisRange, Clock)
    Reset            - To Reset System Variables Values (RisRange, Clock)

---------------------------------------------------------------------------------

");
                    Console.Write(">>> ");
                    string input = Console.ReadLine() ?? "";

                    // Parse the user input to the enum, representing the chosen configuration option
                    if (Enum.TryParse(input, out operation))
                    {
                        switch (operation)
                        {
                            case ConfigSubMenuEnum.Exit:
                                {
                                    // Exits the configuration sub-menu
                                    Console.WriteLine("Leaving the lobby ... ");
                                    break;
                                }
                            case ConfigSubMenuEnum.AddMinute:
                                {
                                    // Adds one minute to the current clock setting
                                    s_dalConfig?.Clock.AddMinutes(1);
                                    break;
                                }
                            case ConfigSubMenuEnum.AddHour:
                                {
                                    // Adds one hour to the current clock setting
                                    s_dalConfig?.Clock.AddHours(1);
                                    break;
                                }
                            case ConfigSubMenuEnum.AddMonth:
                                {
                                    // Adds one month to the current clock setting
                                    s_dalConfig?.Clock.AddMonths(1);
                                    break;
                                }
                            case ConfigSubMenuEnum.AddYear:
                                {
                                    // Adds one year to the current clock setting
                                    s_dalConfig?.Clock.AddYears(1);
                                    break;
                                }
                            case ConfigSubMenuEnum.ShowCurrentClock:
                                {
                                    // Displays the current clock setting
                                    Console.WriteLine(s_dalConfig?.Clock);
                                    break;
                                }
                            case ConfigSubMenuEnum.SetValue:
                                {
                                    // Allows user to set a new value for either RiskRange or Clock
                                    ConfigVariable configVariable = GetUsersConfigVariableChoice();
                                    switch (configVariable)
                                    {
                                        case ConfigVariable.RiskRange:
                                            // Sets a new value for RiskRange
                                            Console.WriteLine("Enter the new value: ");
                                            TimeSpan timeSpan = TimeSpan.Parse(Console.ReadLine() ?? "");
                                            s_dalConfig!.RiskRange = timeSpan;
                                            break;
                                        case ConfigVariable.Clock:
                                            // Sets a new value for Clock
                                            Console.WriteLine("Enter the new value: ");
                                            DateTime clock = DateTime.Parse(Console.ReadLine() ?? "");
                                            s_dalConfig!.Clock = clock;
                                            break;
                                        default:
                                            {
                                                throw new Exception($"Invalid configVariable: {configVariable} ");
                                            }
                                    }
                                    break;
                                }
                            case ConfigSubMenuEnum.ShowValue:
                                {
                                    ConfigVariable configVariable = GetUsersConfigVariableChoice();

                                    switch (configVariable)
                                    {
                                        case ConfigVariable.RiskRange:
                                            // Shows the current RiskRange value
                                            Console.WriteLine(s_dalConfig?.RiskRange);
                                            break;
                                        case ConfigVariable.Clock:
                                            // Shows the current Clock value
                                            Console.WriteLine(s_dalConfig?.Clock);
                                            break;
                                        default:
                                            {
                                                throw new Exception($"Invalid configVariable: {configVariable} ");
                                            }
                                    }
                                    break;
                                }
                            case ConfigSubMenuEnum.Reset:
                                {
                                    // Resets all settings to their default values
                                    s_dalConfig?.Reset();
                                    break;
                                }
                            default:
                                {
                                    throw new Exception($"Forbidden Operation: The {operation} operation is not allowed!");
                                }
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
        /// Given a Class Type value, this method will print all the records associated with the data type in the database
        /// </summary>
        /// <param name="classType">The data type requested from the database to be shown</param>
        private static void ReadAllEntitiesByType(ClassType classType)
        {
            try
            { 
                switch (classType)
                {
                    case ClassType.Assignment:
                        {
                            List<Assignment> listOfAssignments = s_dalAssignment?.ReadAll()
                                ?? throw new Exception("The Assignments in Database Are Not Available");
                            Console.WriteLine("Assignment Entities: ");
                            Console.Beep();
                            foreach (Assignment assignment in listOfAssignments)
                                PrintAssignmentEntity(assignment);
                            break;
                        }
                    case ClassType.Call:
                        {
                            List<Call> listOfCalls = s_dalCall?.ReadAll() ?? throw new Exception("The Calls in Database Are Not Available!");
                            Console.WriteLine("\nCall Entities: ");
                            foreach (Call call in listOfCalls)
                                PrintCallEntity(call);
                            break;
                        }
                    case ClassType.Volunteer:
                        {
                            List<Volunteer> listOfVolunteers = s_dalVolunteer?.ReadAll() ?? throw new Exception("Error: The list of Volunteer instances is null");
                            Console.WriteLine("\nVolunteer Entities: ");
                            Console.Beep();
                            foreach (Volunteer volunteer in listOfVolunteers)
                                PrintVolunteerEntity(volunteer);
                            break;
                        }
                    default:
                        {
                            throw new Exception($"Internal Error: {classType} Is Not a Valid Class Type");
                        }
                }
            }
            catch(Exception error)
            {
                Console.WriteLine(error.Message);
            }

        }


        /// <summary>
        /// Prints the data of all the entities in the database
        /// </summary>
        private static void ReadAllEntitiesByDb()
        {
            ReadAllEntitiesByType(ClassType.Assignment);
            ReadAllEntitiesByType(ClassType.Call);
            ReadAllEntitiesByType(ClassType.Volunteer);
        }
        

        /// <summary>
        /// This method requests the id value from the user, locates the instance if existed and prints its values
        /// </summary>
        /// <param name="classType">The type of instance which is requested, either Assignment, Call or Volunteer</param>
        private static void ReadSingleDbEntity(ClassType classType)
        {
            bool requestCompleted = false;
            int requestedObjectId;
            bool isValid;
            do
            {
                try
                {
                    //Get Entity ID
                    do
                    {
                        isValid = !Int32.TryParse(Console.ReadLine(), out requestedObjectId);
                        if (!isValid)
                        {
                            Console.WriteLine("Invalid Input Value! Please Enter Only Natural Numbers");
                        }
                        Console.WriteLine($"Enter Id Value For Object Of Type {classType}");
                        Console.Write(">>> ");

                    } while (!isValid);

                    //Fetch Object By ID and Print Out Its Fields
                    switch (classType)
                    {
                        case ClassType.Assignment:
                            {
                                Assignment result = s_dalAssignment?.Read(requestedObjectId)!
                                    ?? throw new Exception($"The Assignment instance with id of {requestedObjectId} hasn't been found");
                                PrintAssignmentEntity(result);
                                break;
                            }
                        case ClassType.Call:
                            {
                                Call result = s_dalCall?.Read(requestedObjectId)!
                                    ?? throw new Exception($"The Call instance with id of {requestedObjectId} hasn't been found");
                                PrintCallEntity(result);
                                break;
                            }
                        case ClassType.Volunteer:
                            {
                                Volunteer result = s_dalVolunteer?.Read(requestedObjectId)!
                                    ?? throw new Exception($"The Volunteer instance with id of {requestedObjectId} hasn't been found");
                                PrintVolunteerEntity(result);
                                break;
                            }
                        default:
                            {
                                throw new Exception($"Internal Error: Class Type is {classType}. Is not a Assignment, Call or Volunteer");
                            }
                    }

                    // Operation Has Been Done Successfully
                    requestCompleted = true;
                }
                catch (Exception error)
                {
                    Console.WriteLine(error.Message);
                    Console.Beep();
                    Console.WriteLine("Press Enter to Continue ... ");
                    Console.ReadKey();
                }
            } while (!requestCompleted);
        }


        /// <summary>
        /// Performs the create action for a specific class.
        /// </summary>
        /// <param name="classType">The class type for the create action.</param>
        private static void CreateDbEntity(ClassType classType)
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
                        if (s_dalAssignment?.Read(newAsignment.Id) == null)
                        {
                            s_dalAssignment?.Create(newAsignment);
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
                        if (s_dalCall?.Read(newcall.Id) == null)
                        {
                            s_dalCall?.Create(newcall);
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


        /// <summary>
        /// This method initializes the database, filling it with pre-made dummy data 
        /// </summary>
        private static void DbInit() => DalTest.Initialization.Do(s_dalAssignment, s_dalCall, s_dalVolunteer, s_dalConfig);







        /// <summary>
        /// Help Method: Requests from the user to choose a ConfigVariable value and returns it
        /// </summary>
        /// <returns>The selected ConfigVariable value</returns>
        private static ConfigVariable GetUsersConfigVariableChoice()
        {
            bool isValidInput;
            string input;
            ConfigVariable configVariable;
            do
            {
                Console.WriteLine("Choose a Variable To View (RiskRange / Clock)");
                Console.Write(">>>");
                input = Console.ReadLine() ?? "";
                isValidInput = Enum.TryParse(input, out configVariable);
                if (!isValidInput)
                    Console.WriteLine($"Invalid Input Value! {input} Isn't Either RiskRange or Clock");

            } while (!isValidInput);
            return configVariable;
        }

        /// <summary>
        /// Given a single Assignment entity, the method will print all its fields
        /// </summary>
        /// <param name="assignment">The object requested to be printed</param>
        private static void PrintAssignmentEntity(Assignment assignment)
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

        /// <summary>
        /// Given a single Call entity, the method will print all its fields
        /// </summary>
        /// <param name="call">The object requested to be printed</param>
        private static void PrintCallEntity(Call call)
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
        
        /// <summary>
        /// Given a single Volunteer entity, the method will print all its fields
        /// </summary>
        /// <param name="volunteer">The object requested to be printed</param>
        private static void PrintVolunteerEntity(Volunteer volunteer)
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
    }
}
