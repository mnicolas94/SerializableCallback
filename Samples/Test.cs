using System;
using UnityEngine;

namespace SerializableCallback.Samples
{
    [Serializable]
    public class ParamCallback : SerializableCallback<int, float>{}


    public class Test : MonoBehaviour
    {
        [SerializeField] private SerializableCallback<float> _floatCallback;
        [SerializeField] private SerializableCallback<int, float> _parameterCallback;
        [SerializeField] private ParamCallback _parameterCallbackF;

        public float FloatFunction()
        {
            return 42.0f;
        }

        public float ParamFunction(int param)
        {
            return param;
        }
    }
}
