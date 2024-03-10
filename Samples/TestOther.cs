using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SerializableCallback.Samples
{
    public class TestOther : MonoBehaviour
    {
        
        public async void AsyncMethod()
        {
            Debug.Log(nameof(AsyncMethod));
        }
        
        public async void AsyncMethodObject(Object o)
        {
            Debug.Log($"{nameof(AsyncMethod)} : {o.name}");
        }
        
        public async void AsyncMethodSo(ScriptableObject so)
        {
            Debug.Log($"{nameof(AsyncMethod)} : {so.name}");
        }
        
        public async void AsyncMethodFloat(float f)
        {
            Debug.Log($"{nameof(AsyncMethod)} : {f}");
        }
        
        public async void AsyncMethodClass(SerializableClass sc)
        {
            Debug.Log($"{nameof(AsyncMethod)} : {sc}");
        }
        
        public async void AsyncMethodStruct(SerializableStruct ss)
        {
            Debug.Log($"{nameof(AsyncMethod)} : {ss}");
        }
    }

    [Serializable]
    public class SerializableClass
    {
        public int i;
    }
    
    [Serializable]
    public class SerializableStruct
    {
        public int i;
    }
}