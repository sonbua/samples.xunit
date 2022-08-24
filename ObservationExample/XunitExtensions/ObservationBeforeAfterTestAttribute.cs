using System;
using System.Reflection;

namespace XunitExtensions
{
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
}