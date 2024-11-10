
namespace DalTest;

using Dal;
using DalApi;
using DO;

static public class Initialization
{
    private static IAssignment? s_dalAssignment;
    private static ICall? s_dalCall;
    private static IVolunteer? s_dalVolunteer;
    private static IConfig? s_dalConfig;

    private static readonly Random s_rand = new();

    static string[] names = new string[] {
    "John Smith", "Jane Doe", "Michael Johnson", "Emily Davis", "David Brown",
    "Sarah Wilson", "James Jones", "Jessica Garcia", "Robert Miller", "Mary Martinez",
    "William Anderson", "Linda Taylor", "Richard Thomas", "Barbara Hernandez", "Joseph Moore",
    "Susan Martin", "Charles Jackson", "Margaret Thompson", "Christopher White", "Patricia Harris",
    "Daniel Lewis", "Jennifer Clark", "Matthew Robinson", "Elizabeth Walker", "Anthony Hall",
    "Karen Allen", "Mark Young", "Nancy King", "Paul Wright", "Betty Scott",
    "Steven Green", "Sandra Adams", "Andrew Baker", "Ashley Gonzalez", "Joshua Nelson",
    "Kimberly Carter", "Kevin Mitchell", "Donna Perez", "Brian Roberts", "Carol Turner",
    "Edward Phillips", "Michelle Campbell", "Ronald Parker", "Amanda Evans", "Timothy Edwards",
    "Melissa Collins", "Jason Stewart", "Deborah Sanchez", "Jeffrey Morris", "Stephanie Rogers",
    "Ryan Reed", "Rebecca Cook", "Jacob Morgan", "Laura Bell", "Gary Murphy",
    "Sharon Bailey", "Nicholas Rivera", "Cynthia Cooper", "Eric Richardson", "Kathleen Cox",
    "Stephen Howard", "Amy Ward", "Jonathan Torres", "Angela Peterson", "Larry Gray",
    "Helen Ramirez", "Scott James", "Anna Watson", "Frank Brooks", "Ruth Kelly",
    "Justin Sanders", "Brenda Price", "Brandon Bennett", "Pamela Wood", "Samuel Barnes",
    "Nicole Ross", "Gregory Henderson", "Katherine Coleman", "Benjamin Jenkins", "Christine Perry",
    "Patrick Powell", "Samantha Long", "Raymond Patterson", "Janet Hughes", "Jack Flores",
    "Maria Washington", "Dennis Butler", "Heather Simmons", "Jerry Foster", "Diane Gonzales",
    "Tyler Bryant", "Rachel Alexander", "Aaron Russell", "Catherine Griffin", "Henry Diaz",
    "Julie Hayes", "Adam Myers", "Victoria Ford", "Nathan Hamilton", "Megan Graham"
};
    static string[] emails = new string[] {
    "john.smith@example.com", "jane.doe@example.com", "michael.johnson@example.com", "emily.davis@example.com", "david.brown@example.com",
    "sarah.wilson@example.com", "james.jones@example.com", "jessica.garcia@example.com", "robert.miller@example.com", "mary.martinez@example.com",
    "william.anderson@example.com", "linda.taylor@example.com", "richard.thomas@example.com", "barbara.hernandez@example.com", "joseph.moore@example.com",
    "susan.martin@example.com", "charles.jackson@example.com", "margaret.thompson@example.com", "christopher.white@example.com", "patricia.harris@example.com",
    "daniel.lewis@example.com", "jennifer.clark@example.com", "matthew.robinson@example.com", "elizabeth.walker@example.com", "anthony.hall@example.com",
    "karen.allen@example.com", "mark.young@example.com", "nancy.king@example.com", "paul.wright@example.com", "betty.scott@example.com",
    "steven.green@example.com", "sandra.adams@example.com", "andrew.baker@example.com", "ashley.gonzalez@example.com", "joshua.nelson@example.com",
    "kimberly.carter@example.com", "kevin.mitchell@example.com", "donna.perez@example.com", "brian.roberts@example.com", "carol.turner@example.com",
    "edward.phillips@example.com", "michelle.campbell@example.com", "ronald.parker@example.com", "amanda.evans@example.com", "timothy.edwards@example.com",
    "melissa.collins@example.com", "jason.stewart@example.com", "deborah.sanchez@example.com", "jeffrey.morris@example.com", "stephanie.rogers@example.com",
    "ryan.reed@example.com", "rebecca.cook@example.com", "jacob.morgan@example.com", "laura.bell@example.com", "gary.murphy@example.com",
    "sharon.bailey@example.com", "nicholas.rivera@example.com", "cynthia.cooper@example.com", "eric.richardson@example.com", "kathleen.cox@example.com",
    "stephen.howard@example.com", "amy.ward@example.com", "jonathan.torres@example.com", "angela.peterson@example.com", "larry.gray@example.com",
    "helen.ramirez@example.com", "scott.james@example.com", "anna.watson@example.com", "frank.brooks@example.com", "ruth.kelly@example.com",
    "justin.sanders@example.com", "brenda.price@example.com", "brandon.bennett@example.com", "pamela.wood@example.com", "samuel.barnes@example.com",
    "nicole.ross@example.com", "gregory.henderson@example.com", "katherine.coleman@example.com", "benjamin.jenkins@example.com", "christine.perry@example.com",
    "patrick.powell@example.com", "samantha.long@example.com", "raymond.patterson@example.com", "janet.hughes@example.com", "jack.flores@example.com",
    "maria.washington@example.com", "dennis.butler@example.com", "heather.simmons@example.com", "jerry.foster@example.com", "diane.gonzales@example.com",
    "tyler.bryant@example.com", "rachel.alexander@example.com", "aaron.russell@example.com", "catherine.griffin@example.com", "henry.diaz@example.com",
    "julie.hayes@example.com", "adam.myers@example.com", "victoria.ford@example.com", "nathan.hamilton@example.com", "megan.graham@example.com"
};
    static string[] addresses = new string[] {
"123 Main St, Springfield, IL 62701",
"456 Elm St, Denver, CO 80202",
"789 Maple Ave, Austin, TX 73301",
"101 Oak St, Seattle, WA 98101",
"202 Pine St, Boston, MA 02108",
"303 Cedar St, Miami, FL 33101",
"404 Birch St, San Francisco, CA 94101",
"505 Walnut St, Chicago, IL 60601",
"606 Chestnut St, Dallas, TX 75201",
"707 Ash St, Atlanta, GA 30301",
"808 Poplar St, Portland, OR 97201",
"909 Willow St, Phoenix, AZ 85001",
"1010 Spruce St, Philadelphia, PA 19101",
"1111 Fir St, Minneapolis, MN 55401",
"1212 Redwood St, San Diego, CA 92101",
"1313 Cypress St, Houston, TX 77001",
"1414 Palm St, Las Vegas, NV 89101",
"1515 Magnolia St, Orlando, FL 32801",
"1616 Sycamore St, New York, NY 10001",
"1717 Dogwood St, Los Angeles, CA 90001",
"1818 Hickory St, San Antonio, TX 78201",
"1919 Juniper St, San Jose, CA 95101",
"2020 Laurel St, Columbus, OH 43201",
"2121 Linden St, Charlotte, NC 28201",
"2222 Olive St, Indianapolis, IN 46201",
"2323 Palm St, Jacksonville, FL 32201",
"2424 Pine St, Fort Worth, TX 76101",
"2525 Maple St, Detroit, MI 48201",
"2626 Oak St, Memphis, TN 38101",
"2727 Cedar St, Baltimore, MD 21201",
"2828 Birch St, Milwaukee, WI 53201",
"2929 Walnut St, Albuquerque, NM 87101",
"3030 Chestnut St, Tucson, AZ 85701",
"3131 Ash St, Fresno, CA 93701",
"3232 Poplar St, Sacramento, CA 94203",
"3333 Willow St, Kansas City, MO 64101",
"3434 Spruce St, Mesa, AZ 85201",
"3535 Fir St, Omaha, NE 68101",
"3636 Redwood St, Colorado Springs, CO 80901",
"3737 Cypress St, Raleigh, NC 27601",
"3838 Palm St, Miami, FL 33101",
"3939 Magnolia St, Long Beach, CA 90801",
"4040 Sycamore St, Virginia Beach, VA 23450",
"4141 Dogwood St, Oakland, CA 94601",
"4242 Hickory St, Minneapolis, MN 55401",
"4343 Juniper St, Tulsa, OK 74101",
"4444 Laurel St, Tampa, FL 33601",
"4545 Linden St, Arlington, TX 76001",
"4646 Olive St, New Orleans, LA 70112",
"4747 Palm St, Wichita, KS 67201",
"4848 Pine St, Cleveland, OH 44101",
"4949 Maple St, Bakersfield, CA 93301",
"5050 Oak St, Aurora, CO 80010",
"5151 Cedar St, Anaheim, CA 92801",
"5252 Birch St, Honolulu, HI 96801",
"5353 Walnut St, Santa Ana, CA 92701",
"5454 Chestnut St, Riverside, CA 92501",
"5555 Ash St, Corpus Christi, TX 78401",
"5656 Poplar St, Lexington, KY 40502",
"5757 Willow St, Stockton, CA 95201",
"5858 Spruce St, St. Louis, MO 63101",
"5959 Fir St, Cincinnati, OH 45201",
"6060 Redwood St, Pittsburgh, PA 15201",
"6161 Cypress St, Anchorage, AK 99501",
"6262 Palm St, Henderson, NV 89002",
"6363 Magnolia St, Greensboro, NC 27401",
"6464 Sycamore St, Plano, TX 75023",
"6565 Dogwood St, Lincoln, NE 68501",
"6666 Hickory St, Buffalo, NY 14201",
"6767 Juniper St, Fort Wayne, IN 46801",
"6868 Laurel St, Jersey City, NJ 07302",
"6969 Linden St, Chula Vista, CA 91910",
"7070 Olive St, Norfolk, VA 23501",
"7171 Palm St, Orlando, FL 32801",
"7272 Pine St, St. Petersburg, FL 33701",
"7373 Maple St, Laredo, TX 78040",
"7474 Oak St, Madison, WI 53701",
"7575 Cedar St, Durham, NC 27701",
"7676 Birch St, Lubbock, TX 79401",
"7777 Walnut St, Winston-Salem, NC 27101",
"7878 Chestnut St, Garland, TX 75040",
"7979 Ash St, Glendale, AZ 85301",
"8080 Poplar St, Hialeah, FL 33010",
"8181 Willow St, Reno, NV 89501",
"8282 Spruce St, Baton Rouge, LA 70801",
"8383 Fir St, Irvine, CA 92602",
"8484 Redwood St, Chesapeake, VA 23320",
"8585 Cypress St, Scottsdale, AZ 85250",
"8686 Palm St, North Las Vegas, NV 89030",
"8787 Magnolia St, Fremont, CA 94536",
"8888 Sycamore St, Gilbert, AZ 85233",
"8989 Dogwood St, San Bernardino, CA 92401",
"9090 Hickory St, Boise, ID 83701",
"9191 Juniper St, Birmingham, AL 35201",
"9292 Laurel St, Rochester, NY 14602",
"9393 Linden St, Richmond, VA 23218",
"9494 Olive St, Spokane, WA 99201",
"9595 Palm St, Des Moines, IA 50301",
"9696 Pine St, Modesto, CA 95350",
"9797 Maple St, Fayetteville, NC 28301",
"9898 Oak St, Tacoma, WA 98401",
"9999 Cedar St, Oxnard, CA 93030"
};
    static int[] ids = new int[] {
    123456789, 234567890, 345678901, 456789012, 567890123,
    678901234, 789012345, 890123456, 901234567, 112345678,
    223456789, 334567890, 445678901, 556789012, 667890123,
    778901234, 889012345, 990123456, 101234567, 212345678,
    323456789, 434567890, 545678901, 656789012, 767890123,
    878901234, 989012345, 100123456, 211234567, 322345678,
    433456789, 544567890, 655678901, 766789012, 877890123,
    988901234, 109012345, 210123456, 321234567, 432345678,
    543456789, 654567890, 765678901, 876789012, 987890123,
    109901234, 210012345, 321123456, 432234567, 543345678,
    654456789, 765567890, 876678901, 987789012, 109890123,
    210901234, 321012345, 432123456, 543234567, 654345678,
    765456789, 876567890, 987678901, 109789012, 210890123,
    321901234, 432012345, 543123456, 654234567, 765345678,
    876456789, 987567890, 109678901, 210789012, 321890123,
    432901234, 543012345, 654123456, 765234567, 876345678,
    987456789, 109567890, 210678901, 321789012, 432890123,
    543901234, 654012345, 765123456, 876234567, 987345678,
    109456789, 210567890, 321678901, 432789012, 543890123
};
    static string[] phoneNumbers = new string[] {
    "050-1234567", "050-2345678", "050-3456789", "050-4567890", "050-5678901",
    "050-6789012", "050-7890123", "050-8901234", "050-9012345", "050-0123456",
    "052-1234567", "052-2345678", "052-3456789", "052-4567890", "052-5678901",
    "052-6789012", "052-7890123", "052-8901234", "052-9012345", "052-0123456",
    "053-1234567", "053-2345678", "053-3456789", "053-4567890", "053-5678901",
    "053-6789012", "053-7890123", "053-8901234", "053-9012345", "053-0123456",
    "054-1234567", "054-2345678", "054-3456789", "054-4567890", "054-5678901",
    "054-6789012", "054-7890123", "054-8901234", "054-9012345", "054-0123456",
    "055-1234567", "055-2345678", "055-3456789", "055-4567890", "055-5678901",
    "055-6789012", "055-7890123", "055-8901234", "055-9012345", "055-0123456",
    "056-1234567", "056-2345678", "056-3456789", "056-4567890", "056-5678901",
    "056-6789012", "056-7890123", "056-8901234", "056-9012345", "056-0123456",
    "057-1234567", "057-2345678", "057-3456789", "057-4567890", "057-5678901",
    "057-6789012", "057-7890123", "057-8901234", "057-9012345", "057-0123456",
    "058-1234567", "058-2345678", "058-3456789", "058-4567890", "058-5678901",
    "058-6789012", "058-7890123", "058-8901234", "058-9012345", "058-0123456",
    "059-1234567", "059-2345678", "059-3456789", "059-4567890", "059-5678901",
    "059-6789012", "059-7890123", "059-8901234", "059-9012345", "059-0123456",
    "050-1111111", "050-2222222", "050-3333333", "050-4444444", "050-5555555",
    "050-6666666", "050-7777777", "050-8888888", "050-9999999", "050-0000000"
};
    static string[] passwords = new string[] {
    "P@ssw0rd123", "Qwerty!234", "A1b2C3d4E5", "Zxcvbnm!23", "Passw0rd!@#",
    "SecureP@ss1", "MyP@ssw0rd2", "Admin!23456", "UserP@ss789", "Login!@#123",
    "P@ssword!23", "Qwerty!@#45", "A1b2C3!@#4", "Zxcvbn!@#56", "Pass!@#7890",
    "Secure!@#123", "MyP@ss!@#45", "Admin!@#678", "User!@#9012", "Login!@#345",
    "P@ssw0rd!@#", "Qwerty!@#67", "A1b2C3!@#8", "Zxcvbn!@#90", "Pass!@#1234",
    "Secure!@#567", "MyP@ss!@#89", "Admin!@#012", "User!@#3456", "Login!@#789",
    "P@ssw0rd!@#1", "Qwerty!@#23", "A1b2C3!@#4", "Zxcvbn!@#56", "Pass!@#7890",
    "Secure!@#123", "MyP@ss!@#45", "Admin!@#678", "User!@#9012", "Login!@#345",
    "P@ssw0rd!@#2", "Qwerty!@#34", "A1b2C3!@#5", "Zxcvbn!@#67", "Pass!@#8901",
    "Secure!@#234", "MyP@ss!@#56", "Admin!@#789", "User!@#0123", "Login!@#456",
    "P@ssw0rd!@#3", "Qwerty!@#45", "A1b2C3!@#6", "Zxcvbn!@#78", "Pass!@#9012",
    "Secure!@#345", "MyP@ss!@#67", "Admin!@#890", "User!@#1234", "Login!@#567",
    "P@ssw0rd!@#4", "Qwerty!@#56", "A1b2C3!@#7", "Zxcvbn!@#89", "Pass!@#0123",
    "Secure!@#456", "MyP@ss!@#78", "Admin!@#901", "User!@#2345", "Login!@#678",
    "P@ssw0rd!@#5", "Qwerty!@#67", "A1b2C3!@#8", "Zxcvbn!@#90", "Pass!@#1234",
    "Secure!@#567", "MyP@ss!@#89", "Admin!@#012", "User!@#3456", "Login!@#789",
    "P@ssw0rd!@#6", "Qwerty!@#78", "A1b2C3!@#9", "Zxcvbn!@#01", "Pass!@#2345",
    "Secure!@#678", "MyP@ss!@#90", "Admin!@#123", "User!@#4567", "Login!@#890"
};
    static double[] latitudes = new double[100] {
    32.0853, 31.7683, 32.7940, 31.0461, 32.7940,
    32.1093, 31.2520, 32.7940, 31.0461, 32.7940,
    32.0853, 31.7683, 32.7940, 31.0461, 32.7940,
    32.1093, 31.2520, 32.7940, 31.0461, 32.7940,
    32.0853, 31.7683, 32.7940, 31.0461, 32.7940,
    32.1093, 31.2520, 32.7940, 31.0461, 32.7940,
    32.0853, 31.7683, 32.7940, 31.0461, 32.7940,
    32.1093, 31.2520, 32.7940, 31.0461, 32.7940,
    32.0853, 31.7683, 32.7940, 31.0461, 32.7940,
    32.1093, 31.2520, 32.7940, 31.0461, 32.7940,
    32.0853, 31.7683, 32.7940, 31.0461, 32.7940,
    32.1093, 31.2520, 32.7940, 31.0461, 32.7940,
    32.0853, 31.7683, 32.7940, 31.0461, 32.7940,
    32.1093, 31.2520, 32.7940, 31.0461, 32.7940,
    32.0853, 31.7683, 32.7940, 31.0461, 32.7940,
    32.1093, 31.2520, 32.7940, 31.0461, 32.7940,
    32.0853, 31.7683, 32.7940, 31.0461, 32.7940,
    32.1093, 31.2520, 32.7940, 31.0461, 32.7940,
    32.0853, 31.7683, 32.7940, 31.0461, 32.7940,
    32.1093, 31.2520, 32.7940, 31.0461, 32.7940
};
    static double[] longitudes = new double[100] {
    34.7818, 35.2137, 34.9896, 34.8516, 34.9896,
    34.8555, 34.7818, 34.9896, 34.8516, 34.9896,
    34.7818, 35.2137, 34.9896, 34.8516, 34.9896,
    34.8555, 34.7818, 34.9896, 34.8516, 34.9896,
    34.7818, 35.2137, 34.9896, 34.8516, 34.9896,
    34.8555, 34.7818, 34.9896, 34.8516, 34.9896,
    34.7818, 35.2137, 34.9896, 34.8516, 34.9896,
    34.8555, 34.7818, 34.9896, 34.8516, 34.9896,
    34.7818, 35.2137, 34.9896, 34.8516, 34.9896,
    34.8555, 34.7818, 34.9896, 34.8516, 34.9896,
    34.7818, 35.2137, 34.9896, 34.8516, 34.9896,
    34.8555, 34.7818, 34.9896, 34.8516, 34.9896,
    34.7818, 35.2137, 34.9896, 34.8516, 34.9896,
    34.8555, 34.7818, 34.9896, 34.8516, 34.9896,
    34.7818, 35.2137, 34.9896, 34.8516, 34.9896,
    34.8555, 34.7818, 34.9896, 34.8516, 34.9896,
    34.7818, 35.2137, 34.9896, 34.8516, 34.9896,
    34.8555, 34.7818, 34.9896, 34.8516, 34.9896,
    34.7818, 35.2137, 34.9896, 34.8516, 34.9896,
    34.8555, 34.7818, 34.9896, 34.8516, 34.9896
};

