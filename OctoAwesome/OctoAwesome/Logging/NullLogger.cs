using System;

namespace OctoAwesome.Logging
{
    /// <summary>
    /// Logger that absorbs all log messages and voids them.
    /// </summary>
    public class NullLogger : ILogger
    {
        /// <summary>
        /// Gets a default null logger.
        /// </summary>
        public static ILogger Default { get; }

        static NullLogger()
        {
            Default = new NullLogger().As(nameof(Default));
        }

        /// <summary>
        /// Gets the name of the logger.
        /// </summary>
        public string Name { get; private init; }

        /// <inheritdoc />
        public ILogger As(string loggerName) => new NullLogger
        {
            Name = loggerName
        };
        public ILogger As(Type type)
            => As(type.FullName);

        /// <inheritdoc />
        public void Debug(string message) { }

        /// <inheritdoc />
        public void Debug(string message, Exception exception) { }

        /// <inheritdoc />
        public void Debug<T>(T message) { }

        /// <inheritdoc />
        public void Error(string message) { }

        /// <inheritdoc />
        public void Error(string message, Exception exception) { }

        /// <inheritdoc />
        public void Error<T>(T message) { }

        /// <inheritdoc />
        public void Fatal(string message) { }

        /// <inheritdoc />
        public void Fatal(string message, Exception exception) { }

        /// <inheritdoc />
        public void Fatal<T>(T message) { }

        /// <inheritdoc />
        public void Info(string message) { }

        /// <inheritdoc />
        public void Info(string message, Exception exception) { }

        /// <inheritdoc />
        public void Info<T>(T message) { }

        /// <inheritdoc />
        public void Trace(string message) { }

        /// <inheritdoc />
        public void Trace(string message, Exception exception) { }

        /// <inheritdoc />
        public void Trace<T>(T message) { }

        /// <inheritdoc />
        public void Warn(string message) { }

        /// <inheritdoc />
        public void Warn(string message, Exception exception) { }

        /// <inheritdoc />
        public void Warn<T>(T message) { }

        /// <inheritdoc />
        public void Flush() { }
    }
}
