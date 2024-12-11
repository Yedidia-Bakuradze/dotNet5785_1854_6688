using Dal;
using DalApi;
using DO;
using System.Collections;
using System.Xml;

namespace DalTest
{
    internal class Program
    {
        //private static IAssignment? ss_dalAssignment = new AssignmentImplementation(); // Stage 1
        //private static IVolunteer? ss_dalVolunteer = new VolunteerImplementation(); // Stage 1
        //private static ICall? ss_dalCall = new CallImplementation(); // Stage 1
        //private static IConfig? ss_dalConfig = new ConfigImplementation(); // Stage 1
        //static readonly IDal ss_dal = new DalList(); // Stage 2
        //static readonly IDal ss_dal = new DalXml(); // Stage 3
        static readonly IDal ss_dal = Factory.Get; // Stage 4

        public enum MainMenuEnum { FirstRun, Exit, ShowAssignmentMenu, ShowCallMenu, ShowVolunteerMenu, DbInit, ShowAllDbData, ShowConfigMenu, ResetSysAndDb }
        public enum ClassSubMenuEnum { FirstRun, Exit, Create, Read, ReadAll, Update, Delete, DeleteAll }
        public enum ConfigSubMenuEnum { FirstRun, Exit,AddMinute ,AddHour,AddMonth , AddYear, ShowCurrentClock,SetValue,ShowValue,Reset }
        public enum ClassType { Assignment, Call, Volunteer };
        public enum ConfigVariable { FirstRun,RiskRange,Clock }
        static void Main(string[] args)
        {
            mainMenu();
        }

