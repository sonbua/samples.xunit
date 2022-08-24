using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace XunitExtensions
{
    public class ObservationTestInvoker : TestInvoker<ObservationTestCase>
    {
        public ObservationTestInvoker(
            ITest test,
            IMessageBus messageBus,
            Type testClass,
            MethodInfo testMethod,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
            : base(test, messageBus, testClass, null, testMethod, null, aggregator, cancellationTokenSource)
        {
        }

        public Task<decimal> RunCustomAsync()
        {
            return Aggregator.RunAsync(async () =>
            {
                if (!CancellationTokenSource.IsCancellationRequested)
                {
                    var testClassInstance = CreateTestClass();
                    try
                    {
                        if (testClassInstance is IAsyncLifetime asyncLifetime2)
                            await asyncLifetime2.InitializeAsync();
                        if (!CancellationTokenSource.IsCancellationRequested)
                        {
                            // customized from xunit
                            // https://github.com/xunit/xunit/blob/main/src/xunit.v3.core/Sdk/v3/Runners/XunitTestInvoker.cs
                            await BeforeTestMethodInvokedAsync(testClassInstance);
                            if (!CancellationTokenSource.IsCancellationRequested && !Aggregator.HasExceptions)
                            {
                                await InvokeTestMethodAsync(testClassInstance);
                            }

                            await AfterTestMethodInvokedAsync(testClassInstance);
                        }

                        if (testClassInstance is IAsyncLifetime asyncLifetime3)
                            await Aggregator.RunAsync(asyncLifetime3.DisposeAsync);
                    }
                    finally
                    {
                        Aggregator.Run(() =>
                            Test.DisposeTestClass(testClassInstance, MessageBus, Timer, CancellationTokenSource));
                    }
                }

                return Timer.Total;
            });
        }

        protected virtual Task BeforeTestMethodInvokedAsync(object testClassInstance)
        {
            var attributes =
                TestCase.TestMethod.Method.GetCustomAttributes(typeof(ObservationBeforeAfterTestAttribute))
                    .Cast<ReflectionAttributeInfo>()
                    .Select(x => x.Attribute)
                    .Cast<ObservationBeforeAfterTestAttribute>();

            foreach (var attribute in attributes)
            {
                attribute.Before(TestMethod, testClassInstance);
            }

            return base.BeforeTestMethodInvokedAsync();
        }

        protected virtual Task AfterTestMethodInvokedAsync(object testClassInstance)
        {
            return base.AfterTestMethodInvokedAsync();
        }
    }
}
