using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SerializableCallback
{
	public abstract class SerializableCallbackBase<TReturn> : SerializableCallbackBase {
		public InvokableCallbackBase<TReturn> func;

		public override void ClearCache() {
			base.ClearCache();
			func = null;
		}

		protected abstract Type GetInvokableType();
		
		protected override void Cache()
		{
			var invokableType = GetInvokableType();
			if (_target == null || string.IsNullOrEmpty(_methodName))
			{
				func = Activator.CreateInstance(invokableType, new object[] { null, null }) as InvokableCallbackBase<TReturn>;
			}
			else
			{
				if (_dynamic)
				{
					if (_isStatic)
					{
						func = Activator.CreateInstance(invokableType, new object[] { TargetType, MethodName }) as InvokableCallbackBase<TReturn>;
					}
					else
					{
						func = Activator.CreateInstance(invokableType, new object[] { Target, MethodName }) as InvokableCallbackBase<TReturn>;
					}
				}
				else
				{
					func = GetPersistentMethod();
				}
			}
		}

		protected InvokableCallbackBase<TReturn> GetPersistentMethod() {
			Type[] types = new Type[ArgRealTypes.Length + 1];
			Array.Copy(ArgRealTypes, types, ArgRealTypes.Length);
			types[types.Length - 1] = typeof(TReturn);

			Type genericType;
			switch (types.Length) {
				case 1:
					genericType = typeof(InvokableCallback<>).MakeGenericType(types);
					break;
				case 2:
					genericType = typeof(InvokableCallback<,>).MakeGenericType(types);
					break;
				case 3:
					genericType = typeof(InvokableCallback<, ,>).MakeGenericType(types);
					break;
				case 4:
					genericType = typeof(InvokableCallback<, , ,>).MakeGenericType(types);
					break;
				case 5:
					genericType = typeof(InvokableCallback<, , , ,>).MakeGenericType(types);
					break;
				default:
					throw new ArgumentException(types.Length + "args");
			}

			object[] constructorArguments;
			if (_isStatic)
			{
				constructorArguments = new object[] { TargetType, MethodName };	
			}
			else
			{
				constructorArguments = new object[] { Target, MethodName };
			}
			return Activator.CreateInstance(genericType, constructorArguments) as InvokableCallbackBase<TReturn>;
		}
	}

	/// <summary> An inspector-friendly serializable function </summary>
	[Serializable]
	public abstract class SerializableCallbackBase : ISerializationCallbackReceiver {

		/// <summary> Target object </summary>
		public Object Target { get { return _target; } set { _target = value; ClearCache(); } }
		public Type TargetType { get { return Type.GetType(_targetTypeName); } }
		
		/// <summary> Target method name </summary>
		public string MethodName { get { return _methodName; } set { _methodName = value; ClearCache(); } }
		public object[] Args { get { return args != null ? args : args = _args.Select(x => x.GetValue()).ToArray(); } }
		public object[] args;
		public Type[] ArgTypes { get { return argTypes != null ? argTypes : argTypes = _args.Select(x => Arg.RealType(x.argType)).ToArray(); } }
		public Type[] argTypes;
		public Type[] ArgRealTypes { get { return argRealTypes != null ? argRealTypes : argRealTypes = _args.Select(x => Type.GetType(x._typeName)).ToArray(); } }
		public Type[] argRealTypes;
		public bool Dynamic { get { return _dynamic; } set { _dynamic = value; ClearCache(); } }

		// Target
		[SerializeField] protected bool _isStatic;
		[SerializeField] protected Object _target;
		[SerializeField] protected string _targetTypeName;
		
		// Method
		[SerializeField] protected string _methodName;
		[SerializeField] protected Arg[] _args;
		[SerializeField] protected bool _dynamic;
		
#pragma warning disable 0414
		[SerializeField] private string _typeName;
#pragma warning restore 0414

		[SerializeField] private bool dirty;

#if UNITY_EDITOR
		protected SerializableCallbackBase() {
			_typeName = GetType().AssemblyQualifiedName;
		}
#endif

		public virtual void ClearCache() {
			argTypes = null;
			args = null;
		}

		public void SetMethod(Object target, string methodName, bool dynamic, params Arg[] args) {
			_target = target;
			_methodName = methodName;
			_dynamic = dynamic;
			_args = args;
			_isStatic = false;
			ClearCache();
		}
		
		public void SetStaticMethod(Type targetType, string methodName, bool dynamic, params Arg[] args)
		{
			_targetTypeName = targetType.AssemblyQualifiedName;
            _methodName = methodName;
            _dynamic = dynamic;
            _args = args;
            _isStatic = true;
            ClearCache();
        }

		protected abstract void Cache();

		public void OnBeforeSerialize() {
#if UNITY_EDITOR
			if (dirty) { ClearCache(); dirty = false; }
#endif
		}

		public void OnAfterDeserialize() {
#if UNITY_EDITOR
			_typeName = base.GetType().AssemblyQualifiedName;
#endif
		}
	}
}