using UnityEngine;
using UnityEngine.AI;

namespace BasicEnemy
{
    public class DeathColliderDisabler : MonoBehaviour
    {
        [Header("Dependencies")]
        public HealthChangedEventSO HealthUpdateIDChannel;
        private InstanceIdentity identity;
        private Collider mainCollider;
        private NavMeshAgent navMeshAgent;
        private int myInstanceID;
        void Awake()
        {
            identity = GetComponent<InstanceIdentity>();
            if (identity != null) myInstanceID = identity.GetID();
            
            mainCollider = GetComponent<Collider>();
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        void OnEnable()
        {
            if (HealthUpdateIDChannel != null)
            {
                HealthUpdateIDChannel.OnEventRaised += OnGlobalHealthChanged;
            }
        }

        void OnDisable()
        {
            if (HealthUpdateIDChannel != null)
            {
                HealthUpdateIDChannel.OnEventRaised -= OnGlobalHealthChanged;
            }
        }

        private void OnGlobalHealthChanged(int eventInstanceID, float currentHealth, float maxHealth)
        {
            if (eventInstanceID == myInstanceID && currentHealth <= 0)
            {
                DisableColliderAndPhysics();
            }
        }

        private void DisableColliderAndPhysics()
        {
            if (navMeshAgent != null)
            {
                navMeshAgent.enabled = false; 
            }
            if (mainCollider != null)
            {
                mainCollider.enabled = false;
            }
            
        
        }
    }
}