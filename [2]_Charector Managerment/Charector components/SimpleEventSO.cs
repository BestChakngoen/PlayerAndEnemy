
using UnityEngine;
using UnityEngine.Events;

namespace BasicEnemy
{
    [CreateAssetMenu(fileName = "SimpleEvent", menuName = "Game Events/Simple Event")]
    public class SimpleEventSO : ScriptableObject
    {
        public UnityAction OnEventRaised;

        public void RaiseEvent()
        {
            OnEventRaised?.Invoke();
        }
    }
}