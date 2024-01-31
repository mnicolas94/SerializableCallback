using System;

namespace SerializableCallback
{
    [Serializable]
    public class SerializableStaticCallback<TReturn> : SerializableStaticCallbackBase<TReturn>
    {
        public virtual TReturn Invoke()
        {
            if (func == null) Cache(null);
            
            if (_dynamic)
            {
                InvokableStaticCallback<TReturn> call = func as InvokableStaticCallback<TReturn>;
                return call.Invoke();
            }
            else
            {
                return func.Invoke(Args);
            }
        }

        protected override void Cache(params object[] args)
        {
            if (_targetType == null || _targetType.Type == null || string.IsNullOrEmpty(_methodName))
            {
                func = new InvokableStaticCallback<TReturn>(null, null);
            }
            else
            {
                if (_dynamic)
                {
                    func = new InvokableStaticCallback<TReturn>(targetType, methodName);
                }
                else
                {
                    func = GetPersistentMethod();
                }
            }
        }
    }

    [Serializable]
    public class SerializableStaticCallback<T0, TReturn> : SerializableStaticCallbackBase<TReturn>
    {
        public virtual TReturn Invoke(T0 arg0)
        {
            if (func == null) Cache(arg0);
            
            if (_dynamic)
            {
                InvokableStaticCallback<T0, TReturn> call = func as InvokableStaticCallback<T0, TReturn>;
                return call.Invoke(arg0);
            }
            else
            {
                return func.Invoke(Args);
            }
        }

        protected override void Cache(params object[] args)
        {
            if (_targetType == null || _targetType.Type == null || string.IsNullOrEmpty(_methodName))
            {
                func = new InvokableCallback<T0, TReturn>(null, null);
            }
            else
            {
                if (_dynamic)
                {
                    func = new InvokableStaticCallback<T0, TReturn>(targetType, methodName);
                }
                else
                {
                    func = GetPersistentMethod();
                }
            }
        }
    }

    [Serializable]
    public class SerializableStaticCallback<T0,T1, TReturn> : SerializableStaticCallbackBase<TReturn>
    {
        public virtual TReturn Invoke(T0 arg0,T1 arg1)
        {
            if (func == null) Cache(arg0,arg1);
            
            if (_dynamic)
            {
                InvokableStaticCallback<T0, T1, TReturn> call = func as InvokableStaticCallback<T0, T1, TReturn>;
                return call.Invoke(arg0,arg1);
            }
            else
            {
                return func.Invoke(Args);
            }
        }

        protected override void Cache(params object[] args)
        {
            if (_targetType == null || _targetType.Type == null || string.IsNullOrEmpty(_methodName))
            {
                func = new InvokableCallback<T0, T1, TReturn>(null, null);
            }
            else
            {
                if (_dynamic)
                {
                    func = new InvokableStaticCallback<T0, T1, TReturn>(targetType, methodName);
                }
                else
                {
                    func = GetPersistentMethod();
                }
            }
        }
    }

    [Serializable]
    public class SerializableStaticCallback<T0, T1, T2, TReturn> : SerializableStaticCallbackBase<TReturn>
    {
        public virtual TReturn Invoke(T0 arg0, T1 arg1, T2 arg2)
        {
            if (func == null) Cache(arg0,arg1,arg2);
            
            if (_dynamic)
            {
                InvokableStaticCallback<T0, T1, T2, TReturn> call = func as InvokableStaticCallback<T0, T1, T2, TReturn>;
                return call.Invoke(arg0, arg1, arg2);
            }
            else
            {
                return func.Invoke(Args);
            }
        }

        protected override void Cache(params object[] args)
        {
            if (_targetType == null || _targetType.Type == null || string.IsNullOrEmpty(_methodName))
            {
                func = new InvokableCallback<T0, T1, T2, TReturn>(null, null);
            }
            else
            {
                if (_dynamic)
                {
                    func = new InvokableStaticCallback<T0, T1, T2, TReturn>(targetType, methodName);
                }
                else
                {
                    func = GetPersistentMethod();
                }
            }
        }
    }

    [Serializable]
    public class SerializableStaticCallback<T0, T1, T2, T3, TReturn> : SerializableStaticCallbackBase<TReturn>
    {
        public virtual TReturn Invoke(T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            if (func == null) Cache(arg0,arg1,arg2);
            
            if (_dynamic)
            {
                InvokableStaticCallback<T0, T1, T2, T3, TReturn> call = func as InvokableStaticCallback<T0, T1, T2, T3, TReturn>;
                return call.Invoke(arg0, arg1, arg2, arg3);
            }
            else
            {
                return func.Invoke(Args);
            }
        }

        protected override void Cache(params object[] args)
        {
            if (_targetType == null || _targetType.Type == null || string.IsNullOrEmpty(_methodName))
            {
                func = new InvokableCallback<T0, T1, T2, T3, TReturn>(null, null);
            }
            else
            {
                if (_dynamic)
                {
                    func = new InvokableStaticCallback<T0, T1, T2, T3, TReturn>(targetType, methodName);
                }
                else
                {
                    func = GetPersistentMethod();
                }
            }
        }
    }
}