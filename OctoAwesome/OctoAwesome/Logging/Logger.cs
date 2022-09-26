using NLog;
using System;
using OctoAwesome.Extension;

namespace OctoAwesome.Logging
{
    /// <summary>
    /// Logger which wraps <see cref="NLog"/> loggers.
    /// </summary>
    public sealed class Logger : ILogger
    {
        private static readonly NLog.ILogger nullLogger;

        static Logger()
        {
            nullLogger = LogManager.LogFactory.CreateNullLogger();
        }

        private NLog.ILogger internalLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class.
        /// </summary>
        public Logger()
        {
            internalLogger = nullLogger;
        }

        /// <inheritdoc />
        public void Info(string message)
            => internalLogger.Info(message);

        /// <inheritdoc />
        public void Info(string message, Exception exception)
            => internalLogger.Info(exception, message);

        /// <inheritdoc />
        public void Info<T>(T message)
            => internalLogger.Info(message);

        /// <inheritdoc />
        public void Error(string message)
            => internalLogger.Error(message);

        /// <inheritdoc />
        public void Error(string message, Exception exception)
            => internalLogger.Error(exception, message);

        /// <inheritdoc />
        public void Error<T>(T message)
            => internalLogger.Error(message);

        /// <inheritdoc />
        public void Warn(string message)
            => internalLogger.Warn(message);

        /// <inheritdoc />
        public void Warn(string message, Exception exception)
            => internalLogger.Warn(exception, message);

        /// <inheritdoc />
        public void Warn<T>(T message)
            => internalLogger.Warn(message);

        /// <inheritdoc />
        public void Debug(string message)
            => internalLogger.Debug(message);

        /// <inheritdoc />
        public void Debug(string message, Exception exception)
            => internalLogger.Debug(exception, message);

        /// <inheritdoc />
        public void Debug<T>(T message)
            => internalLogger.Debug(message);

        /// <inheritdoc />
        public void Trace(string message)
            => internalLogger.Trace(message);

        /// <inheritdoc />
        public void Trace(string message, Exception exception)
            => internalLogger.Trace(exception, message);

        /// <inheritdoc />
        public void Trace<T>(T message)
            => internalLogger.Trace(message);

        /// <inheritdoc />
        public void Fatal(string message)
            => internalLogger.Fatal(message);

        /// <inheritdoc />
        public void Fatal(string message, Exception exception)
            => internalLogger.Fatal(exception, message);

        /// <inheritdoc />
        public void Fatal<T>(T message)
            => internalLogger.Fatal(message);

        /// <inheritdoc />
        public ILogger As(string loggerName)
        {
            internalLogger = LogManager.GetLogger(loggerName);
            return this;
        }

        /// <inheritdoc />
        public ILogger As(Type type)
            => As(NullabilityHelper.NotNullAssert(type.FullName, "type.FullName != null"));

        /// <inheritdoc />
        public void Flush()
        {
            LogManager.Flush();
        }
    }
}
