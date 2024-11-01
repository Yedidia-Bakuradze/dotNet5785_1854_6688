
using System.Runtime.Intrinsics.Arm;

namespace Stage0
{
    partial class Program
    {
        static void Main(string[] args)
        {
            Welcome1854();
            Welcome6688();
            Console.ReadKey();
        }

        static partial void Welcome6688();

        /// <summary>
        /// Yedidia's welcome method
        /// </summary>
        private static void Welcome1854()
        {
            Console.WriteLine("Enter your name: ");
            string userName = Console.ReadLine();
            Console.WriteLine("{0}, welcome to my first console application", userName);
            Console.ReadKey();

        }
    }
}
