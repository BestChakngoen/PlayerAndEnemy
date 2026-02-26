using UnityEngine;
using BasicEnemy;

namespace GameManger
{
    public class HealPickup : MonoBehaviour
    {
        [Header("VFX & SFX")]
        public GameObject healVFXPrefab;
        public LayerMask playerLayer;

        private void OnTriggerEnter(Collider other)
        {
            if (((1 << other.gameObject.layer) & playerLayer) != 0)
            {
                PlayerInventory playerInventory = other.GetComponent<PlayerInventory>();

                if (playerInventory != null)
                {
                    playerInventory.AddPotion();
                    //SpawnPickupVFX(other.transform);
                    Destroy(gameObject);
                }
            }
        }

        private void SpawnPickupVFX(Transform playerTransform)
        {
            if (healVFXPrefab == null) return;
            
            GameObject vfxInstance = Instantiate(
                healVFXPrefab, 
                transform.position,
                Quaternion.identity, 
                playerTransform 
            );
            
            ParticleSystem ps = vfxInstance.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                float duration = ps.main.duration + ps.main.startLifetimeMultiplier;
                Destroy(vfxInstance, duration);
            }
            else
            {
                Destroy(vfxInstance, 3f); 
            }
            
        }
    }
}