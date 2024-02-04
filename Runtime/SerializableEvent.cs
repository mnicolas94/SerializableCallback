using System;

namespace SerializableCallback
{
	[Serializable]
	public class SerializableEvent : SerializableEventBase {
		public void Invoke() {
			if (invokable == null) Cache();
			if (_dynamic)
			{
				InvokableEvent call = invokable as InvokableEvent;
				call.Invoke();
			}
			else
			{
				invokable.Invoke(Args);
			}
		}

		protected override Type GetInvokableType()
		{
			return typeof(InvokableEvent);
		}
	}

	[Serializable]
	public class SerializableEvent<T0> : SerializableEventBase {
		public void Invoke(T0 arg0) {
			if (invokable == null) Cache();
			if (_dynamic) {
				InvokableEvent<T0> call = invokable as InvokableEvent<T0>;
				call.Invoke(arg0);
			} else {
				invokable.Invoke(Args);
			}
		}

		protected override Type GetInvokableType()
		{
			return typeof(InvokableEvent<T0>);
		}
	}

	[Serializable]
	public class SerializableEvent<T0, T1> : SerializableEventBase {
		public void Invoke(T0 arg0, T1 arg1) {
			if (invokable == null) Cache();
			if (_dynamic) {
				InvokableEvent<T0, T1> call = invokable as InvokableEvent<T0, T1>;
				call.Invoke(arg0, arg1);
			} else {
				invokable.Invoke(Args);
			}
		}

		protected override Type GetInvokableType()
		{
			return typeof(InvokableEvent<T0, T1>);
		}
	}

	[Serializable]
	public class SerializableEvent<T0, T1, T2> : SerializableEventBase {
		public void Invoke(T0 arg0, T1 arg1, T2 arg2) {
			if (invokable == null) Cache();
			if (_dynamic) {
				InvokableEvent<T0, T1, T2> call = invokable as InvokableEvent<T0, T1, T2>;
				call.Invoke(arg0, arg1, arg2);
			} else {
				invokable.Invoke(Args);
			}
		}

		protected override Type GetInvokableType()
		{
			return typeof(InvokableEvent<T0, T1, T2>);
		}
	}

	[Serializable]
	public class SerializableEvent<T0, T1, T2, T3> : SerializableEventBase {
		public void Invoke(T0 arg0, T1 arg1, T2 arg2, T3 arg3) {
			if (invokable == null) Cache();
			if (_dynamic) {
				InvokableEvent<T0, T1, T2, T3> call = invokable as InvokableEvent<T0, T1, T2, T3>;
				call.Invoke(arg0, arg1, arg2, arg3);
			} else {
				invokable.Invoke(Args);
			}
		}

		protected override Type GetInvokableType()
		{
			return typeof(InvokableEvent<T0, T1, T2, T3>);
		}
	}
}