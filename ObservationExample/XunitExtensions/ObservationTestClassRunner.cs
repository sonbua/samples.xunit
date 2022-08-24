using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace XunitExtensions
{
    public class ObservationTestClassRunner : TestClassRunner<ObservationTestCase>
    {
        public ObservationTestClassRunner(
            ITestClass testClass,
            IReflectionTypeInfo @class,
            IEnumerable<ObservationTestCase> testCases,
            IMessageSink diagnosticMessageSink,
            IMessageBus messageBus,
            ITestCaseOrderer testCaseOrderer,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
            : base(testClass, @class, testCases, diagnosticMessageSink, messageBus, testCaseOrderer, aggregator, cancellationTokenSource)
        {
        }

        protected override Task<RunSummary> RunTestMethodAsync(ITestMethod testMethod,
                                                               IReflectionMethodInfo method,
                                                               IEnumerable<ObservationTestCase> testCases,
                                                               object[] constructorArguments)
        {
            return new ObservationTestMethodRunner(testMethod, Class, method, testCases, MessageBus, new ExceptionAggregator(Aggregator), CancellationTokenSource).RunAsync();
        }
    }
}
