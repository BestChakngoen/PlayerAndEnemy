using UnityEngine;
using UnityEngine.Events;

namespace BasicEnemy
{
    [System.Serializable]
    public class HealthBarCleanupFinishedEvent : UnityEvent<int> { } 

    [CreateAssetMenu(fileName = "HealthBarCleanupFinishedEvent", menuName = "Game Events/Health Bar Cleanup Finished Event")]
    public class HealthBarCleanupFinishedSO : ScriptableObject
    {
        public HealthBarCleanupFinishedEvent OnEventRaised = new HealthBarCleanupFinishedEvent();

        public void RaiseEvent(int instanceID)
        {
            OnEventRaised.Invoke(instanceID);
        }
    }
}