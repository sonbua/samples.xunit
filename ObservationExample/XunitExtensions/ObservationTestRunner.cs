using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace XunitExtensions
{
    public class ObservationTestRunner : XunitTestRunner
    {
        private readonly IReadOnlyList<Attribute> _beforeAfterAttributes;

        public ObservationTestRunner(
            ITest test,
            IMessageBus messageBus,
            Type testClass,
            object[] constructorArguments,
            MethodInfo testMethod,
            object[] testMethodArguments,
            string skipReason,
            IReadOnlyList<Attribute> beforeAfterAttributes,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
            : base(test, messageBus, testClass, constructorArguments, testMethod, testMethodArguments, skipReason, beforeAfterAttributes.OfType<BeforeAfterTestAttribute>().ToList(), aggregator, cancellationTokenSource)
        {
            _beforeAfterAttributes = beforeAfterAttributes;
        }

        protected override async Task<decimal> InvokeTestMethodAsync(ExceptionAggregator aggregator)
        {
            var testInvoker = new ObservationTestInvoker(Test, MessageBus, TestClass, ConstructorArguments, TestMethod, TestMethodArguments, _beforeAfterAttributes, aggregator, CancellationTokenSource);

            return await testInvoker.RunCustomAsync();
        }
    }
}
