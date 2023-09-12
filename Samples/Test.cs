using System;
using System.Threading;
using UnityEngine;

namespace SerializableCallback.Samples
{
    [Serializable]
    public class ParamCallback : SerializableCallback<int, float>{}


    public class Test : MonoBehaviour
    {
        [SerializeField] private SerializableCallback<float> _floatCallback;
        [SerializeField] private SerializableCallback<int, float> _parameterCallback;
        [SerializeField] private ParamCallback _parameterCallbackInherit;
        [SerializeField] private SerializableCallback<CancellationToken, string> _nonValueParameter;

        public float FloatFunction()
        {
            return 42.0f;
        }

        public float ParamFunction(int param)
        {
            return param;
        }

        public string NonValueFunction(CancellationToken ct)
        {
            return $"ct: {ct.ToString()}";
        }

        [ContextMenu("Test")]
        public void DebugFunctions()
        {
            Debug.Log($"_floatCallback: {_floatCallback.Invoke()}");
            Debug.Log($"_parameterCallback: {_parameterCallback.Invoke(5)}");
            Debug.Log($"_parameterCallbackInherit: {_parameterCallbackInherit.Invoke(5)}");
            Debug.Log($"_nonValueParameter: {_nonValueParameter.Invoke(default)}");
        }
    }
}
