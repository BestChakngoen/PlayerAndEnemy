using UnityEngine;

namespace BasicEnemy
{
    public class DeathHealthBarDisabler : MonoBehaviour
    {
        [Header("Cleanup Listener")]
        public HealthBarCleanupFinishedSO OnCleanupFinishedChannel; 
        
        [SerializeField] 
        private GameObject healthBarRoot;
        private InstanceIdentity identity;
        private int myInstanceID;

        void Awake()
        {
            identity = GetComponentInParent<InstanceIdentity>(); 
            if (identity != null) 
            {
                myInstanceID = identity.GetID();
            }
            else
            {
                GameObject root = transform.root.gameObject;
                myInstanceID = root.GetInstanceID();
                Debug.LogError($"[HealthBarDisabler] InstanceIdentity missing. Using Root ID: {myInstanceID}");
            }

            if (healthBarRoot == null)
            {
                healthBarRoot = gameObject;
            }
        }

        void OnEnable()
        {
            if (OnCleanupFinishedChannel != null)
            {
                OnCleanupFinishedChannel.OnEventRaised.AddListener(OnCleanupFinished);
            }
        }

        void OnDisable()
        {
            if (OnCleanupFinishedChannel != null)
            {
                OnCleanupFinishedChannel.OnEventRaised.RemoveListener(OnCleanupFinished);
            }
        }

        private void OnCleanupFinished(int eventInstanceID)
        {
            if (eventInstanceID == myInstanceID)
            {
                DisableHealthBar(); 
            }
        }

        private void DisableHealthBar()
        {
            if (healthBarRoot != null)
            {
                healthBarRoot.SetActive(false);
            }
        }
    }
}