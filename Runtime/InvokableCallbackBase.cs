using System;

namespace SerializableCallback
{
	public abstract class InvokableCallbackBase<TReturn>
	{
		public abstract TReturn Invoke(params object[] args);
		
		protected static TFunc CreateDelegate<TFunc>(object target, string methodName) where TFunc : Delegate
		{
			return (TFunc) Delegate.CreateDelegate(typeof(TFunc), target, methodName);
		}

		protected static TFunc CreateDelegate<TFunc>(Type targetType, string methodName) where TFunc : Delegate
		{
			return (TFunc) Delegate.CreateDelegate(typeof(TFunc), targetType, methodName);
		}
	}
	
	public abstract class InvokableCallbackBase<TFunc, TReturn> : InvokableCallbackBase<TReturn>
		where TFunc : Delegate
	{
		protected TFunc _function;

		protected abstract TFunc GetDefaultFunction();

		protected InvokableCallbackBase(object target, string methodName)
		{
			if (target == null || string.IsNullOrEmpty(methodName))
			{
				_function = GetDefaultFunction();
			}
			else
			{
				_function = CreateDelegate<TFunc>(target, methodName);
			}
		}

		protected InvokableCallbackBase(Type targetType, string methodName)
		{
			if (targetType == null || string.IsNullOrEmpty(methodName))
			{
				_function = GetDefaultFunction();
			}
			else
			{
				_function = CreateDelegate<TFunc>(targetType, methodName);
			}
		}
	}
}