using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carpenter
{
    public enum LogLevel : byte
    {
        Verbose,
        Info,
        Warning,
        Error
    }

    /// <summary>
    /// A basic logging class used by Carpenter to log out different pieces of information to standard output
    /// </summary>
    public static class Logger
    { 
        private static ConsoleColor[] LogColor =
        {
            ConsoleColor.DarkGray,
            ConsoleColor.Gray,
            ConsoleColor.Yellow,
            ConsoleColor.Red
        };

        private static bool _showTimestamp = false;
        private static HashSet<LogLevel> _enabledLogLevels = new()
        {
            //LogLevel.Verbose,
            LogLevel.Info,
            LogLevel.Warning,
            LogLevel.Error
        };

        public static void Log(LogLevel level, string message)
        {
            if (_enabledLogLevels.Contains(level) == false)
            {
                return;
            }

            if ((int)level < LogColor.Length)
            {
                Console.ForegroundColor = LogColor[(int)level];
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Gray;
            }

            if (_showTimestamp)
            {
                Console.Write($"[{DateTime.Now.ToString("'HH':'mm':'ss'.'ffff")}] ");
            }

            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void EnableLevel(LogLevel level, bool bEnable)
        {
            if (bEnable == false)
            {
                _enabledLogLevels.Remove(level);
            }
            else if (!_enabledLogLevels.Contains(level))
            {
                _enabledLogLevels.Add(level);
            }
        }
    }
}
