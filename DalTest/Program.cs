using Dal;
using DalApi;
using DO;
using System.Data;
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
            while (operation != ClassSubMenuEnum.Exit)
            {
                try
                {
                    Console.WriteLine($@"
---------------------------------------------------------
Welcome To The {classType} Entity Management Window
Please Choose The Operation That You Would Like To Use:
    * 1 Exit - Exit to the Main Hub
    * 2 Create - Create a new {classType} entity
    * 3 Read - Read an individual {classType} entity 
    * 4 ReadAll - Read all {classType} entities in the database
    * 5 Update - Update an individual's {classType} entity field 
    * 6 Delete - Delete an individual {classType} entity
    * 7 DeleteAll - Delete all {classType} entities in the database
---------------------------------------------------------

");
                    Console.Write(">>> ");
                    string input = Console.ReadLine() ?? "";
                    if (Enum.TryParse(input, out operation))
                    {

                        switch (operation)
                        {
                            case ClassSubMenuEnum.Exit:
                                Console.WriteLine("Logging out of the Sub-Menu ...");
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
                                UpdateDbAction(classType);
                                break;
                            case ClassSubMenuEnum.Delete:
                                DeleteEntityById(classType);
                                break;
                            case ClassSubMenuEnum.DeleteAll:
                                DeleteAllDbAction(classType);
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
        /// This method removes all the records in the database of a given type
        /// </summary>
        /// <param name="classType">The entity type which is requested to be cleared</param>
        private static void DeleteAllDbAction(ClassType classType)
        {
            try
            {
                switch (classType)
                {
                    case ClassType.Assignment:
                        {
                            s_dalAssignment?.DeleteAll();
                            break;
                        }
                    case ClassType.Call:
                        {
                            s_dalCall?.DeleteAll();
                            break;
                        }
                    case ClassType.Volunteer:
                        {
                            s_dalVolunteer?.DeleteAll();
                            break;
                        }
                    default:
                        {
                            throw new Exception($"Internal Error: {classType} is not allowed");
                        }
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
            }
        }

        /// <summary>
        /// This method requests from the user an id in order to delete the entity with the same id of a given entity type
        /// </summary>
        /// <param name="classType">Entity type to be removed</param>
        private static void DeleteEntityById(ClassType classType)
        {
            string input;
            int id;
            bool isValid;
            do
            {
                input = Console.ReadLine() ?? "";
                Console.Write($"Enter the ID of object of type of {classType}: ");
                isValid = Int32.TryParse(input, out id);

                if (!isValid)
                    Console.WriteLine($"The Value {input} is not a valid number, Please try again");
            } while (!isValid);

            try
            {
                switch (classType)
                {
                    case ClassType.Assignment:
                        {
                            s_dalAssignment?.Delete(id);
                            break;
                        }
                    case ClassType.Call:
                        {
                            s_dalCall?.Delete(id);
                            break;
                        }
                    case ClassType.Volunteer:
                        {
                            s_dalVolunteer?.Delete(id);
                            break;
                        }
                    default:
                        {
                            throw new Exception($"Internal Error: {classType} is not allowed");
                        }
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
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
                        //TODO: Not enough data is requested from the user

                        // Request associated CallId 
                        Console.Write("Associated Call ID: ");
                        int callId = int.Parse(Console.ReadLine() ?? "");

                        // Prompt for the id of the volunteer
                        Console.Write("Associated Volunteer ID: ");
                        int volunteerId = int.Parse(Console.ReadLine() ?? "");

                        // Prompt for the time of starting
                        Console.Write("Time Of Starting: ");
                        DateTime start = DateTime.Parse(Console.ReadLine() ?? "");

                        // Prompt for the time of ending
                        Console.Write("Time Of Ending (Optional): ");
                        bool isValid = DateTime.TryParse(Console.ReadLine(),out DateTime tmp);
                        DateTime? end = (isValid) ? tmp: null;

                        // Prompt for the cause of ending
                        Console.Write("Cause Of ending (Optional): ");
                        isValid = Enum.TryParse(Console.ReadLine() ?? "", out TypeOfEnding tmp2);
                        TypeOfEnding? typeOfEnding = (isValid) ? tmp2 : null;

                        //Creation of the assignment entity
                        s_dalAssignment?.Create(new Assignment
                        {
                            Called = callId,
                            VolunteerId = volunteerId,
                            TimeOfStarting = start,
                            TimeOfEnding = end,
                            TypeOfEnding = typeOfEnding,
                        });
                    }
                    break;
                case ClassType.Call:
                    {
                        // Prompt for the call type
                        Console.Write("Type Of Call: ");
                        string input = Console.ReadLine() ?? "";
                        Enum.TryParse(input, out CallTypes callType);

                        // Prompt for the address
                        Console.Write("Address Of the Call: ");
                        string address = Console.ReadLine() ?? "";

                        // Prompt for the latitude of the address
                        Console.Write("Address's Latitude: ");
                        double latitude = double.Parse(Console.ReadLine() ?? "");

                        // Prompt for the longitude of the address
                        Console.Write("Address's Longitude: ");
                        double longitude = double.Parse(Console.ReadLine() ?? "");

                        // Prompt for the time the call starts
                        Console.Write("Call's Start Time: ");
                        DateTime start = DateTime.Parse(Console.ReadLine() ?? "");

                        // Prompt for the description of the call
                        Console.Write("Call's Description (Optional): ");
                        string? description = Console.ReadLine() ?? null;

                        // Prompt for the deadline of the call
                        Console.Write("Call's End Time (Optional): ");
                        DateTime tmp;
                        bool isValid = DateTime.TryParse(Console.ReadLine(),out tmp);
                        DateTime? deadLine = (isValid) ? tmp : null;

                        s_dalCall?.Create(new Call
                        {
                            Type = callType,
                            FullAddressCall = address,
                            Latitude = latitude,
                            Longitude = longitude,
                            OpeningTime = start,
                            Description = description,
                            DeadLine = deadLine
                        });
                        break;
                    }
                case ClassType.Volunteer:
                    {
                        // Prompt for the volunteer id
                        Console.Write("Volunteer ID: ");
                        int id = int.Parse(Console.ReadLine() ?? "");

                        // Prompt for the role of the volunteer
                        Console.Write("Volunteer's Access Role (Admin/Volunteer): ");
                        string input = Console.ReadLine() ?? "";
                        Enum.TryParse(input, out Roles role);

                        // Prompt for the volunteer full name
                        Console.Write("Volunteer Full Name: ");
                        string name = Console.ReadLine() ?? "";

                        // Prompt for the volunteer phone number
                        Console.Write("Volunteer's Phone Number: ");
                        string phoneNumber = Console.ReadLine() ?? "";

                        // Prompt for the volunteer email
                        Console.Write("Volunteer's Email Address: ");
                        string email = Console.ReadLine() ?? "";

                        // Prompt for the max distance to call the volunteer
                        Console.Write("Volunteer's Max Distance To Take a Call: ");
                        double tmp;
                        bool isValid = double.TryParse(Console.ReadLine() ,out tmp);
                        double? maxDistance = (isValid) ? tmp : null;

                        // Prompt for the type of range
                        Console.Write("Volunteer's Distance Type Of Range (e.g., Local, Regional, National): ");
                        string input1 = Console.ReadLine() ?? "";
                        Enum.TryParse(input1, out TypeOfRange typeOfRange);

                        // Prompt for the volunteer's active status
                        Console.WriteLine("Is the volunteer active? (true/false)");
                        bool active = bool.Parse(Console.ReadLine() ?? "");

                        // Prompt for the volunteer's password
                        Console.WriteLine("Volunteer's Password (Optional): ");
                        string? password = Console.ReadLine() ?? null;

                        // Prompt for the volunteer's full current address
                        Console.Write("Volunteer's Full Address (Optional): ");
                        string? address = Console.ReadLine() ?? null;

                        // Prompt for the volunteer's full current address
                        Console.Write("Volunteer's Full Address Latitude (Optional): ");
                        double tmp2;
                        isValid = double.TryParse(Console.ReadLine(),out tmp2);
                        double? lat = (isValid) ? tmp2 : null;

                        // Prompt for the volunteer's full current address
                        Console.Write("Volunteer's Full Address Longitude (Optional): ");
                        isValid = double.TryParse(Console.ReadLine(), out tmp2);
                        double? lng = (isValid) ? tmp2 : null;

                        s_dalVolunteer?.Create(new Volunteer
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
                            Latitude = lat,
                            Longitude = lng,
                        });
                        break;
                    }
            }
        }

        /// <summary>
        /// Updates an entity of a given type and a given id value
        /// </summary>
        /// <param name="classType">Individual entity type which is requested to be updated</param>
        private static void UpdateDbAction(ClassType classType)
        {
            string input;
            int id;
            bool isValid;
            do
            {
                input = Console.ReadLine() ?? "";
                Console.Write($"Enter the ID of object of type of {classType}: ");
                isValid = Int32.TryParse(input, out id);

                if (!isValid)
                    Console.WriteLine($"The Value {input} is not a valid number, Please try again");
            } while (!isValid);

            try
            {
                switch (classType)
                {
                    case ClassType.Assignment:
                        {
                            Assignment result = s_dalAssignment?.Read(id) ?? 
                                throw new Exception($"Assignment Object Failed: The Assignment with ID of {id} hasn't been found");
                            int Called = result.Called ;
                            int VolunteerId = result.VolunteerId ;
                            DateTime TimeOfStarting = result.TimeOfStarting;
                            DateTime? TimeOfEnding = result.TimeOfEnding ;
                            TypeOfEnding? TypeOfEnding = result.TypeOfEnding;

                            Console.WriteLine(@$"
---------------------------------------
Assignment Object
ID: {result.Id}
CallId: {result.Called}
VolunteerId: {result.VolunteerId}
---------------------------------------

");

                            //Request for a new TimeOfStarting value 
                            do
                            {
                                Console.WriteLine("Enter new Time Of Starting Value in the following format: dd/mm/yyyy hh:mm:ss");
                                Console.Write(">>> ");
                                
                                input = Console.ReadLine() ?? "";
                                isValid = DateTime.TryParse(input, out TimeOfStarting);
                                
                                if(!isValid)
                                    Console.WriteLine($"The value: {input} is not valid!");
                            } while (!isValid);

                            //Request for a new TimeOfEnding value
                            do
                            {
                                Console.WriteLine("Enter new Time Of Ending Value in the following format: dd/mm/yyyy hh:mm:ss");
                                Console.Write(">>> ");
                                input = Console.ReadLine() ?? "";
                                
                                if(input == "")
                                {
                                    isValid = true;
                                    TimeOfEnding = null;
                                }
                                else
                                {

                                    DateTime temp;
                                    isValid = DateTime.TryParse(input, out temp);
                                    if (!isValid)
                                    {
                                        Console.WriteLine($"The value: {input} is not valid!");
                                    }
                                    else
                                    {
                                        TimeOfEnding = temp;
                                    }
                                }
                            } while (!isValid);

                            //Request for a new TypeOfEnding value
                            do
                            {
                                
                                Console.WriteLine($"Select new Type Of Ending value from the following list ({DO.TypeOfEnding.CancellationExpired},{DO.TypeOfEnding.AdminCanceled},{DO.TypeOfEnding.Treated},{DO.TypeOfEnding.SelfCanceled})");
                                Console.Write(">>> ");
                                input = Console.ReadLine() ?? "";
                                if (input == "")
                                {
                                    isValid = true;
                                    TimeOfEnding = null;
                                }
                                else
                                {

                                    DO.TypeOfEnding temp;
                                    isValid = Enum.TryParse(input, out temp);
                                    if (!isValid)
                                    {
                                        Console.WriteLine($"The value: {input} is not valid!");
                                    }
                                    else
                                    {
                                        TypeOfEnding = temp;
                                    }
                                }
                            } while (!isValid);

                            Console.Write(@$"
----------------------------------------
Assignment Object Update - Old Version
Assignment ID: {result.Id}                 --->
Call ID: {result.Called}                   --->
Volunteer ID: {result.VolunteerId}         --->
Time Of Starting: {result.TimeOfStarting}  --->
Time Of Ending: {result.TimeOfEnding }     --->
Type Of Ending: {result.TypeOfEnding}      --->
----------------------------------------

");
                            Console.Write(@$"
----------------------------------------
Assignment Object Update - New Version
Assignment ID: {result.Id}          
Call ID: {Called}                   
Volunteer ID: {VolunteerId}         
Time Of Starting: {TimeOfStarting}  
Time Of Ending: {TimeOfEnding}     
Type Of Ending: {TypeOfEnding}     
----------------------------------------

");
                            Console.WriteLine();

                            Assignment newAssignment = new Assignment()
                            {
                                Id = result.Id,
                                Called  = Called ,
                                VolunteerId  = VolunteerId ,
                                TimeOfStarting = TimeOfStarting,
                                TimeOfEnding = TimeOfEnding,
                                TypeOfEnding = TypeOfEnding,
                            };
                            s_dalAssignment.Update(newAssignment);
                            
                            break;
                        }
                    case ClassType.Call:
                    {
                            Call result = s_dalCall?.Read(id) ??
                                    throw new Exception($"Call Object Failed: The Assignment with ID of {id} hasn't been found");
                            CallTypes Type = result.Type;
                            string FullAddressCall = result.FullAddressCall;
                            double Latitude = result.Latitude;
                            double Longitude = result.Longitude;
                            DateTime OpeningTime = result.OpeningTime;
                            string? Description = result.Description;
                            DateTime? DeadLine = result.DeadLine;

                            Console.WriteLine(@$"
---------------------------------------
Call Object
ID: {result.Id}
---------------------------------------

");

                            //Request for a new Type Of Call value 
                            do
                            {
                                Console.WriteLine($"Enter new Type Of Call from these options ({DO.CallTypes.FoodPreparation},{DO.CallTypes.FoodDelivery})");
                                Console.Write(">>> ");

                                input = Console.ReadLine() ?? "";
                                isValid = Enum.TryParse(input, out Type);

                                if (!isValid)
                                    Console.WriteLine($"The value: {input} is not valid!");
                            } while (!isValid);

                            //Request for a new call full address value 
                            do
                            {
                                Console.WriteLine($"Enter new Call Full Address");
                                Console.Write(">>> ");

                                input = Console.ReadLine() ?? "";

                                if (input == "")
                                    Console.WriteLine($"The value: `{input}` is not valid!");
                                //TODO: Need to add the option to check if the address is valid
                                //TOOD: Need to add the option to find the latitude and logitude of the new loaction
                            } while (input == "");
                            FullAddressCall = input;

                            //Request for a new Latitude value 
                            do
                            {
                                Console.WriteLine($"Enter new Call Full Address's Latitude value");
                                Console.Write(">>> ");

                                input = Console.ReadLine() ?? "";
                                isValid = double.TryParse(input, out Latitude);
                                if (!isValid)
                                    Console.WriteLine($"The value: {input} is not valid!");
                            } while (!isValid);

                            //Request for a new Longitude value 
                            do
                            {
                                Console.WriteLine($"Enter new Call Full Address's Longitude value");
                                Console.Write(">>> ");

                                input = Console.ReadLine() ?? "";
                                isValid = double.TryParse(input, out Longitude);
                                if (!isValid)
                                    Console.WriteLine($"The value: {input} is not valid!");
                            } while (!isValid);

                            //Request for a new Opening Time value
                            do
                            {
                                Console.WriteLine($"Enter new Opening Time value in the following formatting (DD/MM/YYYY HH:MM:SS)");
                                Console.Write(">>> ");

                                isValid = DateTime.TryParse(input, out OpeningTime);
                                if (!isValid)
                                {
                                    Console.WriteLine($"The value: {input} is not valid!");
                                }
                            } while (!isValid);

                            //Request for a new description value
                            {
                                Console.WriteLine($"Enter a new description for the call");
                                Console.Write(">>> ");
                                input = Console.ReadLine() ?? "";
                            }

                            //Request for a new Dead Line Time value
                            do
                            {

                                Console.WriteLine($"Enter a new Dead Line value in the next format (DD/MM/YYYY HH:MM:SS)");
                                Console.Write(">>> ");
                                input = Console.ReadLine() ?? "";
                                if (input == "")
                                {
                                    isValid = true;
                                    DeadLine = null;
                                }
                                else
                                {
                                    DateTime temp;
                                    isValid = Enum.TryParse(input, out temp);
                                    if (!isValid)
                                    {
                                        Console.WriteLine($"The value: {input} is not valid!");
                                    }
                                    else
                                    {
                                        DeadLine = temp;
                                    }
                                }
                            } while (!isValid);

                            Console.Write(@$"
-----------------------------------------------
Call Object Update - Old Version
Call ID: {result.Id}                        -->
Type : {result.Type}                        -->
FullAddressCall : {result.FullAddressCall}  -->
Latitude : {result.Latitude}                -->
Longitude : {result.Longitude}              -->
OpeningTime : {result.OpeningTime}          -->
Description : {result.Description}          -->
DeadLine : {result.DeadLine}                -->
-----------------------------------------------

");
                            Console.Write(@$"
-----------------------------------------------
Volunteer Object Update - New Version
Call ID: {result.Id}   
Type : {Type}
FullAddressCall : {FullAddressCall}
Latitude : {Latitude}
Longitude : {Longitude}
OpeningTime : {OpeningTime}
Description : {Description}
DeadLine : {DeadLine}
-----------------------------------------------

");
                            Console.WriteLine();

                            Call newCall = new Call()
                            {
                                Id = result.Id,
                                Type = Type,
                                FullAddressCall = FullAddressCall,
                                Latitude = Latitude,
                                Longitude = Longitude,
                                OpeningTime = OpeningTime,
                                Description = Description,
                                DeadLine = DeadLine,
                            };
                            s_dalCall?.Update(newCall);
                            break;
                    }
                    case ClassType.Volunteer:
                    {
                            Volunteer result = s_dalVolunteer?.Read(id) ??
                                        throw new Exception($"Volunteer Object Failed: The Assignment with ID of {id} hasn't been found");
                            int Id = result.Id;
                            Roles Role = result.Role;
                            string FullName = result.FullName;
                            string PhoneNumber = result.PhoneNumber;
                            string Email = result.Email;
                            double? MaxDistanceToCall = result.MaxDistanceToCall;
                            TypeOfRange TypeOfRange=result.TypeOfRange;
                            bool Active= result.Active;
                            string? Password = result.Password;
                            string? FullCurrentAddress = result.FullCurrentAddress;
                            double? Latitude = result.Latitude;
                            double? Longitude = result.Longitude;



                            Console.WriteLine(@$"
---------------------------------------
Volunteer Object
ID: {result.Id}
---------------------------------------

");

                            //Request for a new Role value for the volunteer
                            Console.WriteLine($"Enter new Role value for the role ({DO.Roles.Admin},{DO.Roles.Volunteer})");
                            Console.Write(">>> ");
                            Enum.TryParse(input, out Role);

                            //Request for a new call full address value 
                            Console.WriteLine($"Enter new Full Name");
                            Console.Write(">>> ");
                            FullName = Console.ReadLine() ?? "";

                            //Request for a new Phone Number value 
                            Console.WriteLine($"Enter new Phone Number");
                            Console.Write(">>> ");
                            PhoneNumber = Console.ReadLine() ?? "";

                            //Request for a new Email Address value 
                            Console.WriteLine($"Enter new Email address");
                            Console.Write(">>> ");
                            Email = Console.ReadLine() ?? "";

                            //Request for a new Max Distance value To Answer a Call 
                            Console.WriteLine($"Enter new Max Distance to answer a Call");
                            Console.Write(">>> ");
                            double tmp;
                            double.TryParse(Console.ReadLine(), out tmp);
                            MaxDistanceToCall = tmp;

                            //Request for a new Type of Range value 
                            Console.WriteLine($"Enter a new Type Of Range from the following list: ({DO.TypeOfRange.walkingDistance},{DO.TypeOfRange.drivingDistance},{DO.TypeOfRange.AirDistance})");
                            Console.Write(">>> ");
                            Enum.TryParse<TypeOfRange>(Console.ReadLine(), out TypeOfRange);

                            //Request for a new Phone Number value 
                            Console.WriteLine($"Enter a new Is Active value");
                            Console.Write(">>> ");
                            bool.TryParse(Console.ReadLine(), out Active);

                            //Request for a new Phone Number value 
                            Console.WriteLine($"Enter a new Password");
                            Console.Write(">>> ");
                            Password = Console.ReadLine();

                            //Request for a new Current Address value 
                            Console.WriteLine($"Enter a new Full Current Address value");
                            Console.Write(">>> ");
                            FullCurrentAddress = Console.ReadLine();

                            //Request for a new Longitude value 
                            Console.WriteLine($"Enter a new Longitude value");
                            Console.Write(">>> ");
                            double.TryParse(Console.ReadLine(), out tmp);
                            Longitude = tmp;

                            //Request for a new Latitude value 
                            Console.WriteLine($"Enter a new Latitude value");
                            Console.Write(">>> ");
                            double.TryParse(Console.ReadLine(), out tmp);
                            Latitude = tmp;

                            //Summery
                            Console.Write(@$"
-----------------------------------------------
Volunteer Object Update - Old Version
Volunteer ID : {result.Id}
Role : {result.Role}
Full Name : {result.FullName}
Phone Number : {result.PhoneNumber}
Email : {result.Email}
Max Distance To Call : {result.MaxDistanceToCall}
Type Of Range : {result.TypeOfRange}
Active : {result.Active}
Password : {result.Password}
Full Current Address : {result.FullCurrentAddress}
Latitude : {result.Latitude}
Longitude : {result.Longitude}
-----------------------------------------------

");
                            Console.Write(@$"
-----------------------------------------------
Volunteer Object Update - New Version
Volunteer ID : {result.Id}
Role : {Role}
Full Name : {FullName}
Phone Number : {PhoneNumber}
Email : {Email}
Max Distance To Call : {MaxDistanceToCall}
Type Of Range : {TypeOfRange}
Active : {Active}
Password : {Password}
Full Current Address : {FullCurrentAddress}
Latitude : {Latitude}
Longitude : {Longitude}
-----------------------------------------------

");
                            Console.WriteLine();

                            Volunteer newVolunteer = new Volunteer()
                            {
                                Id = result.Id,
                                Role = Role,
                                FullName = FullName,
                                PhoneNumber = PhoneNumber,
                                Email = Email,
                                MaxDistanceToCall = MaxDistanceToCall,
                                TypeOfRange = TypeOfRange,
                                Active = Active,
                                Password = Password,
                                FullCurrentAddress = FullCurrentAddress,
                                Latitude = Latitude,
                                Longitude = Longitude
                            };
                            s_dalVolunteer?.Update(newVolunteer);
                            break;
                    }
                    default:
                    {
                        throw new Exception($"Internal Error: {classType} is not allowed");
                    }
                }
            }catch(Exception error)
            {
                Console.WriteLine(error.Message);
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
