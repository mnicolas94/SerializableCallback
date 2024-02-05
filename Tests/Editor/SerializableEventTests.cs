using System;
using NUnit.Framework;
using UnityEngine;

namespace SerializableCallback.Tests.Editor
{
    public class SerializableEventTests
    {
        [Test]
        public void When_SetMethod_And_Invoke_Then_CallbackIsInvoked() {
    
            // Arrange
            var evt = new SerializableEvent();
            var target = ScriptableObject.CreateInstance<TestsEventsTarget>();
            int callCount = 0;
            target.OnEvent += () => callCount++;
    
            // Act 
            evt.SetMethod<Action>(target.OnEventMethod, true);
            evt.Invoke();
    
            // Assert
            Assert.AreEqual(1, callCount);
        }

        [Test]
        public void When_SetMethodWithArgs_And_InvokeWithArgs_Then_CallbackIsInvoked() {

            // Arrange
            var evt = new SerializableEvent<int, string>();
            var target = ScriptableObject.CreateInstance<TestsEventsTarget>();
            int arg1 = 10;
            string arg2 = "Test";
            string result = "";
            target.OnEventWithArgs += (i, s) => result = s + i;
    
            // Act
            evt.SetMethod<Action<int, string>>(target.OnEventWithArgsMethod, true);
            evt.Invoke(arg1, arg2);
    
            // Assert
            Assert.AreEqual(arg2 + arg1, result);
        }

        [Test]
        public void When_SetStaticMethod_And_Invoke_Then_CallbackIsInvoked() {

            // Arrange
            var evt = new SerializableEvent();
            TestsEventsTarget.StaticCallCount = 0;
            evt.SetMethod<Action>(TestsEventsTarget.StaticEvent, true);
    
            // Act
            evt.Invoke();
    
            // Assert
            Assert.AreEqual(1, TestsEventsTarget.StaticCallCount);
        }
    }
    
    public class TestsEventsTarget : ScriptableObject
    {
        public static int StaticCallCount;

        public static void StaticEvent()
        {
            StaticCallCount++;
        }
        
        public Action OnEvent;
        public void OnEventMethod()
        {
            OnEvent();
        }
  
        public Action<int, string> OnEventWithArgs;
        public void OnEventWithArgsMethod(int arg1, string arg2)
        {
            OnEventWithArgs(arg1, arg2);
        }
    }
}