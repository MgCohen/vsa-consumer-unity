using System;
using Microsoft.Extensions.Logging;
using UnityEngine;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace App {
    public sealed class UnityLogProvider : ILoggerProvider {
        public ILogger CreateLogger(string categoryName) => new UnityLogger(categoryName);

        public void Dispose() { }

        sealed class UnityLogger : ILogger {
            readonly string category;

            public UnityLogger(string category) => this.category = category;

            public IDisposable BeginScope<TState>(TState state) => null;

            public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Information;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) {
                if (!IsEnabled(logLevel)) {
                    return;
                }
                var message = $"[{category}] {formatter(state, exception)}";
                if (logLevel >= LogLevel.Error) {
                    Debug.LogError(exception is null ? message : $"{message}\n{exception}");
                } else if (logLevel == LogLevel.Warning) {
                    Debug.LogWarning(message);
                } else {
                    Debug.Log(message);
                }
            }
        }
    }
}
