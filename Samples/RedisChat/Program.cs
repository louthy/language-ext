using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;

namespace RedisChat
{
    class Program
    {
        static void Main(string[] args)
        {
            GetRoomName();
            GetUsername();
        }

        private static string GetRoomName()
        {
            Console.Write("Please enter the room you wish to join: ");
            return Console.ReadLine();
        }

        private static string GetUsername()
        {
            Console.Write("Please enter your name: ");
            return Console.ReadLine();
        }
    }
}
