using System;

namespace SerializableCallback
{
    public class InvokableCallback<TReturn> : InvokableCallbackBase<Func<TReturn>, TReturn>
    {
        public InvokableCallback(object target, string methodName) : base(target, methodName)
        {
        }

        public InvokableCallback(Type targetType, string methodName) : base(targetType, methodName)
        {
        }

        public override TReturn Invoke(params object[] args)
        {
            return _function();
        }
        
        public TReturn Invoke()
        {
            return _function();
        }

        protected override Func<TReturn> GetDefaultFunction()
        {
            return () => default;
        }
    }

    public class InvokableCallback<T0, TReturn> : InvokableCallbackBase<Func<T0, TReturn>, TReturn>
    {
        public InvokableCallback(object target, string methodName) : base(target, methodName)
        {
        }

        public InvokableCallback(Type targetType, string methodName) : base(targetType, methodName)
        {
        }

        public override TReturn Invoke(params object[] args)
        {
            return _function((T0) args[0]);
        }
        
        public TReturn Invoke(T0 arg0)
        {
            return _function(arg0);
        }

        protected override Func<T0, TReturn> GetDefaultFunction()
        {
            return x => default;
        }
    }

    public class InvokableCallback<T0, T1, TReturn> : InvokableCallbackBase<Func<T0, T1, TReturn>, TReturn>
    {
        public InvokableCallback(object target, string methodName) : base(target, methodName)
        {
        }

        public InvokableCallback(Type targetType, string methodName) : base(targetType, methodName)
        {
        }

        public override TReturn Invoke(params object[] args)
        {
            return _function((T0) args[0], (T1) args[1]);
        }
        
        public TReturn Invoke(T0 arg0, T1 arg1)
        {
            return _function(arg0, arg1);
        }

        protected override Func<T0, T1, TReturn> GetDefaultFunction()
        {
            return (x, y) => default;
        }
    }

    public class InvokableCallback<T0, T1, T2, TReturn> : InvokableCallbackBase<Func<T0, T1, T2, TReturn>, TReturn>
    {
        public InvokableCallback(object target, string methodName) : base(target, methodName)
        {
        }

        public InvokableCallback(Type targetType, string methodName) : base(targetType, methodName)
        {
        }

        public override TReturn Invoke(params object[] args)
        {
            return _function((T0) args[0], (T1) args[1], (T2) args[2]);
        }
        
        public TReturn Invoke(T0 arg0, T1 arg1, T2 arg2)
        {
            return _function(arg0, arg1, arg2);
        }

        protected override Func<T0, T1, T2, TReturn> GetDefaultFunction()
        {
            return (x, y, z) => default;
        }
    }

    public class InvokableCallback<T0, T1, T2, T3, TReturn> : InvokableCallbackBase<Func<T0, T1, T2, T3, TReturn>, TReturn>
    {
        public InvokableCallback(object target, string methodName) : base(target, methodName)
        {
        }

        public InvokableCallback(Type targetType, string methodName) : base(targetType, methodName)
        {
        }

        public override TReturn Invoke(params object[] args)
        {
            return _function((T0) args[0], (T1) args[1], (T2) args[2], (T3) args[3]);
        }
        
        public TReturn Invoke(T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            return _function(arg0, arg1, arg2, arg3);
        }

        protected override Func<T0, T1, T2, T3, TReturn> GetDefaultFunction()
        {
            return (x, y, z, w) => default;
        }
    }
}