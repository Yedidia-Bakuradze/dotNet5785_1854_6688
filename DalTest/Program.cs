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
        public enum ClassType { Assignment, Call, Volunteer };
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
                                ConfigSubMenu();
                                break;
                            case MainMenuEnum.ResetSysAndDb:
                                ResetDbAndSystem();
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
        /// Displays the submenu for a specific class and handles user input.
        /// </summary>
        /// <param name="classId">The class type for the submenu.</param>
        private static void ShowSubMenu(ClassType classId)
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
                                CreateDbAction(classId);
                                break;
                            case ClassSubMenuEnum.Read:
                                //TODO: ReadDbAction(classId);
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
        /// Performs the create action for a specific class.
        /// </summary>
        /// <param name="classId">The class type for the create action.</param>
        private static void CreateDbAction(ClassType classId)
        {
            //Getting data for the new instance
            switch (classId)
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
