using System;

namespace OctoAwesome.Logging
{
    public interface ILogger
    {
        void Debug(string message);
        void Debug(string message, Exception exception);
        void Debug<T>(T message);

        void Error(string message);
        void Error(string message, Exception exception);
        void Error<T>(T message);

        void Fatal(string message);
        void Fatal(string message, Exception exception);
        void Fatal<T>(T message);

        void Info(string message);
        void Info(string message, Exception exception);
        void Info<T>(T message);

        void Trace(string message);
        void Trace(string message, Exception exception);
        void Trace<T>(T message);

        void Warn(string message);
        void Warn(string message, Exception exception);
        void Warn<T>(T message);

        ILogger As(string loggerName);
        ILogger As(Type type);
    }
}