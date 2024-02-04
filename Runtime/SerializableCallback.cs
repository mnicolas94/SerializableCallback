using System;

namespace SerializableCallback
{
	[Serializable]
	public class SerializableCallback<TReturn> : SerializableCallbackBase<TReturn>
	{
		public TReturn Invoke()
		{
			if (func == null) Cache();
			if (_dynamic)
			{
				InvokableCallback<TReturn> call = func as InvokableCallback<TReturn>;
				return call.Invoke();
			}
			else
			{
				return func.Invoke(Args);
			}
		}

		protected override Type GetInvokableType()
		{
			return typeof(InvokableCallback<TReturn>);
		}
	}

	[Serializable]
	public class SerializableCallback<T0, TReturn> : SerializableCallbackBase<TReturn>
	{
		public TReturn Invoke(T0 arg0)
		{
			if (func == null) Cache();
			if (_dynamic)
			{
				InvokableCallback<T0, TReturn> call = func as InvokableCallback<T0, TReturn>;
				return call.Invoke(arg0);
			}
			else
			{
				return func.Invoke(Args);
			}
		}

		protected override Type GetInvokableType()
		{
			return typeof(InvokableCallback<T0, TReturn>);
		}
	}

	[Serializable]
	public class SerializableCallback<T0, T1, TReturn> : SerializableCallbackBase<TReturn>
	{
		public TReturn Invoke(T0 arg0, T1 arg1)
		{
			if (func == null) Cache();
			if (_dynamic)
			{
				InvokableCallback<T0, T1, TReturn> call = func as InvokableCallback<T0, T1, TReturn>;
				return call.Invoke(arg0, arg1);
			}
			else
			{
				return func.Invoke(Args);
			}
		}

		protected override Type GetInvokableType()
		{
			return typeof(InvokableCallback<T0, T1, TReturn>);
		}
	}

	[Serializable]
	public class SerializableCallback<T0, T1, T2, TReturn> : SerializableCallbackBase<TReturn>
	{
		public TReturn Invoke(T0 arg0, T1 arg1, T2 arg2)
		{
			if (func == null) Cache();
			if (_dynamic)
			{
				InvokableCallback<T0, T1, T2, TReturn> call = func as InvokableCallback<T0, T1, T2, TReturn>;
				return call.Invoke(arg0, arg1, arg2);
			}
			else
			{
				return func.Invoke(Args);
			}
		}

		protected override Type GetInvokableType()
		{
			return typeof(InvokableCallback<T0, T1, T2, TReturn>);
		}
	}

	[Serializable]
	public class SerializableCallback<T0, T1, T2, T3, TReturn> : SerializableCallbackBase<TReturn>
	{
		public TReturn Invoke(T0 arg0, T1 arg1, T2 arg2, T3 arg3)
		{
			if (func == null) Cache();
			if (_dynamic)
			{
				InvokableCallback<T0, T1, T2, T3, TReturn> call = func as InvokableCallback<T0, T1, T2, T3, TReturn>;
				return call.Invoke(arg0, arg1, arg2, arg3);
			}
			else
			{
				return func.Invoke(Args);
			}
		}

		protected override Type GetInvokableType()
		{
			return typeof(InvokableCallback<T0, T1, T2, T3, TReturn>);
		}
	}
}