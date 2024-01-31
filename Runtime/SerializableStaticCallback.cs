using System;
using UnityEngine;

namespace SerializableCallback
{
    [Serializable]
    public class SerializableStaticCallback<TReturn> : SerializableStaticCallbackBase<TReturn>
    {
        public virtual TReturn Invoke()
        {
            if (func == null || !_isStatic) Cache(null);
            InvokableStaticCallback<TReturn> call = func as InvokableStaticCallback<TReturn>;
            return call.Invoke();
        }

        protected override void Cache(params object[] args)
        {
            _isStatic = targetType.GetMethod(methodName).IsStatic;
            ///! with no argument it HAS to be static.
            func = new InvokableStaticCallback<TReturn>(targetType, methodName);
        }
    }


    [Serializable]
    public class SerializableStaticCallback<T0, TReturn> : SerializableStaticCallbackBase<TReturn>
    {
        public virtual TReturn Invoke(T0 arg0)
        {
            if (func == null || !_isStatic) Cache(arg0);
            if (isStatic)
            {
                InvokableStaticCallback<T0, TReturn> call = func as InvokableStaticCallback<T0, TReturn>;
                //Debug.Log("this is a static class");
                return call.Invoke(arg0);
            }
            else
            {
                InvokableStaticCallback<TReturn> call = func as InvokableStaticCallback<TReturn>;
                //Debug.Log("this is a instance");
                return call.Invoke();
            }
        }

        protected override void Cache(params object[] args)
        {
            _isStatic = targetType.GetMethod(methodName).IsStatic;
            if (isStatic)
            {
                func = new InvokableStaticCallback<T0, TReturn>(targetType, methodName);
                //Debug.Log("this is a static class");
            } 
            else
            {
                Debug.Log($" {methodName} is {_isStatic} and prefering instance {args[0]}");
                func = new InvokableStaticCallback<TReturn>(args[0], methodName);
                //Debug.Log("this is a instance class");
            }
        }
    }

    [Serializable]
    public class SerializableStaticCallback<T0,T1, TReturn> : SerializableStaticCallbackBase<TReturn>
    {
        public virtual TReturn Invoke(T0 arg0,T1 arg1)
        {
            if (func == null || !_isStatic) Cache(arg0,arg1);
            if (isStatic)
            {
                InvokableStaticCallback<T0, T1, TReturn> call = func as InvokableStaticCallback<T0, T1, TReturn>;
                //Debug.Log("this is a static class");
                return call.Invoke(arg0,arg1);
            }
            else
            {
                InvokableStaticCallback<T1, TReturn> call = func as InvokableStaticCallback<T1, TReturn>;
                //Debug.Log("this is a instance");
                return call.Invoke(arg1);
            }
        }

        protected override void Cache(params object[] args)
        {
            _isStatic = targetType.GetMethod(methodName).IsStatic;
            if (isStatic)
            {
                func = new InvokableStaticCallback<T0, T1, TReturn>(targetType, methodName);
                //Debug.Log("this is a static class");
            }
            else
            {
                func = new InvokableStaticCallback<T1, TReturn>(args[0], methodName);
                //Debug.Log("this is a instance class");
            }
        }
    }

    [Serializable]
    public class SerializableStaticCallback<T0, T1, T2, TReturn> : SerializableStaticCallbackBase<TReturn>
    {
        public virtual TReturn Invoke(T0 arg0, T1 arg1, T2 arg2)
        {
            if (func == null || !_isStatic) Cache(arg0,arg1,arg2);
            if (isStatic)
            {
                InvokableStaticCallback<T0, T1, T2, TReturn> call = func as InvokableStaticCallback<T0, T1, T2, TReturn>;
                //Debug.Log("this is a static class");
                return call.Invoke(arg0, arg1, arg2);
            }
            else
            {
                InvokableStaticCallback<T1, T2, TReturn> call = func as InvokableStaticCallback<T1, T2, TReturn>;
                //Debug.Log("this is a instance");
                return call.Invoke(arg1, arg2);
            }
        }

        protected override void Cache(params object[] args)
        {
            _isStatic = targetType.GetMethod(methodName).IsStatic; 
            if (isStatic)
            {
                func = new InvokableStaticCallback<T0, T1, T2, TReturn>(targetType, methodName);
                //Debug.Log("this is a static class");
            }
            else
            {
                func = new InvokableStaticCallback<T1, T2, TReturn>(args[0], methodName);
                //Debug.Log("this is a instance class");
            }
        }
    }

    [Serializable]
    public class SerializableStaticCallback<T0, T1, T2, T3, TReturn> : SerializableStaticCallbackBase<TReturn>
    {
        public virtual TReturn Invoke(T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            if (func == null || !_isStatic) Cache(arg0, arg1, arg2, arg3);
            if (isStatic)
            {
                InvokableStaticCallback<T0, T1, T2, T3, TReturn> call = func as InvokableStaticCallback<T0, T1, T2, T3, TReturn>;
                //Debug.Log("this is a static class");
                return call.Invoke(arg0, arg1, arg2, arg3);
            }
            else
            {
                InvokableStaticCallback<T1, T2, T3, TReturn> call = func as InvokableStaticCallback<T1, T2, T3, TReturn>;
                //Debug.Log("this is a instance");
                return call.Invoke(arg1, arg2, arg3);
            }
        }

        protected override void Cache(params object[] args)
        {
            _isStatic = targetType.GetMethod(methodName).IsStatic;
            if (isStatic)
            {
                func = new InvokableStaticCallback<T0, T1, T2, T3, TReturn>(targetType, methodName);
                //Debug.Log("this is a static class");
            }
            else
            {
                func = new InvokableStaticCallback<T1, T2, T3, TReturn>(args[0], methodName);
                //Debug.Log("this is a instance class");
            }
        }
    }
}