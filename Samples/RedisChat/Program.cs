using System;
using LanguageExt;

namespace RedisChat
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("*** THIS EXAMPLE IS INCOMPLETE AND NOT WORKING!  ***");

            GetUsername();
        }

        private static string GetUsername()
        {
            Console.Write("Please enter your name: ");
            return Console.ReadLine();
        }
    }
}
