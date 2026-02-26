
using UnityEngine;
using UnityEngine.Events;

namespace BasicEnemy
{
    [CreateAssetMenu(fileName = "HealthChangedEvent", menuName = "Game Events/Health Changed Event")]
    public class HealthChangedEventSO : ScriptableObject
    {
        // Payload: (InstanceID, currentHealth, maxHealth)
        public UnityAction<int, float, float> OnEventRaised;

        public void RaiseEvent(int instanceID, float currentHealth, float maxHealth)
        {
            OnEventRaised?.Invoke(instanceID, currentHealth, maxHealth);
        }
    }
}