using UnityEngine;

namespace SerializableCallback.Behaviours
{
    public class ConditionalEvent : MonoBehaviour
    {
        [SerializeField] private SerializableValueCallback<bool> _condition;
        [SerializeField] private bool _negate;
        [SerializeField] private SerializableEvent _event;

        public void Invoke()
        {
            if (_negate ^ _condition.Value)
            {
                _event.Invoke();
            }
        }
    }
}