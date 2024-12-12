namespace BlTest
{
    internal class Program
    {
        //Main BL Action manager for the bl layer
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }


        public void MainMenu()
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
        }
    }
}
