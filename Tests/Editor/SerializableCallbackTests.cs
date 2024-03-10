using System;
using NUnit.Framework;
using UnityEngine;

namespace SerializableCallback.Tests.Editor
{
    public class SerializableCallbackTests
    {
        [Test]
        public void When_SetMethod_And_Invoke_Then_ResultIsExpected()
        {
            // arrange
            var callback = new SerializableCallback<float>();
            var callbacksTarget = ScriptableObject.CreateInstance<TestsCallbacksTarget>();
            callbacksTarget.ExpectedResult = 42f;
            callback.SetMethod<Func<float>>(callbacksTarget.FloatFunction, true);
            
            // act
            var result = callback.Invoke();

            // assert
            Assert.AreEqual(callbacksTarget.ExpectedResult, result);
        }
        
        [Test]
        public void When_SetStaticMethod_And_Invoke_Then_ResultIsExpected()
        {
            // arrange
            var callback = new SerializableCallback<string>();
            callback.SetMethod<Func<string>>(TestsCallbacksTarget.StaticStringFunction, true);
            TestsCallbacksTarget.ExpectedStaticResult = "Static";
            
            // act
            var result = callback.Invoke();

            // assert
            Assert.AreEqual(TestsCallbacksTarget.ExpectedStaticResult, result);
        }
        
        [Test]
        public void When_SetNotDynamicMethod_And_Invoke_Then_ResultIsExpected()
        {
            // arrange
            var callback = new SerializableCallback<string>();
            var callbacksTarget = ScriptableObject.CreateInstance<TestsCallbacksTarget>();
            var firstArgument = 1;
            var secondArgument = 2f;
            var args = new Arg[]{Arg.FromValue(firstArgument), Arg.FromValue(secondArgument)};
            var expected = callbacksTarget.StringFunctionWithArgs(firstArgument, secondArgument);
            callback.SetMethod<Func<int, float, string>>(callbacksTarget.StringFunctionWithArgs, false, args);
            
            // act
            var result = callback.Invoke();

            // assert
            Assert.AreEqual(expected, result);
        }
        
        [Test]
        public void When_SetDynamicMethod_And_InvokeWithArgs_Then_ResultIsExpected()
        {
            // arrange
            var callback = new SerializableCallback<int, float, string>();
            var callbacksTarget = ScriptableObject.CreateInstance<TestsCallbacksTarget>();
            var firstArgument = 1;
            var secondArgument = 2f;
            var expected = callbacksTarget.StringFunctionWithArgs(firstArgument, secondArgument);
            callback.SetMethod<Func<int, float, string>>(callbacksTarget.StringFunctionWithArgs, true);
            
            // act
            var result = callback.Invoke(firstArgument, secondArgument);

            // assert
            Assert.AreEqual(expected, result);
        }
        
        [Test]
        public void When_InvokeCallbackWithNoMethod_ReturnsDefault()
        {
            // arrange
            var callback = new SerializableCallback<string>();
            string expected = default;
            
            // act
            var result = callback.Invoke();

            // assert
            Assert.AreEqual(expected, result);
        }
        
        [Test]
        public void When_InvokeCallbackNonDynamycally_WithObjectChildTypeArgument_Then_DoNotThrowError() {

            // Arrange
            var callback = new SerializableCallback<bool>();
            var target = ScriptableObject.CreateInstance<TestsCallbacksTarget>();
            var argument = ScriptableObject.CreateInstance<TestsCallbacksTarget>();
    
            // Act and assert
            var args = new Arg[]{Arg.FromValue(argument)};
            callback.SetMethod<Func<TestsCallbacksTarget, bool>>(target.MethodWithScriptableObjectArgument, false, args);
            callback.Invoke();
        }
    }

    public class TestsCallbacksTarget : ScriptableObject
    {
        public object ExpectedResult;
        public static object ExpectedStaticResult;
        
        public float FloatFunction()
        {
            return (float)ExpectedResult;
        }
        
        public string StringFunctionWithArgs(int integer, float floatingPoint)
        {
            return $"{integer}-{floatingPoint}";
        }

        public static string StaticStringFunction()
        {
            return (string) ExpectedStaticResult;
        }
        
        public bool MethodWithScriptableObjectArgument(TestsCallbacksTarget argument)
        {
            return true;
        }
    }
}