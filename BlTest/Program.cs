using BO;
using Helpers;
using System.Data.Common;
using System.Security.Cryptography.X509Certificates;

namespace BlTest;

internal class Program
{
    //Main BL Action manager for the bl layer
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    public enum MainMenuOperation { Exit, Call, Volunteer, Admin}
    public enum CallMenuOperation { Exit, AddCall,UpdateCall,SelectCallToDo, UpdateCallEnd, EndOfCallStatusUpdate, DeleteCallRequest, GetListOfCalls, GetDetielsOfCall ,GetClosedCallsByVolunteer, GetOpenCallsForVolunteer, GetTotalCallsByStatus }
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
        string? input;
        CallMenuOperation Calloperation;
        do
        {
            try
            {
                input = Console.ReadLine() ?? "";
                if (!Enum.TryParse(input, out Calloperation))
                    throw new BO.BlInvalidEnumValueOperationException($"Bl: Enum value for the main window is not a valid operation");

                switch (Calloperation)
                {
                    case CallMenuOperation.Exit:
                        return;
                    case CallMenuOperation.AddCall:
                        BO.CallType callType;
                        string CallAddress;
                        string Description;
                        DateTime DeadLine;
                        Console.WriteLine("Pls enter the type of the call:");
                        input = Console.ReadLine() ?? "";
                        if (!Enum.TryParse(input, out callType))
                        {
                            throw new BO.BlInvalidEnumValueOperationException($"Bl: Enum value for the main window is not a valid operation");
                        }
                        Console.WriteLine("Pls describe your call [optional]:");
                        Description = Console.ReadLine() ?? "";
                        Console.WriteLine("Pls enter the address of the call:");
                        CallAddress = Console.ReadLine() ?? "";
                        Console.WriteLine("What is the deadline for the call:");
                        DeadLine = DateTime.Parse(Console.ReadLine() ?? "");
                        if(DeadLine < s_bl.Admin.GetClock())
                        {
                            throw new BO.BlInvalidEnumValueOperationException($"Bl: The deadline for the call is invalid");
                        }
                        BO.Call call = new BO.Call()
                        {
                            TypeOfCall = callType,
                            Description = Description,
                            CallAddress = CallAddress,
                            CallStartTime = s_bl.Admin.GetClock(),
                            CallDeadLine = DeadLine
                        };
                        s_bl.Call.AddCall(call);
                        break;
                    case CallMenuOperation.UpdateCall:
                        BO.CallType callType1;
                        string CallAddress1;
                        string Description1;
                        DateTime DeadLine1;
                        string answer;
                        Console.WriteLine("Please give me the call ID you want to update:");
                        int callId = int.Parse(Console.ReadLine() ?? "");
                        BO.Call call1 = s_bl.Call.GetDetielsOfCall(callId);
                        Console.WriteLine("do you want to change the type of the call: Y/N");
                        answer = Console.ReadLine() ?? "";
                        if (answer == "Y")
                        {
                            Console.WriteLine("Pls enter the type of the call:");
                            input = Console.ReadLine() ?? "";
                            if (!Enum.TryParse(input, out callType1))
                            {
                                throw new BO.BlInvalidEnumValueOperationException($"Bl: Enum value for the main window is not a valid operation");
                            }
                            call1.TypeOfCall = callType1;
                        }
                        Console.WriteLine("do you want to change the description of the call: Y/N");
                        answer = Console.ReadLine() ?? "";
                        if (answer == "Y")
                        {
                            Console.WriteLine("Pls describe your call [optional]:");
                            Description1 = Console.ReadLine() ?? "";
                            call1.Description = Description1;
                        }
                        Console.WriteLine("Do you want to change the call adress: Y/N");
                        answer = Console.ReadLine() ?? "";
                        if (answer == "Y")
                        {
                            Console.WriteLine("Pls enter the address of the call:");
                            CallAddress1 = Console.ReadLine() ?? "";
                            call1.CallAddress = CallAddress1;
                        }

                        Console.WriteLine("Do you want to change the deadline of the call: Y/N");
                        answer = Console.ReadLine() ?? "";
                        if (answer == "Y")
                        {
                            Console.WriteLine("What is the deadline for the call:");
                            DeadLine1 = DateTime.Parse(Console.ReadLine() ?? "");
                            if (DeadLine1 < s_bl.Admin.GetClock())
                            {
                                throw new BO.BlInvalidEnumValueOperationException($"Bl: The deadline for the call is invalid");
                            }
                            call1.CallDeadLine = DeadLine1;
                        }
                        s_bl.Call.UpdateCall(call1);
                        break;
                    case CallMenuOperation.SelectCallToDo:
                        int callId1;
                        int VolunteerId;
                        Console.WriteLine("Please give me the call ID you want to select:");
                        callId1 = int.Parse(Console.ReadLine() ?? "");
                        Console.WriteLine("Please give me the volunteer ID you want to select:");
                        VolunteerId = int.Parse(Console.ReadLine() ?? "");
                        s_bl.Call.SelectCallToDo(VolunteerId, callId1);
                        break;
                    case CallMenuOperation.UpdateCallEnd:
                        int callId2;
                        int VolunteerId1;
                        Console.WriteLine("Please give me the call ID you want to update:");
                        callId2 = int.Parse(Console.ReadLine() ?? "");
                        Console.WriteLine("Please give me the volunteer ID you want to update:");
                        VolunteerId1 = int.Parse(Console.ReadLine() ?? "");
                        s_bl.Call.UpdateCallEnd(VolunteerId1, callId2);
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
                }
            }
            catch (Exception ex)
            {
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
