﻿using System;
using System.Diagnostics;
using System.Reflection;
using Xunit;
using XunitExtensions;

[assembly: TestFramework("XunitExtensions.ObservationTestFramework", "ObservationExample")]

public class When_you_have_a_new_stack
{
    Stack<string> stack;

    public When_you_have_a_new_stack()
    {
        stack = new Stack<string>();
    }

    [Observation]
    [PrivateField]
    public void should_be_empty()
    {
        Assert.True(stack.IsEmpty);
    }

    [Fact]
    public void should_not_allow_you_to_call_Pop()
    {
        Assert.Throws<InvalidOperationException>(() => stack.Pop());
    }

    [Fact]
    public void should_not_allow_you_to_call_Peek()
    {
        Assert.Throws<InvalidOperationException>(() => { string unused = stack.Peek; });
    }
}

public class PrivateFieldAttribute : ObservationBeforeAfterTestAttribute
{
    public override void Before(MethodInfo methodUnderTest, object testClassInstance)
    {
        Debug.WriteLine("Entering test method");
    }

    public override void After(MethodInfo methodUnderTest, object testClassInstance)
    {
        Debug.WriteLine("Exiting test method");
    }
}    

// public class When_you_push_an_item_onto_the_stack
// {
//     Stack<string> stack;
//
//     protected override void EstablishContext()
//     {
//         stack = new Stack<string>();
//     }
//
//     protected override void Because()
//     {
//         stack.Push("first element");
//     }
//
//     [Fact]
//     public void should_not_be_empty()
//     {
//         Assert.False(stack.IsEmpty);
//     }
// }
//
// public class When_you_push_null_onto_the_stack : Specification
// {
//     Stack<string> stack;
//
//     protected override void EstablishContext()
//     {
//         stack = new Stack<string>();
//     }
//
//     protected override void Because()
//     {
//         stack.Push(null);
//     }
//
//     [Fact]
//     public void should_not_be_empty()
//     {
//         Assert.False(stack.IsEmpty);
//     }
//
//     [Fact]
//     public void should_return_null_when_calling_Peek()
//     {
//         Assert.Null(stack.Peek);
//     }
//
//     [Fact]       // Order dependent: calling Pop before Top would cause a test failure!
//     public void should_return_null_when_calling_Pop()
//     {
//         Assert.Null(stack.Pop());
//     }
// }
//
// public class When_you_push_then_pop_a_value_from_the_stack : Specification
// {
//     Stack<string> stack;
//     const string expected = "first element";
//     string actual;
//
//     protected override void EstablishContext()
//     {
//         stack = new Stack<string>();
//     }
//
//     protected override void Because()
//     {
//         stack.Push(expected);
//         actual = stack.Pop();
//     }
//
//     [Fact]
//     public void should_get_the_value_that_was_pushed()
//     {
//         Assert.Equal(expected, actual);
//     }
//
//     [Fact]
//     public void should_be_empty_again()
//     {
//         Assert.True(stack.IsEmpty);
//     }
// }
//
// public class When_you_push_an_item_on_the_stack_and_call_Peek : Specification
// {
//     Stack<string> stack;
//     const string expected = "first element";
//     string actual;
//
//     protected override void EstablishContext()
//     {
//         stack = new Stack<string>();
//     }
//
//     protected override void Because()
//     {
//         stack.Push(expected);
//         actual = stack.Peek;
//     }
//
//     [Fact]
//     public void should_not_modify_the_stack()
//     {
//         Assert.False(stack.IsEmpty);
//     }
//
//     [Fact]
//     public void should_return_the_last_item_pushed_onto_the_stack()
//     {
//         Assert.Equal(expected, actual);
//     }
//
//     [Fact]
//     public void should_return_the_same_item_for_subsequent_Peek_calls()
//     {
//         Assert.Equal(actual, stack.Peek);
//         Assert.Equal(actual, stack.Peek);
//         Assert.Equal(actual, stack.Peek);
//     }
// }
//
// public class When_you_push_several_items_onto_the_stack : Specification
// {
//     Stack<string> stack;
//     const string firstElement = "firstElement";
//     const string secondElement = "secondElement";
//     const string thirdElement = "thirdElement";
//
//     protected override void EstablishContext()
//     {
//         stack = new Stack<string>();
//     }
//
//     protected override void Because()
//     {
//         stack.Push(firstElement);
//         stack.Push(secondElement);
//         stack.Push(thirdElement);
//     }
//
//     [Fact]
//     public void should_Pop_last_item_first()
//     {
//         Assert.Equal(thirdElement, stack.Pop());
//     }
//
//     [Fact]
//     public void should_Pop_second_item_second()
//     {
//         Assert.Equal(secondElement, stack.Pop());
//     }
//
//     [Fact]
//     public void should_Pop_first_item_last()
//     {
//         Assert.Equal(firstElement, stack.Pop());
//     }
// }
//
// public class When_you_throw_an_exception_during_class_construction : Specification
// {
//     public When_you_throw_an_exception_during_class_construction()
//     {
//         throw new Exception();
//     }
//
//     [Fact]
//     public void should_fail()
//     {
//         // This test should display as having failed, even without any assertions being called
//     }
// }
//
// public class When_you_throw_an_exception_during_because_call : Specification
// {
//     protected override void Because()
//     {
//         throw new Exception();
//     }
//
//     [Fact]
//     public void should_fail()
//     {
//         // This test should display as having failed, even without any assertions being called
//     }
// }
