using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private readonly IReadOnlyList<Attribute> _beforeAfterAttributes;
        private readonly System.Collections.Generic.Stack<Attribute> _beforeAfterAttributesRun;

        public ObservationTestInvoker(
            ITest test,
            IMessageBus messageBus,
            Type testClass,
            object[] constructorArguments,
            MethodInfo testMethod,
            object[] testMethodArguments,
            IReadOnlyList<Attribute> beforeAfterAttributes,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
            : base(test, messageBus, testClass, constructorArguments, testMethod, testMethodArguments, aggregator, cancellationTokenSource)
        {
            _beforeAfterAttributes = beforeAfterAttributes;
            _beforeAfterAttributesRun = new System.Collections.Generic.Stack<Attribute>();
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
            foreach (var beforeTestAttribute in _beforeAfterAttributes)
            {
                var name = beforeTestAttribute.GetType().Name;
                
                if (!MessageBus.QueueMessage(new BeforeTestStarting(Test, name)))
                {
                    CancellationTokenSource.Cancel();
                }
                else
                {
                    try
                    {
                        var action = beforeTestAttribute is ObservationBeforeAfterTestAttribute
                            ? () => ((ObservationBeforeAfterTestAttribute)beforeTestAttribute).Before(TestMethod, testClassInstance)
                            : (Action)(() => ((BeforeAfterTestAttribute)beforeTestAttribute).Before(TestMethod));

                        Timer.Aggregate(action);
                        _beforeAfterAttributesRun.Push(beforeTestAttribute);
                    }
                    catch (Exception ex)
                    {
                        Aggregator.Add(ex);
                        break;
                    }
                    finally
                    {
                        if (!MessageBus.QueueMessage(new BeforeTestFinished(Test, name)))
                            CancellationTokenSource.Cancel();
                    }
                }
                
                if (CancellationTokenSource.IsCancellationRequested)
                {
                    break;
                }
            }

            return base.BeforeTestMethodInvokedAsync();
        }

        protected virtual Task AfterTestMethodInvokedAsync(object testClassInstance)
        {
            foreach (var afterTestAttribute in _beforeAfterAttributesRun)
            {
                string name = afterTestAttribute.GetType().Name;
                
                if (!MessageBus.QueueMessage(new AfterTestStarting(Test, name)))
                {
                    CancellationTokenSource.Cancel();
                }

                var action = afterTestAttribute is ObservationBeforeAfterTestAttribute
                    ? () => ((ObservationBeforeAfterTestAttribute)afterTestAttribute).After(TestMethod, testClassInstance)
                    : (Action)(() => ((BeforeAfterTestAttribute)afterTestAttribute).After(TestMethod));
                Aggregator.Run(() => Timer.Aggregate(action));
                
                if (!MessageBus.QueueMessage(new AfterTestFinished(Test, name)))
                {
                    CancellationTokenSource.Cancel();
                }
            }
            
            return base.AfterTestMethodInvokedAsync();
        }
    }
}
