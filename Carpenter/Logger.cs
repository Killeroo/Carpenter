using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Carpenter
{
    public enum LogLevel : byte
    {
        Error,
        Warning,
        Info,
        Verbose
    }

    /// <summary>
    /// A basic logging class used by Carpenter to log out different pieces of information to standard output
    /// </summary>
    public static class Logger
    { 
        private static readonly Dictionary<LogLevel, ConsoleColor> LogColors = new()
        {
            { LogLevel.Error ,ConsoleColor.Red },
            { LogLevel.Warning ,ConsoleColor.Yellow },
            { LogLevel.Info ,ConsoleColor.Gray },
            { LogLevel.Verbose ,ConsoleColor.DarkGray }
        };

        private static bool _showTimestamp = true;
        private static bool _showFilename = true;
        private static bool _showCurrentThread = true;
        private static LogLevel _currentLogLevel = LogLevel.Info;
        private static Dictionary<string, string> _cachedFileNames = new();

        private class LogEntry
        {
            public LogLevel LogLevel;
            public string Message;
            public string Source;
            public string Timestamp;
            public int ThreadId;
        }

        private static Thread _loggingThread = null;
        private static Object _loggingLock = new();
        private static BlockingCollection<LogEntry> _messageQueue = new();
        private static ConcurrentBag<LogEntry> _messagePool = new();

        public static void Log(LogLevel level, string message, [CallerFilePath] string sourceFilePath = "")
        {
            if (level > _currentLogLevel)
            {
                return;
            }

            lock (_loggingLock)
            {
                if (_loggingThread == null)
                {
                    _loggingThread = new Thread(() => ProcessLogBuffer()) { IsBackground = true };
                    _loggingThread.Start();
                    
                    AppDomain.CurrentDomain.ProcessExit += (sender, args) =>
                    {
                        _messageQueue.CompleteAdding();
                        _loggingThread.Join(2000);
                    };
                }
            }
            
            if (!_messagePool.TryTake(out var logEntry))
            {
                logEntry = new LogEntry();
            }
            logEntry.LogLevel = level;
            logEntry.Message = message;
            logEntry.Source = sourceFilePath;
            logEntry.ThreadId = Environment.CurrentManagedThreadId;
            logEntry.Timestamp = DateTime.Now.ToString("HH:mm:ss.ff");
            _messageQueue.Add(logEntry);
        }

        private static void ProcessLogBuffer()
        {
            foreach (LogEntry entry in _messageQueue.GetConsumingEnumerable())
            {
                if (!LogColors.TryGetValue(entry.LogLevel, out ConsoleColor logColor))
                {
                    logColor = ConsoleColor.Gray;
                }
                Console.ForegroundColor = logColor;

                if (_showTimestamp)
                {
                    Console.Write($"[{entry.Timestamp}] ");
                }

                if (_showCurrentThread) 
                {
                    ConsoleColor threadColor = (ConsoleColor) (((entry.ThreadId * 23) ^ (entry.ThreadId >> 6)) % 16);
                    Console.ForegroundColor = threadColor;
                    Console.Write($"[{entry.ThreadId}] ");
                    Console.ForegroundColor = logColor;
                }

                if (_showFilename && !string.IsNullOrEmpty(entry.Source))
                {
                    if (!_cachedFileNames.TryGetValue(entry.Source, out string formattedFileName))
                    {
                        int lastDirChar = entry.Source.LastIndexOf(Path.DirectorySeparatorChar);
                        if (lastDirChar != -1)
                        {
                            formattedFileName = entry.Source.Substring(
                                lastDirChar + 1,
                                entry.Source.Length - lastDirChar - 1 /* Skip dir separator */ - 3 /* Skip '.cs' */);
                            _cachedFileNames.Add(entry.Source, formattedFileName);
                        }
                    }

                    Console.Write($"[{formattedFileName}] ");
                }

                Console.WriteLine(entry.Message);
                Console.ResetColor();

                _messagePool.Add(entry);
            }
        }

        private static ConsoleColor GetColorForInt(int input)
        {
            ConsoleColor color;
            
            int offset = 0;
            do
            {
                color = (ConsoleColor)(((input + offset * 23) ^ (input + offset >> 4)) % 16);
                offset++;
            } while (LogColors.Values.Contains(color));

            return color;
        }

        public static void SetLogLevel(LogLevel level)
        {
            if (level != _currentLogLevel) 
                _currentLogLevel = level;
        }
    }
}
