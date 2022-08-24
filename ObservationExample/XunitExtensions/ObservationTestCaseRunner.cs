using System.Threading;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace XunitExtensions
{
    public class ObservationTestCaseRunner : TestCaseRunner<ObservationTestCase>
    {
        readonly string displayName;

        public ObservationTestCaseRunner(
            ObservationTestCase testCase,
            string displayName,
            IMessageBus messageBus,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
            : base(testCase, messageBus, aggregator, cancellationTokenSource)
        {
            this.displayName = displayName;
        }

        protected override Task<RunSummary> RunTestAsync()
        {
            var testClass = TestCase.TestMethod.TestClass.Class.ToRuntimeType();
            var testMethod = TestCase.TestMethod.Method.ToRuntimeMethod();
            var test = new XunitTest(TestCase, displayName);

            return new ObservationTestRunner(test, MessageBus, testClass, testMethod, Aggregator, CancellationTokenSource).RunAsync();
        }
    }
}

