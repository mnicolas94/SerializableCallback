using System;
using System.Collections.Generic;
using System.Linq;

public static class SerializableEventExtension
{
    #region 0 argument
    public static T SetCSharpAction<T>(this T @this, Action a) where T : SerializableEvent
    {
        @this.dynamic = true;
        @this.ClearCache();
        var tmp = new InvokableEvent(null, "");
        tmp.action = a;
        @this.invokable = tmp;
        return @this;
    }

    public static void Invoke<T>(this ICollection<T> @this) where T : SerializableEvent
    {
        foreach(var el in @this) el.Invoke();
    }
    public static void AddListener<T>(this ICollection<T> @this, Action a) where T:SerializableEvent, new()
        => @this.Add(new T().SetCSharpAction(a));
    public static void RemoveListener<T>(this ICollection<T> @this, Action a) where T : SerializableEvent
        => @this.Remove(@this.Where(i => (i.invokable as InvokableEvent).action == a).First());
    #endregion

    #region 1 argument
    public static T SetCSharpAction<T, U>(this T @this, Action<U> a) where T : SerializableEvent<U>
    {
        @this.dynamic = true;
        @this.ClearCache();
        var tmp = new InvokableEvent<U>(null, "");
        tmp.action = a;
        @this.invokable = tmp;
        return @this;
    }

    public static void Invoke<T, U>(this ICollection<T> @this, U arg0) where T : SerializableEvent<U>
    {
        foreach(var el in @this) el.Invoke(arg0);
    }
    public static void AddListener<T, U>(this ICollection<T> @this, Action<U> a) where T : SerializableEvent<U>, new()
        => @this.Add(new T().SetCSharpAction(a));
    public static void RemoveListener<T, U>(this ICollection<T> @this, Action<U> a) where T : SerializableEvent<U>
        => @this.Remove(@this.Where(i => (i.invokable as InvokableEvent<U>).action == a).First());
    #endregion

    #region 2 argument
    public static T SetCSharpAction<T, U, V>(this T @this, Action<U,V> a) where T : SerializableEvent<U,V>
    {
        @this.dynamic = true;
        @this.ClearCache();
        var tmp = new InvokableEvent<U,V>(null, "");
        tmp.action = a;
        @this.invokable = tmp;
        return @this;
    }

    public static void Invoke<T,U,V>(this ICollection<T> @this, U arg0, V arg1) where T : SerializableEvent<U,V>
    {
        foreach(var el in @this) el.Invoke(arg0, arg1);
    }
    public static void AddListener<T, U, V>(this ICollection<T> @this, Action<U,V> a) where T : SerializableEvent<U,V>, new()
        => @this.Add(new T().SetCSharpAction(a));
    public static void RemoveListener<T, U, V>(this ICollection<T> @this, Action<U,V> a) where T : SerializableEvent<U,V>
        => @this.Remove(@this.Where(i => (i.invokable as InvokableEvent<U,V>).action == a).First());
    #endregion

    #region 3 argument
    public static T SetCSharpAction<T, U, V, W>(this T @this, Action<U,V,W> a) where T : SerializableEvent<U,V,W>
    {
        @this.dynamic = true;
        @this.ClearCache();
        var tmp = new InvokableEvent<U,V,W>(null, "");
        tmp.action = a;
        @this.invokable = tmp;
        return @this;
    }

    public static void Invoke<T, U, V, W>(this ICollection<T> @this, U arg0, V arg1, W arg2) where T : SerializableEvent<U, V, W>
    {
        foreach(var el in @this) el.Invoke(arg0, arg1, arg2);
    }
    public static void AddListener<T, U, V, W>(this ICollection<T> @this, Action<U, V, W> a) where T : SerializableEvent<U, V, W>, new()
        => @this.Add(new T().SetCSharpAction(a));
    public static void RemoveListener<T, U, V, W>(this ICollection<T> @this, Action<U, V, W> a) where T : SerializableEvent<U, V, W>
        => @this.Remove(@this.Where(i => (i.invokable as InvokableEvent<U, V, W>).action == a).First());
    #endregion
}