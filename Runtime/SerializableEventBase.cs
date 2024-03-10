using System;

namespace SerializableCallback
{
	public abstract class SerializableEventBase : SerializableCallbackBase {
		public InvokableEventBase invokable;

		public override void ClearCache() {
			base.ClearCache();
			invokable = null;
		}
		
		protected abstract Type GetInvokableType();
		
		protected override void Cache()
		{
			var invokableType = GetInvokableType();
			if ((_target == null && !_isStatic) || string.IsNullOrEmpty(_methodName))
			{
				invokable = Activator.CreateInstance(invokableType, new object[] { null, null }) as InvokableEventBase;
			}
			else
			{
				if (_dynamic)
				{
					if (_isStatic)
					{
						invokable = Activator.CreateInstance(invokableType, new object[] { TargetType, MethodName }) as InvokableEventBase;
					}
					else
					{
						invokable = Activator.CreateInstance(invokableType, new object[] { Target, MethodName }) as InvokableEventBase;
					}
				}
				else
				{
					invokable = GetPersistentMethod();
				}
			}
		}

		protected InvokableEventBase GetPersistentMethod() {
			var argumentsLength = ArgRealTypes.Length;
			Type[] types = new Type[argumentsLength];
			Array.Copy(ArgRealTypes, types, argumentsLength);

			Type genericType = null;
			switch (types.Length) {
				case 0:
					genericType = typeof(InvokableEvent);
					break;
				case 1:
					genericType = typeof(InvokableEvent<>).MakeGenericType(types);
					break;
				case 2:
					genericType = typeof(InvokableEvent<,>).MakeGenericType(types);
					break;
				case 3:
					genericType = typeof(InvokableEvent<, ,>).MakeGenericType(types);
					break;
				case 4:
					genericType = typeof(InvokableEvent<, , ,>).MakeGenericType(types);
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
			return Activator.CreateInstance(genericType, constructorArguments) as InvokableEventBase;
		}
	}
}