using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace XunitExtensions
{
    public class ObservationTestRunner : TestRunner<ObservationTestCase>
    {
        public ObservationTestRunner(
            ITest test,
            IMessageBus messageBus,
            Type testClass,
            MethodInfo testMethod,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
            : base(test, messageBus, testClass, null, testMethod, null, null, aggregator, cancellationTokenSource)
        {
        }

        protected override async Task<Tuple<decimal, string>> InvokeTestAsync(ExceptionAggregator aggregator)
        {
            var duration = await new ObservationTestInvoker(Test, MessageBus, TestClass, TestMethod, aggregator, CancellationTokenSource).RunCustomAsync();
            return Tuple.Create(duration, String.Empty);
        }
    }
}

