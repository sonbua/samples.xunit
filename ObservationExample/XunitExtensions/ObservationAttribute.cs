using System;
using Xunit;
using Xunit.Sdk;

namespace XunitExtensions
{
    [AttributeUsage(AttributeTargets.Method)]
    [XunitTestCaseDiscoverer("XunitExtensions.ObservationTestCaseDiscoverer", "ObservationExample")]
    public class ObservationAttribute : FactAttribute
    {
    }
}