    /// <summary>
    /// Creates Assignment's instances for the db
    /// </summary>
    /// <exception cref="Exception">Throws an exception if the calls database hasn't been generated</exception>
    private static void CreateAssignments()
    {
        //Gets the list of all the calls from the db
        List<Call> listOfCalls = s_dalCall?.ReadAll()
             ?? throw new Exception("List of Calls hasn't been generated yet");
        //Creates for each member a 
        for (int i = 0; i < 100; i++) {
            {
                Call currentCall = listOfCalls[i];

                //Calculates the delta time between the opening and closing time of the call    
                TimeSpan delta = (TimeSpan)(currentCall.DeadLine! - currentCall.OpeningTime);

                //Sets the start and end date based on the delta time that has been calculated   
                DateTime start = s_dalConfig!.Clock.AddDays(delta.Days);
                DateTime end = start.AddDays(s_rand.Next(0, 31));

                //Creates the assignment object - the id is generated in the CRUD's create method so there is no need to provide one here 
                Assignment newAssignment = new()
                {
                    Called = currentCall.Id,
                    VolunteerId = ids[i],
                    TimeOfStarting = start,
                    TimeOfEnding = end,
                    TypeOfEnding =
                    (end > currentCall.DeadLine) ? TypeOfEnding.CancellationExpired
                    : (i < 15) ? TypeOfEnding.SelfCanceled
                    : (i < 30) ? TypeOfEnding.AdminCanceled
                    : TypeOfEnding.Treated
                };
            }

        }
    }
    private static void createCalls()
    {

    }
    private static void createVolunteers()
    {
        for (int i = 0; i < 100; i++)
        {
            Volunteer newVolunteer = new Volunteer
            {
                Id = ids[i],// Valid check digit
                Role = (i == 0) ? Roles.Admin : Roles.Volunteer,
                FullName = names[i],
                PhoneNumber = phoneNumbers[i],//Starts with 0, 10 digit long
                Email = emails[i],
                MaxDistanceToCall = s_rand.Next(30),
                TypeOfRange =
                (i % 3 != 0)
                ? TypeOfRange.AirDistance
                : (i % 5 != 0)
                ? TypeOfRange.walkingDistance
                : TypeOfRange.drivingDistance,
                Active = (i % 3 != 0 || i % 2 != 0 || i == 0) ? true : false,
                Password = passwords[i],
                FullCurrentAddress = addresses[i],
                Latitude = null,
                Longitude = null
            };
            if (s_dalVolunteer?.Read(newVolunteer.Id) == null)
                s_dalVolunteer?.Create(newVolunteer);
        }

    }

}
