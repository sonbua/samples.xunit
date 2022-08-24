using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace XunitExtensions
{
    public class ObservationTestMethodRunner : TestMethodRunner<ObservationTestCase>
    {
        public ObservationTestMethodRunner(
            ITestMethod testMethod,
            IReflectionTypeInfo @class,
            IReflectionMethodInfo method,
            IEnumerable<ObservationTestCase> testCases,
            IMessageBus messageBus,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
            : base(testMethod, @class, method, testCases, messageBus, aggregator, cancellationTokenSource)
        {
        }

        protected override Task<RunSummary> RunTestCaseAsync(ObservationTestCase testCase)
        {
            return testCase.RunAsync(MessageBus, new ExceptionAggregator(Aggregator), CancellationTokenSource);
        }
    }
}
