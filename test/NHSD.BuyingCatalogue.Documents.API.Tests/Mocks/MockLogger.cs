using System;
using Microsoft.Extensions.Logging;

namespace NHSD.BuyingCatalogue.Documents.API.UnitTests.Mocks
{
    // This hand-rolled mock is necessary because we currently have a dependency
    // on ILogger<T> for logging. It is not possible to mock dynamically because
    // the FormattedLogValues framework struct is internal.
    // Recommend that we refactor by defining our owning logging abstraction.    // This hand-rolled mock is necessary because we currently have a dependency
    // on ILogger<T> for logging. It is not possible to mock dynamically because
    // the FormattedLogValues framework struct is internal.
    // Recommend that we refactor by defining our owning logging abstraction.
    internal sealed class MockLogger<T> : ILogger<T>
    {
        private readonly Action<LogLevel, Exception> logCallback;

        internal MockLogger(Action<LogLevel, Exception> logCallback) => this.logCallback = logCallback;

        public IDisposable BeginScope<TState>(TState state) => throw new NotSupportedException();

        public bool IsEnabled(LogLevel logLevel) => throw new NotSupportedException();

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            logCallback(logLevel, exception);
        }
    }
}
