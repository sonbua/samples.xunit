using System;
using System.Diagnostics;
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
        private readonly Specification specification;

        public ObservationTestInvoker(Specification specification,
                                      ITest test,
                                      IMessageBus messageBus,
                                      Type testClass,
                                      MethodInfo testMethod,
                                      ExceptionAggregator aggregator,
                                      CancellationTokenSource cancellationTokenSource)
            : base(test, messageBus, testClass, null, testMethod, null, aggregator, cancellationTokenSource)
        {
            this.specification = specification;
        }

        public new Task<decimal> RunAsync()
        {
            return Aggregator.RunAsync(async () =>
            {
                if (!CancellationTokenSource.IsCancellationRequested)
                {
                    try
                    {
                        if (specification is IAsyncLifetime asyncLifetime2)
                            await asyncLifetime2.InitializeAsync();
                        if (!CancellationTokenSource.IsCancellationRequested)
                        {
                            await BeforeTestMethodInvokedAsync(specification);
                            if (!CancellationTokenSource.IsCancellationRequested && !Aggregator.HasExceptions)
                            {
                                await InvokeTestMethodAsync(specification);
                            }

                            await AfterTestMethodInvokedAsync(specification);
                        }

                        if (specification is IAsyncLifetime asyncLifetime3)
                            await Aggregator.RunAsync(asyncLifetime3.DisposeAsync);
                    }
                    finally
                    {
                        Aggregator.Run(() =>
                            Test.DisposeTestClass(specification, MessageBus, Timer, CancellationTokenSource));
                    }
                }

                return Timer.Total;
            });
            
            // return Aggregator.RunAsync(async () =>
            // {
            //     if (!CancellationTokenSource.IsCancellationRequested)
            //     {
            //         if (!CancellationTokenSource.IsCancellationRequested)
            //         {
            //             if (!Aggregator.HasExceptions)
            //                 await Timer.AggregateAsync(() => InvokeTestMethodAsync(specification));
            //         }
            //     }
            //
            //     return Timer.Total;
            // });
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
    public abstract class ObservationBeforeAfterTestAttribute : Attribute
    {
        /// <summary>
        /// This method is called before the test method is executed.
        /// </summary>
        /// <param name="methodUnderTest">The method under test</param>
        public virtual void Before(MethodInfo methodUnderTest, object testClassInstance)
        {
        }
    }
    
    public class PrivateFieldAttribute : ObservationBeforeAfterTestAttribute
    {
        public override void Before(MethodInfo methodUnderTest, object testClassInstance)
        {
            Debug.WriteLine("private field..................");
        }
    }    
}
