using System.Threading.Channels;

namespace BlTest
{
    internal class Program
    {
        //Main BL Action manager for the bl layer
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public enum MainMenuOperation { Exit, Call, Volunteer, Admin}
        public enum CallMenuOperation { Exit, AddCall, GetCall, UpdateCallSelectCallToDo, UpdateCallEnd, EndOfCallStatusUpdate, DeleteCallRequest, GetListOfCalls, GetDetielsOfCall ,GetClosedCallsByVolunteer, GetOpenCallsForVolunteer, GetTotalCallsByStatus,  GetAllCalls }

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
            MainMenuOperation operation;
            do
            {
                input = Console.ReadLine() ?? "";
                if(!Enum.TryParse(input,out operation))
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
                            break;
                        case MainMenuOperation.Admin:
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


        public void ICallSubMenu(CallMenuOperation callMenuOperation)
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
            CallMenuOperation Calloperation;
            do
            {
                input = Console.ReadLine() ?? "";
                if (!Enum.TryParse(input, out Calloperation))
                    throw new BO.BlInvalidEnumValueOperationException($"Bl: Enum value for the main window is not a valid operation");

                switch (Calloperation)
                {
                    case CallMenuOperation.Exit:
                        return;
                    case CallMenuOperation.AddCall:
                        
                        break;
                    case CallMenuOperation.GetCall:
                        break;
                    case CallMenuOperation.UpdateCallSelectCallToDo:
                        break;
                    case CallMenuOperation.UpdateCallEnd:
                        break;
                    case CallMenuOperation.EndOfCallStatusUpdate:
                        break;
                    case CallMenuOperation.DeleteCallRequest:
                        break;
                    case CallMenuOperation.GetListOfCalls:
                        break;
                    case CallMenuOperation.GetDetielsOfCall:
                        break;
                    case CallMenuOperation.GetClosedCallsByVolunteer:
                        break;
                    case CallMenuOperation.GetOpenCallsForVolunteer:
                        break;
                    case CallMenuOperation.GetTotalCallsByStatus:
                        break;
                    case CallMenuOperation.GetAllCalls:
                        break;
                }
            }
            while (true);
        }


        public void IVolunteerSubMenu()
        {

        }

        public void IAdminSubMenu()
        {

        }
    }
}
