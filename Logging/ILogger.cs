using System;

namespace StringLocalizerExtractor.Logging
{

    /// <summary>
    /// Defines the basic functionality of a logger.
    /// </summary>
    public interface ILogger
    {

        void Verbose(string message);

        void Verbose(string message, Exception exception);

        void Debug(string message);

        void Debug(string message, Exception exception);

        void Information(string message);

        void Information(string message, Exception exception);

        void Warning(string message);

        void Warning(string message, Exception exception);

        void Error(string message);

        void Error(string message, Exception exception);

        void Fatal(string message);

        void Fatal(string message, Exception exception);

    }
}