using BO;
using System.Globalization;
using System.Runtime.InteropServices;

namespace BlTest
{
    internal class Program
    {
        //Main BL Action manager for the bl layer
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public enum Operation { Exit, Call, Volunteer, Admin}
        public enum IVolunteerOperations { Exit, Add, Remove, Update, Read, ReadAll, Login}
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }


        public void MainMenu()
        {
            do
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
                            IVolunteerSubMenu();
                            break;
                        case Operation.Admin:
                            break;
                    }
                }
                catch(Exception ex)
                {
                    ExceptionDisplay(ex);
                }

            } while (true);
        }


        public void ICallSubMenu()
        {

        }

        static public void ExceptionDisplay(Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("To Continue: Please Press Enter");
            Console.Read();
        }
        static public void IVolunteerSubMenu()
        {
            do
            {
                try
                {
                    //Get operation from user
                    Console.WriteLine(
        @"
        ----------------------------------------------------------------
        Volunteer SubMenu: Please Select One Of The Presented Options

        Press 1: To Add a New Volunteer
        Press 2: To Remove an Volunteer
        Press 3: To Update a Volunteer
        Press 4: To Read a Volunteer
        Press 5: To Read All Volunteers 
        Press 6: To Login as a Volunteer
        Press 0: To Exit
        ----------------------------------------------------------------

        >>> 
        "
        );
                    IVolunteerOperations operation;
                    string input = Console.ReadLine() ?? "";

                    if (!Enum.TryParse(input, out operation))
                        throw new BO.BlInvalidEnumValueOperationException($"Bl: There is not such a opereation as {input}");

                    switch (operation)
                    {
                        case IVolunteerOperations.Exit:
                            return;
                        case IVolunteerOperations.Add:
                            AddVolunteer();
                            break;
                        case IVolunteerOperations.Remove:
                            RemoveVolunteer();
                            break;
                        case IVolunteerOperations.Update:
                            UpdateVolunteer();
                            break;
                        case IVolunteerOperations.Read:
                            ReadVolunteer();
                            break;
                        case IVolunteerOperations.ReadAll:
                            ReadAllVolunteers();
                            break;
                        case IVolunteerOperations.Login:
                            LoginVolunteer();
                            break;
                    }
                }
                catch(Exception ex)
                {
                    ExceptionDisplay(ex);
                }
            } while (true);

        }

        private static void LoginVolunteer()
        {
            Console.Write("Enter Your Username (Email Address): ");
            string emailAddress = Console.ReadLine() ?? "";
            Console.Write("Enter Your Password: ");
            string password = Console.ReadLine() ?? "";

            string userType = s_bl.Volunteer.Login(emailAddress,password);
            Console.WriteLine($"The account under the email address of: {emailAddress} is a {userType}");
        }

        private static void ReadAllVolunteers()
        {
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("Displaying all the volunteers");
            foreach(BO.VolunteerInList volunteer in s_bl.Volunteer.GetVolunteers(null, null))
            {
                Console.WriteLine(volunteer);
            }
            Console.WriteLine("-------------------------------------------");
        }

        private static void ReadVolunteer()
        {
            Console.WriteLine("Read Volulnteer Action:");

            Console.Write("Enter the Id of the volunteer to read: ");
            string input = Console.ReadLine() ?? "";
            if (!Int32.TryParse(input, out int id))
                throw new BO.BlInvalidValueTypeToFormatException($"Bl: The value {input}, is not a integer");
            else
                s_bl.Volunteer.GetVolunteerDetails(id);
        }

        private static void UpdateVolunteer()
        {
            int updaterId;
            int id;
            Console.WriteLine("Volunteer Update Mode:");
            Console.Write("Enter Your Id: ");
            Int32.TryParse(Console.ReadLine() ?? "", out updaterId);

            Console.WriteLine("Volunteer's New Field Values:");

            Console.Write("Enter new Id: ");
            Int32.TryParse(Console.ReadLine() ?? "", out id);

            BO.Volunteer currentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);

            if (RequestPremissionToChanged("Full Name"))
            {
                Console.Write("Enter new Full Name: ");
                currentVolunteer.FullName = Console.ReadLine() ?? "";
            }

            if (RequestPremissionToChanged("Phone Number"))
            {
                Console.Write("Enter new Phone Number: ");
                currentVolunteer.PhoneNumber = Console.ReadLine() ?? "";
            }

            if (RequestPremissionToChanged("Email Address"))
            {
                Console.Write("Enter new Email Address: ");
                currentVolunteer.Email = Console.ReadLine() ?? "";
            }

            if (RequestPremissionToChanged("Password"))
            {
                Console.Write("[Optional] Enter new Password: ");
                currentVolunteer.Password = Console.ReadLine() ?? null;
            }

            if (RequestPremissionToChanged("Current Address"))
            {
                Console.Write("[Optional] Enter new Current Address: ");
                currentVolunteer.FullCurrentAddress = Console.ReadLine() ?? null;
            }

            if (RequestPremissionToChanged("User Mode"))
            {
                Console.Write($"Enter new User Mode ({BO.UserRole.Volunteer} / {BO.UserRole.Admin}): ");
                string input = Console.ReadLine() ?? "";
                if (!Enum.TryParse(input, out BO.UserRole role))
                    throw new BlInvalidValueTypeToFormatException($"Bl: Unable to convert the value {input}, to UserMode enum value");
                else
                    currentVolunteer.Role = role;
            }

            if (RequestPremissionToChanged("Active"))
            {
                Console.Write("Do You Want To Be an Active Volunteer? (yes / no): ");
                string input = Console.ReadLine() ?? "";
                if (input == "yes")
                    currentVolunteer.IsActive = true;
                else if (input == "no")
                    currentVolunteer.IsActive = false;
                else
                    throw new BO.BlInvalidValueTypeToFormatException($"Bl: The value {input}, is not either 'yes' or 'not'");
            }

            if (RequestPremissionToChanged("Max Distance"))
            {
                Console.Write("Enter new Max Distance For Taking a Call: ");
                string input = Console.ReadLine() ?? "";
                if (!double.TryParse(input, out double maxDistanceToCall))
                    throw new BO.BlInvalidValueTypeToFormatException($"Bl: The value {input}, is not a double");
                else
                    currentVolunteer.MaxDistanceToCall = maxDistanceToCall;
            }

            if(RequestPremissionToChanged("Distance Range Type "))
            {
                Console.Write($"Enter new Distance Range Type ({BO.TypeOfRange.AirDistance} / {BO.TypeOfRange.WalkingDistance} / {BO.TypeOfRange.DrivingDistance}): ");
                string input = Console.ReadLine() ?? "";
                if (!Enum.TryParse(input, out BO.TypeOfRange rangeType))
                    throw new BO.BlInvalidValueTypeToFormatException($"Bl: The value {input}, is not a type of a range");
                else
                    currentVolunteer.RangeType = rangeType;

            }
            
            s_bl.Volunteer.UpdateVolunteerDetails(updaterId, currentVolunteer);
        }

        private static bool RequestPremissionToChanged(string requestedFieldChanged)
        {
            string input;
            do
            {
                Console.WriteLine($"Do You Want To Chanege the {requestedFieldChanged}? (yes / no)");
                input  = Console.ReadLine() ?? "";

                if (input != "yes" || input != "no")
                    Console.WriteLine($"Error: Input (Accepted: {input}) must be either a yes or a no");
                else
                    break;
            } while (true);
            
            return input == "yes";
        }

        /// <summary>
        /// This method accepts from the user an id of a volunteer to remove
        /// </summary>
        /// <exception cref="BO.BlInvalidValueTypeToFormatException">Thrown when the user enters unconvertable value</exception>
        private static void RemoveVolunteer()
        {
            int id;
            Console.WriteLine("Enter the Id of The Requiered Volunteer To Remove:");
            Console.Write(">>> ");
            string input = Console.ReadLine() ?? "";

            if (!Enum.TryParse(input, out id))
                throw new BO.BlInvalidValueTypeToFormatException($"Bl: Unable to convert:{input} ,to an integer");
            
            s_bl.Volunteer.DeleteVolunteer(id);
        }

        /// <summary>
        /// This method adds a new Volunteer to the database using the user's input values
        /// </summary>
        public static void AddVolunteer()
        {
            int id;
            string fullName;
            string phoneNumber;
            string? email;
            string? password;
            string? fullCurrentAddress;
            BO.UserRole role;
            bool isActive;
            double maxDistanceToCall;
            BO.TypeOfRange rangeType;

            Console.Write("Enter Your Id: ");
            Int32.TryParse(Console.ReadLine()??"",out id);
            
            Console.Write("Enter Your Full Name: ");
            fullName = Console.ReadLine()??"";

            Console.Write("Enter Your Phone Number: ");
            phoneNumber = Console.ReadLine() ?? "";

            Console.Write("Enter Your Email Address: ");
            email = Console.ReadLine() ?? "";

            Console.Write("[Optional] Enter Your Password: ");
            password = Console.ReadLine() ?? null;

            Console.Write("[Optional] Enter Your Current Address: ");
            fullCurrentAddress = Console.ReadLine() ?? null;

            Console.Write($"Enter Your User Mode ({BO.UserRole.Volunteer} / {BO.UserRole.Admin}): ");
            Enum.TryParse(Console.ReadLine() ?? "", out role); 

            Console.Write("Do You Want To Be an Active Volunteer? (yes / no)\n>>> ");
            bool.TryParse(Console.ReadLine() ?? "", out isActive);

            Console.Write("Enter Your Max Distance For Taking a Call: ");
            Double.TryParse(Console.ReadLine() ?? "", out maxDistanceToCall);

            Console.Write($"Enter Your Distance Range Type ({BO.TypeOfRange.AirDistance} / {BO.TypeOfRange.WalkingDistance} / {BO.TypeOfRange.DrivingDistance}): ");
            Enum.TryParse(Console.ReadLine() ?? "", out rangeType);

            BO.Volunteer newVolunteer = new BO.Volunteer
            {
                Id = id,
                Email = email,
                RangeType = rangeType,
                CurrentCall = null,
                FullCurrentAddress = fullCurrentAddress,
                FullName = fullName,
                IsActive = isActive,
                Latitude = null,
                Longitude = null,
                MaxDistanceToCall = maxDistanceToCall,
                Password = password,
                PhoneNumber = phoneNumber,
                Role = role
            };

            s_bl.Volunteer.AddVolunteer(newVolunteer);
        }

        public void IAdminSubMenu()
        {

        }
    }
}
