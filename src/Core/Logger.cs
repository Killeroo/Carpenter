using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carpenter
{
    public static class Logger
    {
        public static void DebugLog(string message)
        {
            Console.WriteLine($"[{DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ffff")}] {message}");
            Console.ResetColor();
        }

        public static void DebugError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[{DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ffff")}] {message}");
            Console.ResetColor();
        }
    }
}
