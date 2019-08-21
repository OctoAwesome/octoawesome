using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Logging
{
    public sealed class Logger : ILogger
    {
        private readonly NLog.ILogger internalLogger;

        internal Logger(NLog.ILogger logger)
        {
            internalLogger = logger;
        }

        public void Info(string message)
            => internalLogger.Info(message);
        public void Info(string message, Exception exception)
            => internalLogger.Info(exception, message);
        public void Info<T>(T message)
            => internalLogger.Info(message);

        public void Error(string message)
            => internalLogger.Error(message);
        public void Error(string message, Exception exception)
            => internalLogger.Error(exception, message);
        public void Error<T>(T message)
            => internalLogger.Error(message);

        public void Warn(string message)
            => internalLogger.Warn(message);
        public void Warn(string message, Exception exception)
            => internalLogger.Warn(exception, message);
        public void Warn<T>(T message)
            => internalLogger.Warn(message);

        public void Debug(string message)
            => internalLogger.Debug(message);
        public void Debug(string message, Exception exception)
            => internalLogger.Debug(exception, message);
        public void Debug<T>(T message)
            => internalLogger.Debug(message);

        public void Trace(string message)
            => internalLogger.Trace(message);
        public void Trace(string message, Exception exception)
            => internalLogger.Trace(exception, message);
        public void Trace<T>(T message)
            => internalLogger.Trace(message);

        public void Fatal(string message)
            => internalLogger.Trace(message);
        public void Fatal(string message, Exception exception)
            => internalLogger.Trace(exception, message);
        public void Fatal<T>(T message)
            => internalLogger.Trace(message);
    }
}
