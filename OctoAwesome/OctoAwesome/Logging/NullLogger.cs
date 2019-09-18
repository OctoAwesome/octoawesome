using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Logging
{
    public class NullLogger : ILogger
    {
        public static ILogger Default { get; }

        static NullLogger()
        {
            Default = new NullLogger().As(nameof(Default));
        }

        public string Name { get; private set; }

        public ILogger As(string loggerName) => new NullLogger
        {
            Name = loggerName
        };
        public ILogger As(Type type)
            => As(type.FullName);

        public void Debug(string message) { }
        public void Debug(string message, Exception exception) { }
        public void Debug<T>(T message) { }

        public void Error(string message) { }
        public void Error(string message, Exception exception) { }
        public void Error<T>(T message) { }

        public void Fatal(string message) { }
        public void Fatal(string message, Exception exception) { }
        public void Fatal<T>(T message) { }

        public void Info(string message) { }
        public void Info(string message, Exception exception) { }
        public void Info<T>(T message) { }

        public void Trace(string message) { }
        public void Trace(string message, Exception exception) { }
        public void Trace<T>(T message) { }

        public void Warn(string message) { }
        public void Warn(string message, Exception exception) { }
        public void Warn<T>(T message) { }

        public void Flush() { }
    }
}
