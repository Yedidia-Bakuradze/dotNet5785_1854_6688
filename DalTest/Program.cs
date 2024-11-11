using Dal;
using DalApi;
using DO;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;

namespace DalTest
{
    internal class Program
    {
        private static IAssignment ? s_dalAssignment  = new AssignmentImplementation();
        private static IVolunteer ? s_dalVolunteer  = new VolunteerImplementation();
        private static ICall ? s_dalCall  = new CallImplementation();
        private static IConfig? s_dalConfig = new ConfigImplementation();
        
        public enum MainMenuEnum { FirstRun,Exit,ShowAssignmentMenu,ShowCallMenu,ShowVolunteerMenu, DbInit, ShowAllDbData,ShowConfigMenu, ResetSysAndDb}
        public enum ClassSubMenuEnum{ FirstRun,Exit, Create,Read,ReadAll,Update,Delete,DeleteAll}
        public enum ClassType { Assignment,Call,Volunteer};
        static void Main(string[] args)
        {
            MainMenu();
        }

        private static void MainMenu()
        {
            MainMenuEnum operation = MainMenuEnum.FirstRun;
            while (operation != MainMenuEnum.Exit)
            {
                try
                {
                    Console.Write("Please choose an option: ");
                        string input = Console.ReadLine() ?? "";
                        if(Enum.TryParse(input,out operation))
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

                }catch(Exception error)
                {
                    Console.WriteLine(error.Message);
                }

            }
        }
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
                                ReadDbAction(classId);
                                break;
                            case ClassSubMenuEnum.ReadAll:
                                ReadAllDbAction(classId);
                                break;
                            case ClassSubMenuEnum.Update:
                                UpdateDbAction(classId);
                                break;
                            case ClassSubMenuEnum.Delete:
                                DeleteDbAction(classId);
                                break;
                            case ClassSubMenuEnum.DeleteAll:
                                DeleteAllDbAction(classId);
                                break;
                        }
                    }
                    catch (Exception error)
                    {
                        Console.WriteLine(error.Message);
                    }
                }
                else {
                    Console.WriteLine($"Invalid operation has been captured: {input}");
                }
            }
        }
        
        private static void CreateDbAction(ClassType classId)
        {
            //Getting data for the new instance
            switch (classId)
            {
                case ClassType.Assignment:


                    break;
                case ClassType.Call:
                    Console.WriteLine("Enter the id of the Call:");
                    int id1 = wdljdsasdd
                    Console.WriteLine("What is the call type:");
                    string input = Console.ReadLine();
                    CallTypes callTypes = Enum.TryParse(input, out callTypes);

                    Console.WriteLine("What is the adrees");
                    string adress = Console.ReadLine();

                    Console.WriteLine("what is the latitude of the adress:");
                    double latitude = double.Parse(Console.ReadLine()); 

                    Console.WriteLine("what is the longitude of the adress:");
                    double longitude = double.Parse(Console.ReadLine());

                    Console.WriteLine("what time as the call start:");
                    DateTime start = DateTime.Parse(Console.ReadLine());

                    Console.WriteLine("Please add description to the call:");
                    string description = Console.ReadLine();

                    Console.WriteLine("What is the Deadlin for the call:");
                    DateTime deadLine = DateTime.Parse(Console.ReadLine());

                    Call newcall = new Call 
                    {
                        Id = id1,
                        Type = callTypes,
                        FullAddressCall = adress,
                        Latitude =latitude,
                        Longitude = longitude,
                        OpeningTime =start,
                        Description = description,
                        DeadLine = deadLine

                    };

                    break;
                case ClassType.Volunteer:
                    Console.WriteLine("Enter the volunteer id:");
                    int id = int.Parse(Console.ReadLine());

                    Console.WriteLine("Enter the volunteer full name:");
                    string name = Console.ReadLine();

                    Console.WriteLine("Enter the Role of the Volunteer Admin/Volunteer:");
                    string input = Console.ReadLine();
                    Roles role = Enum.TryParse(input, out Roles);

                    Console.WriteLine("Enter the Volunteer phone number:");
                    string phonenumber = Console.ReadLine();

                    Console.WriteLine("Enter the Volunteer email:");
                    string email = Console.ReadLine();

                    Console.WriteLine("What is the max distance to call the volunteer:");
                    double maxDistance = double.Parse(Console.ReadLine());

                    Console.WriteLine("The volunteer is active ?");
                    bool active = bool.Parse(Console.ReadLine());

                    Console.WriteLine("Put your password:");
                    string password = Console.ReadLine();

                    Console.WriteLine("Put full your current adrees:");
                    string adress = Console.ReadLine();

                    string input1= Console.ReadLine();
                    TypeOfRange typeOfRange = Enum.TryParse(input1 , out TypeOfRange typeOfRange1);

                    Volunteer newVolunteer = new Volunteer
                    {
                        Id = id,
                        FullName = name,
                        Role = role,
                        PhoneNumber = phonenumber,
                        Email = email,
                        MaxDistanceToCall = maxDistance,
                        Active = active,
                        Password = password,
                        FullCurrentAddress = adress,
                        TypeOfRange = typeOfRange,
                    };
                    if(s_dalVolunteer?.Read(newVolunteer.Id) == null)
                    {
                        s_dalVolunteer?.Create(newVolunteer);
                    }

                    break;
            }

        }

        private static void DbInit() => DalTest.Initialization.Do(s_dalAssignment,s_dalCall,s_dalVolunteer,s_dalConfig);

    }
}
