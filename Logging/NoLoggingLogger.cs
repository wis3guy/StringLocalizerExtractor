#region # using statements #

using System;

#endregion

namespace StringLocalizerExtractor.Logging
{

    /// <summary>
    /// Represents a <see cref="ILogger"/> that doesn't log any message.
    /// </summary>
    internal class NoLoggingLogger : ILogger
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="NoLoggingLogger"/>
        /// class.
        /// </summary>
        public NoLoggingLogger()
        {
        }

        #region # ILogger #

        public void Verbose(string message)
        {
        }

        public void Verbose(string message, Exception exception)
        {
        }

        public void Debug(string message)
        {
        }

        public void Debug(string message, Exception exception)
        {
        }

        public void Information(string message)
        {
        }

        public void Information(string message, Exception exception)
        {
        }

        public void Warning(string message)
        {
        }

        public void Warning(string message, Exception exception)
        {
        }

        public void Error(string message)
        {
        }

        public void Error(string message, Exception exception)
        {
        }

        public void Fatal(string message)
        {
        }

        public void Fatal(string message, Exception exception)
        {
        }

        #endregion
    }
}