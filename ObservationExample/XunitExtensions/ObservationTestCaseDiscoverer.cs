using System.Collections.Generic;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace XunitExtensions
{
    public class ObservationTestCaseDiscoverer : IXunitTestCaseDiscoverer
    {
        private readonly IMessageSink _diagnosticMessageSink;

        public ObservationTestCaseDiscoverer(IMessageSink diagnosticMessageSink)
        {
            _diagnosticMessageSink = diagnosticMessageSink;
        }

        public IEnumerable<IXunitTestCase> Discover(
            ITestFrameworkDiscoveryOptions discoveryOptions,
            ITestMethod testMethod,
            IAttributeInfo factAttribute)
        {
            var skipReason = factAttribute.GetNamedArgument<string>("Skip");

            if (skipReason != null)
            {
                yield return new XunitTestCase(
                    _diagnosticMessageSink,
                    discoveryOptions.MethodDisplayOrDefault(),
                    TestMethodDisplayOptions.None,
                    testMethod);

                yield break;
            }

            testMethod.Method.GetCustomAttributes(typeof(ObservationAttribute));
            var defaultMethodDisplay = discoveryOptions.MethodDisplayOrDefault();
            var defaultMethodDisplayOptions = discoveryOptions.MethodDisplayOptionsOrDefault();

            yield return new ObservationTestCase(
                _diagnosticMessageSink,
                defaultMethodDisplay,
                defaultMethodDisplayOptions,
                testMethod);
        }
    }
}