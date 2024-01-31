using System;
using System.Linq;
using B83;
using UnityEngine;

namespace SerializableCallback
{
    public abstract class SerializableStaticCallbackBase<TReturn> : SerializableStaticCallbackBase 
    { 
        public InvokableCallbackBase<TReturn> func;
    
        public override void ClearCache()
        {
            base.ClearCache();
            func = null;
        }

        protected InvokableCallbackBase<TReturn> GetPersistentMethod()
        {
            Type[] types = new Type[ArgRealTypes.Length + 1];
            Array.Copy(ArgRealTypes, types, ArgRealTypes.Length);
            types[types.Length - 1] = typeof(TReturn);

            Type genericType = null;
            switch (types.Length)
            {
                case 1:
                    genericType = typeof(InvokableStaticCallback<>).MakeGenericType(types);
                    break;
                case 2:
                    genericType = typeof(InvokableStaticCallback<,>).MakeGenericType(types);
                    break;
                case 3:
                    genericType = typeof(InvokableStaticCallback<,,>).MakeGenericType(types);
                    break;
                case 4:
                    genericType = typeof(InvokableStaticCallback<,,,>).MakeGenericType(types);
                    break;
                case 5:
                    genericType = typeof(InvokableStaticCallback<,,,,>).MakeGenericType(types);
                    break;
                default:
                    throw new ArgumentException(types.Length + "args");
            }
            return Activator.CreateInstance(genericType, new object[] { targetType, methodName }) as InvokableCallbackBase<TReturn>;
        }
    }

    /// <summary> An inspector-friendly serializable function </summary>
    [System.Serializable]

    public abstract class SerializableStaticCallbackBase : ISerializationCallbackReceiver
    {

        /// <summary> Target object </summary>
        public Type targetType { get { return _targetType.Type; } }
    
        /// <summary> Target method name </summary>
        public string methodName { get { return _methodName; } set { _methodName = value; ClearCache(); } }
        public object[] Args { get { return args != null ? args : args = _args.Select(x => x.GetValue()).ToArray(); } }
        public object[] args;
        public Type[] ArgTypes { get { return argTypes != null ? argTypes : argTypes = _args.Select(x => Arg.RealType(x.argType)).ToArray(); } }
        public Type[] argTypes;
        public Type[] ArgRealTypes { get { return argRealTypes != null ? argRealTypes : argRealTypes = _args.Select(x => Type.GetType(x._typeName,true)).ToArray(); } }
        public Type[] argRealTypes;
        public bool dynamic { get { return _dynamic; } set { _dynamic = value; ClearCache(); } }


        [SerializeField] protected SerializableMonoScript _targetType;
        [SerializeField] protected string _methodName;
        [SerializeField] protected Arg[] _args;
        [SerializeField] protected bool _dynamic;
#pragma warning disable 0414
        [SerializeField] private string _typeName;
#pragma warning restore 0414

        [SerializeField] private bool dirty;

#if UNITY_EDITOR
        protected SerializableStaticCallbackBase()
        {
            _typeName = base.GetType().AssemblyQualifiedName;
        }
#endif

        public virtual void ClearCache()
        {
            argTypes = null;
            args = null;
        }

        public void SetMethod(string methodName, bool dynamic, params Arg[] args)
        {
            _methodName = methodName;
            _dynamic = dynamic;
            _args = args;
            ClearCache();
        }

        protected abstract void Cache(params object[] args);

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (dirty) { ClearCache(); dirty = false; }
#endif
        }

        public void OnAfterDeserialize()
        {
#if UNITY_EDITOR
            _typeName = base.GetType().AssemblyQualifiedName;
#endif
        }
    }
}