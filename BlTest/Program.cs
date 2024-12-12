using System.Threading.Channels;

namespace BlTest
{
    internal class Program
    {
        //Main BL Action manager for the bl layer
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public enum Operation { Exit, Call, Volunteer, Admin}

        static void Main(string[] args)
        {
            MainMenu();
        }


        public static void MainMenu()
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
            Operation operation;
            do
            {
                input = Console.ReadLine() ?? "";
                if(!Enum.TryParse(input,out operation))
                    throw new BO.BlInvalidEnumValueOperationException($"Bl: Enum value for the main window is not a valid operation");

                try
                {
                    switch (operation)
                    {
                        case Operation.Exit:
                            return;
                        case Operation.Call:
                            break;
                        case Operation.Volunteer:
                            break;
                        case Operation.Admin:
                            break;
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("To Continue Please Press Enter\n>>> ");
                    Console.ReadKey();

                }
            } while (true);
        }


        public void ICallSubMenu()
        {

        }


        public void IVolunteerSubMenu()
        {

        }

        public void IAdminSubMenu()
        {

        }
    }
}
