#region # using statements #

using System;

#endregion

namespace StringLocalizerExtractor.Logging
{

    /// <summary>
    /// Represents a <see cref="ILogger"/> that writes log to the
    /// <see cref="Console"/>.
    /// </summary>
    public class ConsoleLogger : ILogger
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleLogger"/>
        /// class.
        /// </summary>
        public ConsoleLogger()
        {
        }

        #region # Methods #

        #region == Private ==

        private static void Log(LogLevel level, string message, Exception exception)
        {
            Console.Write(DateTimeOffset.Now);
            Console.Write(" [");
            Console.ForegroundColor = GetLevelColor(level);
            Console.Write(level.ToString().ToUpper());
            Console.ResetColor();
            Console.Write("] ");

            // Write message and exception (if any)
            Console.WriteLine(message);
            if (exception != null)
                Console.WriteLine(exception);
        }

        private static ConsoleColor GetLevelColor(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Verbose:
                    return ConsoleColor.Gray;
                case LogLevel.Debug:
                    return ConsoleColor.Gray;
                case LogLevel.Information:
                    return ConsoleColor.White;
                case LogLevel.Warning:
                    return ConsoleColor.Yellow;
                case LogLevel.Error:
                    return ConsoleColor.Red;
                case LogLevel.Fatal:
                    return ConsoleColor.DarkRed;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }

        #endregion

        #endregion

        #region # ILogger #

        public void Verbose(string message)
        {
            Log(LogLevel.Verbose, message, null);
        }

        public void Verbose(string message, Exception exception)
        {
            Log(LogLevel.Verbose, message, exception);
        }

        public void Debug(string message)
        {
            Log(LogLevel.Debug, message, null);
        }

        public void Debug(string message, Exception exception)
        {
            Log(LogLevel.Debug, message, exception);
        }

        public void Information(string message)
        {
            Log(LogLevel.Information, message, null);
        }

        public void Information(string message, Exception exception)
        {
            Log(LogLevel.Information, message, exception);
        }

        public void Warning(string message)
        {
            Log(LogLevel.Warning, message, null);
        }

        public void Warning(string message, Exception exception)
        {
            Log(LogLevel.Warning, message, exception);
        }

        public void Error(string message)
        {
            Log(LogLevel.Error, message, null);
        }

        public void Error(string message, Exception exception)
        {
            Log(LogLevel.Error, message, exception);
        }

        public void Fatal(string message)
        {
            Log(LogLevel.Fatal, message, null);
        }

        public void Fatal(string message, Exception exception)
        {
            Log(LogLevel.Fatal, message, exception);
        }

        #endregion

        private enum LogLevel
        {

            Verbose,
            Debug,
            Information,
            Warning,
            Error,
            Fatal

        }

    }
}