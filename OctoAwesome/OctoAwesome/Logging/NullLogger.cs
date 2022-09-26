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
            Default = new NullLogger(nameof(Default));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NullLogger"/> class.
        /// </summary>
        /// <param name="name">The name of the logger.</param>
        public NullLogger(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets the name of the logger.
        /// </summary>
        public string Name { get; }

        /// <inheritdoc />
        public ILogger As(string loggerName) => new NullLogger(loggerName);

        /// <inheritdoc/>
        public ILogger As(Type type)
            => As(type.FullName ?? type.Name);

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
