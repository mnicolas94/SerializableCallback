using System;

namespace SerializableCallback
{
	public abstract class InvokableEventBase {
		public abstract void Invoke(params object[] args);
		
		protected static TFunc CreateDelegate<TFunc>(object target, string methodName) where TFunc : Delegate
		{
			return (TFunc) Delegate.CreateDelegate(typeof(TFunc), target, methodName);
		}

		protected static TFunc CreateDelegate<TFunc>(Type targetType, string methodName) where TFunc : Delegate
		{
			return (TFunc) Delegate.CreateDelegate(typeof(TFunc), targetType, methodName);
		}
	}
	
	public abstract class InvokableEventBase<TFunc> : InvokableEventBase
		where TFunc : Delegate
	{
		public TFunc _action;

		protected abstract TFunc GetDefaultFunction();

		protected InvokableEventBase(object target, string methodName)
		{
			if (target == null || string.IsNullOrEmpty(methodName))
			{
				_action = GetDefaultFunction();
			}
			else
			{
				_action = CreateDelegate<TFunc>(target, methodName);
			}
		}

		protected InvokableEventBase(Type targetType, string methodName)
		{
			if (targetType == null || string.IsNullOrEmpty(methodName))
			{
				_action = GetDefaultFunction();
			}
			else
			{
				_action = CreateDelegate<TFunc>(targetType, methodName);
			}
		}
	}
}