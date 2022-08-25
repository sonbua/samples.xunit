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
    public class ObservationTestCaseRunner : XunitTestCaseRunner
    {
        private List<Attribute> _beforeAfterTestAttributes;

        public ObservationTestCaseRunner(
            ObservationTestCase testCase,
            string displayName,
            string skipReason,
            object[] constructorArguments,
            object[] testMethodArguments,
            IMessageBus messageBus,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
            : base(testCase, displayName, skipReason, constructorArguments, testMethodArguments, messageBus, aggregator, cancellationTokenSource)
        {
            _beforeAfterTestAttributes = BeforeAfterTestAttributes();
        }
        
        private List<Attribute> BeforeAfterTestAttributes()
        {
            var beforeAfterTestCollectionAttributes = BeforeAfterTestCollectionAttributes();

            return beforeAfterTestCollectionAttributes
                .Concat(TestClass.GetTypeInfo().GetCustomAttributes().Where(IsBeforeAfterTestAttribute))
                .Concat(TestMethod.GetCustomAttributes().Where(IsBeforeAfterTestAttribute))
                .ToList();

            IEnumerable<Attribute> BeforeAfterTestCollectionAttributes()
            {
                if (TestCase.TestMethod.TestClass.TestCollection.CollectionDefinition is IReflectionTypeInfo collectionDefinition)
                {
                    return collectionDefinition.Type.GetTypeInfo()
                        .GetCustomAttributes()
                        .Where(IsBeforeAfterTestAttribute);
                }

                return Enumerable.Empty<Attribute>();
            }

            static bool IsBeforeAfterTestAttribute(Attribute x)
            {
                return x is BeforeAfterTestAttribute || x is ObservationBeforeAfterTestAttribute;
            }
        }

        protected override Task<RunSummary> RunTestAsync()
        {
            var test = new XunitTest(TestCase, DisplayName);

            return new ObservationTestRunner(test, MessageBus, TestClass, ConstructorArguments, TestMethod, TestMethodArguments, SkipReason, _beforeAfterTestAttributes, Aggregator, CancellationTokenSource).RunAsync();
        }
    }
}

