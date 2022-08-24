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
        /// <param name="testClassInstance">The class instance which contains the <paramref name="methodUnderTest"/></param>
        public virtual void Before(MethodInfo methodUnderTest, object testClassInstance)
        {
        }

        /// <summary>
        /// This method is called after the test method is executed.
        /// </summary>
        /// <param name="methodUnderTest">The method under test</param>
        /// <param name="testClassInstance">The class instance which contains the <paramref name="methodUnderTest"/></param>
        public virtual void After(MethodInfo methodUnderTest, object testClassInstance)
        {
        }
    }
}