using Dal;
using DalApi;
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
                    break;
                case ClassType.Volunteer:
                    break;
            }

        }

        private static void DbInit() => DalTest.Initialization.Do(s_dalAssignment,s_dalCall,s_dalVolunteer,s_dalConfig);

    }
}
