
using UnityEngine;

namespace BasicEnemy
{
    public class GlobalDamageTextSpawner : MonoBehaviour
    {
        [Header("Listen to")] public DamageTakenEventSO DamageTakenEvent;

        void OnEnable()
        {
            if (DamageTakenEvent != null)
            {
                DamageTakenEvent.OnEventRaised += SpawnDamageText;
            }
        }

        void OnDisable()
        {
            if (DamageTakenEvent != null)
            {
                DamageTakenEvent.OnEventRaised -= SpawnDamageText;
            }
        }

        private void SpawnDamageText(float damageAmount, Vector3 spawnPosition)
        {
            if (GameManager.Instance != null && GameManager.Instance.damageTextPrefab != null)
            {
                GameObject damageTextInstance = Instantiate(GameManager.Instance.damageTextPrefab, spawnPosition,
                    Quaternion.identity);
                DamageTextManager manager = damageTextInstance.GetComponent<DamageTextManager>();
                if (manager != null)
                {
                    manager.SetDamageText(damageAmount);
                }
            }
        }
    }
}