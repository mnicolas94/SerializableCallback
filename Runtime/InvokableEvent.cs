using System;

namespace SerializableCallback
{
	public class InvokableEvent : InvokableEventBase<Action>
	{
		public InvokableEvent(object target, string methodName) : base(target, methodName)
		{
		}

		public InvokableEvent(Type targetType, string methodName) : base(targetType, methodName)
		{
		}

		public override void Invoke(params object[] args)
		{
			_action();
		}
        
		public void Invoke()
		{
			_action();
		}

		protected override Action GetDefaultFunction()
		{
			return () => { };
		}
	}

	public class InvokableEvent<T0> : InvokableEventBase<Action<T0>>
	{
		public InvokableEvent(object target, string methodName) : base(target, methodName)
		{
		}

		public InvokableEvent(Type targetType, string methodName) : base(targetType, methodName)
		{
		}

		public override void Invoke(params object[] args)
		{
			_action((T0) args[0]);
		}
        
		public void Invoke(T0 x)
		{
			_action(x);
		}

		protected override Action<T0> GetDefaultFunction()
		{
			return x => { };
		}
	}

	public class InvokableEvent<T0, T1> : InvokableEventBase<Action<T0, T1>>
	{
		public InvokableEvent(object target, string methodName) : base(target, methodName)
		{
		}

		public InvokableEvent(Type targetType, string methodName) : base(targetType, methodName)
		{
		}

		public override void Invoke(params object[] args)
		{
			_action((T0) args[0], (T1) args[1]);
		}
        
		public void Invoke(T0 x, T1 y)
		{
			_action(x, y);
		}

		protected override Action<T0, T1> GetDefaultFunction()
		{
			return (x, y) => { };
		}

	}

	public class InvokableEvent<T0, T1, T2> : InvokableEventBase<Action<T0, T1, T2>>
	{
		public InvokableEvent(object target, string methodName) : base(target, methodName)
		{
		}

		public InvokableEvent(Type targetType, string methodName) : base(targetType, methodName)
		{
		}

		public override void Invoke(params object[] args)
		{
			_action((T0) args[0], (T1) args[1], (T2) args[2]);
		}
        
		public void Invoke(T0 x, T1 y, T2 z)
		{
			_action(x, y, z);
		}

		protected override Action<T0, T1, T2> GetDefaultFunction()
		{
			return (x, y, z) => { };
		}
	}

	public class InvokableEvent<T0, T1, T2, T3> : InvokableEventBase<Action<T0, T1, T2, T3>>
	{
		public InvokableEvent(object target, string methodName) : base(target, methodName)
		{
		}

		public InvokableEvent(Type targetType, string methodName) : base(targetType, methodName)
		{
		}

		public override void Invoke(params object[] args)
		{
			_action((T0) args[0], (T1) args[1], (T2) args[2], (T3) args[3]);;
		}
        
		public void Invoke(T0 x, T1 y, T2 z, T3 w)
		{
			_action(x, y, z ,w);
		}

		protected override Action<T0, T1, T2, T3> GetDefaultFunction()
		{
			return (x, y, z, w) => { };
		}
	}
}