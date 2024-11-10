using Dal;
using DalApi;

namespace DalTest
{
    internal class Program
    {
        private static IAssignment ? s_dalAssignment  = new AssignmentImplementation();
        private static IVolunteer ? s_dalVolunteer  = new VolunteerImplementation();
        private static ICall ? s_dalCall  = new CallImplementation();
        private static IConfig? s_dalConfig = new ConfigImplementation();
        public enum MainHub {
            Exit, SubMenu1, SubMenu2 , SubMenu3,DbInit,ShowDbData, ConfigSubMenu,ResetDbAndSystem,
            ExitSubMenu, Create,Read,ReadAll,Update,Delete,DeleteAll,
            InputEntityData, CreateNewObject,
        };
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Please choose an option: ");
                Console.ReadLine();

            }catch(Exception error)
            {
                Console.WriteLine(error.Message);
            }
        }

        static void MainMenu()
        {

        }
        static void SubMenu()
        {

        }

        static void AddEntity()
        {

        }

        static void DisplayObject()
        {

        }

        static void ConfigSubMenu()
        {

        }


    }
}
