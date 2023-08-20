using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SerializableStaticCallback<TReturn> : SerializableStaticCallbackBase<TReturn>
{
    public TReturn Invoke()
    {
        if (func == null) Cache();
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

    protected override void Cache()
    {
        //if (_target == null || string.IsNullOrEmpty(_methodName))
        //{
        //func = new InvokableStaticCallback<TReturn>(null, null);
        //}
        //else
        //{
        //    if (_dynamic)
        //    {
        func = new InvokableStaticCallback<TReturn>(targetType, methodName);
        //    }
        //    else
        //    {
        //        func = GetPersistentMethod();
        //    }
        //}
    }
}


public abstract class SerializableStaticCallback<T0, TReturn> : SerializableStaticCallbackBase<TReturn>
{
    public TReturn Invoke(T0 arg0)
    {
        args = new object[] { arg0 };
        if (func == null) Cache();
        if (_dynamic)
        {
            //InvokableStaticCallback<T0, TReturn> call = func as InvokableStaticCallback<T0, TReturn>;
            //return call.Invoke(arg0);

            try
            {

                InvokableStaticCallback<T0, TReturn> call = func as InvokableStaticCallback<T0, TReturn>;
                Debug.Log("this is a static class");
                return call.Invoke(arg0);
            }
            catch
            {
                InvokableStaticCallback<TReturn> call = func as InvokableStaticCallback<TReturn>;
                Debug.Log("this is a instance");
                return call.Invoke();
            }

        }
        else
        {
            return func.Invoke(Args);
        }
    }

    protected override void Cache()
    {

        //if (_targetType == null || string.IsNullOrEmpty(_methodName))
        //{
        //    func = new InvokableStaticCallback<T0, TReturn>(null, null);
        //}
        //else
        //{
        //    if (_dynamic)
        //    {

        //func = new InvokableStaticCallback<T0, TReturn>(targetType, methodName);
        //return;

        try
        {
            func = new InvokableStaticCallback<T0, TReturn>(targetType, methodName);
            Debug.Log("this is a static class");
        } catch
        {
            
            Debug.Log($"prefering instance {args[0]}");
            func = new InvokableStaticCallback<TReturn>(args[0], methodName);
            Debug.Log("this is a instance class");
        }
        //    }
        //    else
        //    {
        //        func = GetPersistentMethod();
        //    }
        //}
    }
}
