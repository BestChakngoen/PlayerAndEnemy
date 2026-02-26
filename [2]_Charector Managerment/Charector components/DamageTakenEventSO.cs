
using UnityEngine;
using UnityEngine.Events;

namespace BasicEnemy
{
    [CreateAssetMenu(fileName = "DamageTakenEvent", menuName = "Game Events/Damage Taken Event")]
    public class DamageTakenEventSO : ScriptableObject
    {
        public UnityAction<float, Vector3> OnEventRaised;

        public void RaiseEvent(float damageAmount, Vector3 worldPosition)
        {
            OnEventRaised?.Invoke(damageAmount, worldPosition);
        }
    }
}