using System;
using UnityEngine;

namespace SerializableCallback
{
    public enum Mode
    {
        Value,
        Callback
    }
    
    [Serializable]
    public class SerializableValueCallback<T>
    {
#if UNITY_EDITOR
        public static string Editor_PropertyMode = nameof(_mode);
        public static string Editor_PropertyValue = nameof(_value);
        public static string Editor_PropertyCallback = nameof(_callback);
#endif
        
        [SerializeField] private Mode _mode;
        [SerializeField] private T _value;
        [SerializeField] private SerializableCallback<T> _callback;

        public T Value => _mode == Mode.Value ? _value : _callback.Invoke();
    }
}