        /// <summary>
        /// Displays the main menu and handles user input.
        /// </summary>
        private static void mainMenu()
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
                                showSubMenu(ClassType.Assignment);
                                break;
                            case MainMenuEnum.ShowCallMenu:
                                showSubMenu(ClassType.Call);
                                break;
                            case MainMenuEnum.ShowVolunteerMenu:
                                showSubMenu(ClassType.Volunteer);
                                break;
                            case MainMenuEnum.DbInit:
                                dbInit();
                                break;
                            case MainMenuEnum.ShowAllDbData:
                                readAllEntitiesByDb();
                                break;
                            case MainMenuEnum.ShowConfigMenu:
                                configSubMenu();
                                break;
                            case MainMenuEnum.ResetSysAndDb:
                                resetDbAndSystem();
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
        private static void showSubMenu(ClassType classType)
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
                                createDbEntity(classType);
                                break;
                            case ClassSubMenuEnum.Read:
                                readSingleDbEntity(classType);
                                break;
                            case ClassSubMenuEnum.ReadAll:
                                readAllEntitiesByType(classType);
                                break;
                            case ClassSubMenuEnum.Update:
                                updateDbAction(classType);
                                break;
                            case ClassSubMenuEnum.Delete:
                                deleteEntityById(classType);
                                break;
                            case ClassSubMenuEnum.DeleteAll:
                                deleteAllDbAction(classType);
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
        private static void deleteAllDbAction(ClassType classType)
        {
            try
            {
                switch (classType)
                {
                    case ClassType.Assignment:
                        {
                            ss_dal?.Assignment?.DeleteAll();
                            break;
                        }
                    case ClassType.Call:
                        {
                            ss_dal?.Call?.DeleteAll();
                            break;
                        }
                    case ClassType.Volunteer:
                        {
                            ss_dal?.Volunteer?.DeleteAll();
                            break;
                        }
                    default:
                        {
                            throw new DalForbiddenOperation($"Internal Error: {classType} is not allowed");
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
        private static void deleteEntityById(ClassType classType)
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
                            ss_dal?.Volunteer?.Delete(id);
                            break;
                        }
                    case ClassType.Call:
                        {
                            ss_dal?.Call?.Delete(id);
                            break;
                        }
                    case ClassType.Volunteer:
                        {
                            ss_dal?.Call?.Delete(id);
                            break;
                        }
                    default:
                        {
                            throw new DalForbiddenOperation($"Internal Error: {classType} is not allowed");
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
        private static void resetDbAndSystem()
        {
            ss_dal?.Volunteer?.DeleteAll();
            ss_dal?.Call?.DeleteAll();
            ss_dal?.Assignment?.DeleteAll();
            ss_dal?.Config?.Reset();
        }
        

        /// <summary>
        /// Provides a submenu for configuring application settings. Users can adjust time parameters, view or change specific configurations,
        /// and reset all settings. The function loops until the "Exit" option is selected.
        /// i used Chat GPT for the comments here.
        /// </summary>
        private static void configSubMenu()
        {
            ConfigSubMenuEnum operation = ConfigSubMenuEnum.FirstRun;
            while (operation != ConfigSubMenuEnum.Exit)
            {
                try
                {
                    Console.Write(@"
---------------------------------------------------------------------------------

    Please choose an option:

- 1 Exit             - To Exit The Sub Menu
- 2 AddMinute        - To Add Minutes To System Clock
- 3 AddHour          - To Add Hours To System Clock
- 4 AddMonth         - To Add Months To System Clock
- 5 AddYear          - To Add Years To System Clock
- 6 ShowCurrentClock - To Show Current Time
- 7 SetValue         - To Set a Value to System Variables (RisRange, Clock)
- 8 ShowValue        - To Display System Variables Values (RisRange, Clock)
- 9 Reset            - To Reset System Variables Values (RisRange, Clock)

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
                                    ss_dal?.Config?.Clock.AddMinutes(1);
                                    break;
                                }
                            case ConfigSubMenuEnum.AddHour:
                                {
                                    // Adds one hour to the current clock setting
                                    ss_dal?.Config?.Clock.AddHours(1);
                                    break;
                                }
                            case ConfigSubMenuEnum.AddMonth:
                                {
                                    // Adds one month to the current clock setting
                                    ss_dal?.Config?.Clock.AddMonths(1);
                                    break;
                                }
                            case ConfigSubMenuEnum.AddYear:
                                {
                                    // Adds one year to the current clock setting
                                    ss_dal?.Config?.Clock.AddYears(1);
                                    break;
                                }
                            case ConfigSubMenuEnum.ShowCurrentClock:
                                {
                                    // Displays the current clock setting
                                    Console.WriteLine(ss_dal?.Config?.Clock);
                                    break;
                                }
                            case ConfigSubMenuEnum.SetValue:
                                {
                                    // Allows user to set a new value for either RiskRange or Clock
                                    ConfigVariable configVariable = getUsersConfigVariableChoice();
                                    switch (configVariable)
                                    {
                                        case ConfigVariable.RiskRange:
                                            // Sets a new value for RiskRange
                                            Console.WriteLine("Enter the new value: ");
                                            TimeSpan timeSpan = TimeSpan.Parse(Console.ReadLine() ?? "");
                                            ss_dal!.Config!.RiskRange = timeSpan;
                                            break;
                                        case ConfigVariable.Clock:
                                            // Sets a new value for Clock
                                            Console.WriteLine("Enter the new value: ");
                                            DateTime clock = DateTime.Parse(Console.ReadLine() ?? "");
                                            ss_dal!.Config!.Clock = clock;
                                            break;
                                        default:
                                            {
                                                throw new DalInValidConfigVariable($"Invalid configVariable: {configVariable} ");
                                            }
                                    }
                                    break;
                                }
                            case ConfigSubMenuEnum.ShowValue:
                                {
                                    ConfigVariable configVariable = getUsersConfigVariableChoice();

                                    switch (configVariable)
                                    {
                                        case ConfigVariable.RiskRange:
                                            // Shows the current RiskRange value
                                            Console.WriteLine(ss_dal?.Config?.RiskRange);
                                            break;
                                        case ConfigVariable.Clock:
                                            // Shows the current Clock value
                                            Console.WriteLine(ss_dal?.Config?.Clock);
                                            break;
                                        default:
                                            {
                                                throw new DalInValidConfigVariable($"Invalid configVariable: {configVariable} ");
                                            }
                                    }
                                    break;
                                }
                            case ConfigSubMenuEnum.Reset:
                                {
                                    // Resets all settings to their default values
                                    ss_dal?.Config?.Reset();
                                    break;
                                }
                            default:
                                {
                                    throw new DalForbiddenOperation($"Forbidden Operation: The {operation} operation is not allowed!");
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
        private static void readAllEntitiesByType(ClassType classType)
        {
            try
            { 
                switch (classType)
                {
                    case ClassType.Assignment:
                        {
                            IEnumerable listOfAssignments = (ss_dal?.Assignment?.ReadAll()
                                ?? throw new DalDoesNotExistException("The Assignments in Database Are Not Available"));
                            Console.WriteLine("Assignment Entities: ");
                            Console.Beep();
                            foreach (Assignment assignment in listOfAssignments)
                                printAssignmentEntity(assignment);
                            break;
                        }
                    case ClassType.Call:
                        {
                            IEnumerable listOfCalls = ss_dal?.Call?.ReadAll()
                                ?? throw new DalDoesNotExistException("The Calls in Database Are Not Available!");
                            Console.WriteLine("\nCall Entities: ");
                            foreach (Call call in listOfCalls)
                                printCallEntity(call);
                            break;
                        }
                    case ClassType.Volunteer:
                        {
                            List<Volunteer> listOfVolunteers = ss_dal?.Volunteer?.ReadAll().ToList()
                                ?? throw new DalDoesNotExistException("The Volunteers in Database Are Not Available!");
                            
                            Console.WriteLine("\nVolunteer Entities: ");
                            Console.Beep();
                            foreach (Volunteer volunteer in listOfVolunteers)
                                printVolunteerEntity(volunteer);
                            break;
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
        private static void readAllEntitiesByDb()
        {
            readAllEntitiesByType(ClassType.Assignment);
            readAllEntitiesByType(ClassType.Call);
            readAllEntitiesByType(ClassType.Volunteer);
        }
        

        /// <summary>
        /// This method requests the id value from the user, locates the instance if existed and prints its values
        /// </summary>
        /// <param name="classType">The type of instance which is requested, either Assignment, Call or Volunteer</param>
        private static void readSingleDbEntity(ClassType classType)
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
                        Console.WriteLine($"Enter Id Value For Object Of Type {classType}");
                        Console.Write(">>> ");
                        isValid = Int32.TryParse(Console.ReadLine(), out requestedObjectId);
                        if (!isValid)
                        {
                            Console.WriteLine("Invalid Input Value! Please Enter Only Natural Numbers");
                        }


                    } while (!isValid);

                    //Fetch Object By ID and Print Out Its Fields
                    switch (classType)
                    {
                        case ClassType.Assignment:
                            {
                                Assignment result = ss_dal?.Assignment?.Read(requestedObjectId)!
                                    ?? throw new DalDoesNotExistException($"The Assignment instance with id of {requestedObjectId} hasn't been found");
                                printAssignmentEntity(result);
                                break;
                            }
                        case ClassType.Call:
                            {
                                Call result = ss_dal?.Call?.Read(requestedObjectId)!
                                    ?? throw new DalDoesNotExistException($"The Call instance with id of {requestedObjectId} hasn't been found");
                                printCallEntity(result);
                                break;
                            }
                        case ClassType.Volunteer:
                            {
                                Volunteer result = ss_dal?.Volunteer?.Read(requestedObjectId)!
                                    ?? throw new DalDoesNotExistException($"The Volunteer instance with id of {requestedObjectId} hasn't been found");
                                printVolunteerEntity(result);
                                break;
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
        private static void createDbEntity(ClassType classType)
        {
            //Getting data for the new instance
            switch (classType)
            {
                case ClassType.Assignment:
                    {
                        // Request associated CallId
                        int callId = requestIntValue("Associated Call ID: ");

                        // Prompt for the id of the volunteer
                        int volunteerId= requestIntValue("Associated Volunteer ID: ");

                        // Prompt for the time of starting
                        DateTime start = requestDateTimeValue("Time Of Starting: ");

                        // Prompt for the time of ending
                        Console.Write("Time Of Ending (Optional): ");
                        bool isValid = DateTime.TryParse(Console.ReadLine(),out DateTime tmp);
                        DateTime? end = (isValid) ? tmp: null;

                        // Prompt for the cause of ending
                        Console.Write("Cause Of ending (Optional): ");
                        isValid = Enum.TryParse(Console.ReadLine() ?? "", out TypeOfEnding tmp2);
                        TypeOfEnding? typeOfEnding = (isValid) ? tmp2 : null;

                        //Creation of the assignment entity
                        ss_dal?.Assignment?.Create(new Assignment
                        {
                            CallId = callId,
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
                        bool isValid;
                        CallType callType;
                        do
                        {
                            Console.Write("Type Of Call (FoodPreparation,FoodDelivery): ");
                            string input = Console.ReadLine() ?? "";
                            isValid = Enum.TryParse(input, out callType);
                            if (!isValid)
                            {
                                Console.WriteLine($"Error! Please provide a proper Type Of Call value ({CallType.FoodDelivery},{CallType.FoodPreparation})");
                            }
                        } while (!isValid);

                        // Prompt for the address
                        string address = requestStringValue("Address Of the Call: ");

                        // Prompt for the latitude of the address
                        double latitude = requestDoubleValue("Address's Latitude: ");

                        // Prompt for the longitude of the address
                        double longitude = requestDoubleValue("Address's Longitude: ");

                        // Prompt for the time the call starts
                        DateTime start = requestDateTimeValue("Call's Start Time: ");

                        // Prompt for the description of the call
                        Console.Write("Call's Description (Optional): ");
                        string? description = Console.ReadLine() ?? null;

                        // Prompt for the deadline of the call
                        Console.Write("Call's End Time (Optional): ");
                        DateTime tmp;
                        isValid = DateTime.TryParse(Console.ReadLine(),out tmp);
                        DateTime? deadLine = (isValid) ? tmp : null;

                        ss_dal?.Call?.Create(new Call
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
                        int id = requestIntValue("Volunteer ID: ");

                        // Prompt for the role of the volunteer
                        UserRole role;
                        bool isValid;
                        do
                        {
                            Console.Write($"Volunteer's Access Role ({UserRole.Admin} / {UserRole.Volunteer}): ");
                            string input = Console.ReadLine() ?? "";
                            isValid = Enum.TryParse(input, out role);
                            if (!isValid)
                            {
                                Console.WriteLine($"Error! Please provide a proper Role value ({UserRole.Admin} / {UserRole.Volunteer})");
                            }

                        } while (!isValid);

                        // Prompt for the volunteer full name
                        string name = requestStringValue("Volunteer Full Name: ");

                        // Prompt for the volunteer phone number
                        string phoneNumber = requestStringValue("Volunteer's Phone Number: ");

                        // Prompt for the volunteer email
                        string email = requestStringValue("Volunteer's Email Address: ");

                        // Prompt for the max distance to call the volunteer
                        Console.Write("Volunteer's Max Distance To Take a Call: ");
                        double tmp;
                        isValid = double.TryParse(Console.ReadLine() ,out tmp);
                        double? maxDistance = (isValid) ? tmp : null;

                        // Prompt for the type of range
                        TypeOfRange typeOfRange;
                        do
                        {
                            Console.Write("Volunteer's Distance Type Of Range ( (0)e.g.,(1) Local,(2) Regional,(3) National): ");
                            string input = Console.ReadLine() ?? "";
                            isValid = Enum.TryParse(input, out typeOfRange);
                            if (!isValid)
                            {
                                Console.WriteLine("Error! Please provide a proper Type Of Range value");
                            }
                        } while (!isValid);

                        // Prompt for the volunteer's active status
                        bool active;
                        do
                        {
                            Console.WriteLine("Is the volunteer active? (true/false)");
                            isValid = bool.TryParse(Console.ReadLine() ?? "", out active);
                            if (!isValid)
                            {
                                Console.WriteLine("Error! Please provide a proper boolean value");
                            }
                        } while (!isValid);

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

                        ss_dal?.Volunteer?.Create(new Volunteer
                        {
                            Id = id,
                            FullName = name,
                            Role = role,
                            PhoneNumber = phoneNumber,
                            Email = email,
                            MaxDistanceToCall = maxDistance,
                            IsActive = active,
                            Password = password,
                            FullCurrentAddress = address,
                            RangeType = typeOfRange,
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
        private static void updateDbAction(ClassType classType)
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
                            Assignment result = ss_dal?.Assignment?.Read(id) ?? 
                                throw new DalDoesNotExistException($"The Assignment instance with Id of {id} hasn't been found");
                            int Called = result.CallId ;
                            int VolunteerId = result.VolunteerId ;
                            DateTime TimeOfStarting = result.TimeOfStarting;
                            DateTime? TimeOfEnding = result.TimeOfEnding ;
                            TypeOfEnding? TypeOfEnding = result.TypeOfEnding;

                            Console.WriteLine(@$"
---------------------------------------
Assignment Object
ID: {result.Id}
CallId: {result.CallId}
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
Call ID: {result.CallId}                   --->
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
                                CallId  = Called ,
                                VolunteerId  = VolunteerId ,
                                TimeOfStarting = TimeOfStarting,
                                TimeOfEnding = TimeOfEnding,
                                TypeOfEnding = TypeOfEnding,
                            };
                            ss_dal?.Assignment.Update(newAssignment);
                            
                            break;
                        }
                    case ClassType.Call:
                    {
                            Call result = ss_dal?.Call?.Read(id) ??
                                    throw new DalDoesNotExistException($"The Call instance with ID of {id} hasn't been found");
                            CallType Type = result.Type;
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
                                Console.WriteLine($"Enter new Type Of Call from these options ({DO.CallType.FoodPreparation},{DO.CallType.FoodDelivery})");
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
                            ss_dal?.Call?.Update(newCall);
                            break;
                    }
                    case ClassType.Volunteer:
                    {
                            Volunteer result = ss_dal?.Volunteer?.Read(id) ??
                                        throw new DalDoesNotExistException($"The Volunteer instance with ID of {id} hasn't been found");
                            int Id = result.Id;
                            UserRole Role = result.Role;
                            string FullName = result.FullName;
                            string PhoneNumber = result.PhoneNumber;
                            string Email = result.Email;
                            double? MaxDistanceToCall = result.MaxDistanceToCall;
                            TypeOfRange TypeOfRange=result.RangeType;
                            bool Active= result.IsActive;
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
                            Console.WriteLine($"Enter new Role value for the role ({DO.UserRole.Admin},{DO.UserRole.Volunteer})");
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
                            Console.WriteLine($"Enter a new Type Of Range from the following list: ({DO.TypeOfRange.WalkingDistance},{DO.TypeOfRange.DrivingDistance},{DO.TypeOfRange.AirDistance})");
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
Type Of Range : {result.RangeType}
Active : {result.IsActive}
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
                                RangeType = TypeOfRange,
                                IsActive = Active,
                                Password = Password,
                                FullCurrentAddress = FullCurrentAddress,
                                Latitude = Latitude,
                                Longitude = Longitude
                            };
                            ss_dal?.Volunteer?.Update(newVolunteer);
                            break;
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
        //private static void dbInit() => DalTest.Initialization.Do(ss_dalAssignment, ss_dalCall, ss_dalVolunteer, ss_dalConfig);// Stage 1
        //private static void dbInit() => DalTest.Initialization.Do(ss_dal);// Stage 2
        private static void dbInit() => DalTest.Initialization.Do();// Stage 4

        /// <summary>
        /// Help Method: Requests from the user to choose a ConfigVariable value and returns it
        /// </summary>
        /// <returns>The selected ConfigVariable value</returns>
        private static ConfigVariable getUsersConfigVariableChoice()
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
        private static void printAssignmentEntity(Assignment assignment)
        {
            Console.WriteLine($@"
            ---------------------------------------------------------------
                    
                Assignment ID: {assignment.Id}
                Related Call ID: {assignment.CallId}
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
        private static void printCallEntity(Call call)
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
        private static void printVolunteerEntity(Volunteer volunteer)
        {
            Console.WriteLine($@"
            ---------------------------------------------------------------
                    
                Volunteer ID: {volunteer.Id}
                Volunteer Role: {volunteer.Role}
                Full Name: {volunteer.FullName}
                Email Address: {volunteer.Email}
                Max Distance From The Call: {volunteer.MaxDistanceToCall}
                Type Of Distance Range: {volunteer.RangeType}
                Active: {volunteer.IsActive}
                Password: {volunteer.Password}
                Living Address: {volunteer.FullCurrentAddress}
                Living Address Latitude: {volunteer.Latitude}
                Living Address Longitude: {volunteer.Longitude}

            ---------------------------------------------------------------
            >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

            ");
        }
    
        /// <summary>
        /// This method handles the request of int value from the user, including the null check
        /// </summary>
        /// <param name="msg">A display message for the user</param>
        /// <returns>The converted int value</returns>
        private static int requestIntValue(string msg)
        {
            int id;
            bool isValid;
            do
            {
                Console.Write(msg);
                isValid = Int32.TryParse(Console.ReadLine() ?? "", out id);
                if (!isValid)
                {
                    Console.WriteLine("Error! Please enter a valid int value");
                }
            } while (!isValid);
            return id;
        }

        /// <summary>
        /// This method handles the DateTime request from the user, including the conversion of the data type and null value check
        /// </summary>
        /// <param name="msg">The display message for the user</param>
        /// <returns>The converted DateTime value</returns>
        private static DateTime requestDateTimeValue(string msg)
        {
            DateTime date;
            bool isValid;
            do
            {
                Console.Write(msg);
                isValid = DateTime.TryParse(Console.ReadLine() ?? "", out date);
                if (!isValid)
                {
                    Console.WriteLine("Error! Please enter a valid DateTime value");
                }
            } while (!isValid);
            return date;
        }

        /// <summary>
        /// This method handles the double request from the user, including null value check
        /// </summary>
        /// <param name="msg">The display message for the user</param>
        /// <returns>The converted double value</returns>
        private static double requestDoubleValue(string msg)
        {
            double value;
            string input;
            bool isValid;
            do
            {
                Console.Write(msg);
                input = Console.ReadLine() ?? "";
                isValid = double.TryParse(input, out value);
                if (!isValid)
                {
                    Console.WriteLine($"Error! Please provide a proper double value)");
                }
            } while (!isValid);
            return value;
        }
        
        /// <summary>
        /// This method handles the string request from the user, including null value check and empty string check
        /// </summary>
        /// <param name="msg">The display message for the user</param>
        /// <returns>The valid string value</returns>
        private static string requestStringValue(string msg)
        {
            string value;
            bool isValid;
            do
            {
                Console.Write(msg);
                value = Console.ReadLine() ?? "";
                isValid = value.Length != 0;
                if (!isValid)
                {
                    Console.WriteLine($"Error! Please provide a proper string value)");
                }
            } while (!isValid);
            return value;
        }
       
    }
}
