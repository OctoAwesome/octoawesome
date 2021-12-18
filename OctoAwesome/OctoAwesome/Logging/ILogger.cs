using System;

namespace OctoAwesome.Logging
{
    /// <summary>
    /// Interface for loggers.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs a debug message to the logger.
        /// </summary>
        /// <param name="message">The debug message.</param>
        void Debug(string message);

        /// <summary>
        /// Logs a debug message to the logger.
        /// </summary>
        /// <param name="message">The debug message.</param>
        /// <param name="exception">The associated exception to log.</param>
        void Debug(string message, Exception exception);

        /// <summary>
        /// Logs a debug message to the logger.
        /// </summary>
        /// <param name="message">The debug message.</param>
        void Debug<T>(T message);


        /// <summary>
        /// Logs an error message to the logger.
        /// </summary>
        /// <param name="message">The error message.</param>
        void Error(string message);

        /// <summary>
        /// Logs an error message to the logger.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="exception">The associated exception to log.</param>
        void Error(string message, Exception exception);

        /// <summary>
        /// Logs an error message to the logger.
        /// </summary>
        /// <param name="message">The error message.</param>
        void Error<T>(T message);


        /// <summary>
        /// Logs a fatal error message to the logger.
        /// </summary>
        /// <param name="message">The fatal error message.</param>
        void Fatal(string message);

        /// <summary>
        /// Logs a fatal error message to the logger.
        /// </summary>
        /// <param name="message">The fatal error message.</param>
        /// <param name="exception">The associated exception to log.</param>
        void Fatal(string message, Exception exception);

        /// <summary>
        /// Logs a fatal error message to the logger.
        /// </summary>
        /// <param name="message">The fatal error message.</param>
        void Fatal<T>(T message);

        /// <summary>
        /// Logs an info message to the logger.
        /// </summary>
        /// <param name="message">The info message.</param>
        void Info(string message);

        /// <summary>
        /// Logs an info message to the logger.
        /// </summary>
        /// <param name="message">The info message.</param>
        /// <param name="exception">The associated exception to log.</param>
        void Info(string message, Exception exception);

        /// <summary>
        /// Logs an info message to the logger.
        /// </summary>
        /// <param name="message">The info message.</param>
        void Info<T>(T message);

        /// <summary>
        /// Logs a trace message to the logger.
        /// </summary>
        /// <param name="message">The trace message.</param>
        void Trace(string message);

        /// <summary>
        /// Logs a trace message to the logger.
        /// </summary>
        /// <param name="message">The trace message.</param>
        /// <param name="exception">The associated exception to log.</param>
        void Trace(string message, Exception exception);

        /// <summary>
        /// Logs a trace message to the logger.
        /// </summary>
        /// <param name="message">The trace message.</param>
        void Trace<T>(T message);


        /// <summary>
        /// Logs a warning to the logger.
        /// </summary>
        /// <param name="message">The warning message.</param>
        void Warn(string message);

        /// <summary>
        /// Logs a warning to the logger.
        /// </summary>
        /// <param name="message">The warning message.</param>
        /// <param name="exception">The associated exception to log.</param>
        void Warn(string message, Exception exception);

        /// <summary>
        /// Logs a warning to the logger.
        /// </summary>
        /// <param name="message">The warning message.</param>
        void Warn<T>(T message);

        /// <summary>
        /// Gets a logger by its name.
        /// </summary>
        /// <param name="loggerName">The name of the logger to get.</param>
        /// <returns>The logger.</returns>
        ILogger As(string loggerName);

        /// <summary>
        /// Gets a logger associated to a given type.
        /// </summary>
        /// <param name="type">The associated type to the logger to get.</param>
        /// <returns>The logger.</returns>
        ILogger As(Type type);

        /// <summary>
        /// Flush the logged messages.
        /// </summary>
        void Flush();
    }
}