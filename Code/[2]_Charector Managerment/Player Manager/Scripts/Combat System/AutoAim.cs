// AutoAim.cs
using UnityEngine;
using System.Collections.Generic; 
using System.Linq;
using BasicEnemy;

namespace PlayerInputs
{
    public class AutoAim : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float autoAimRange = 10f;
        [SerializeField] private float autoAimSpeed = 5f;
        [SerializeField] private LayerMask enemyLayer; 

        private Transform targetEnemy;
        private CharacterTag ownerTag; 

        private void Awake()
        {
            ownerTag = GetComponentInParent<CharacterTag>();
            if (ownerTag == null)
            {
                Debug.LogError("AutoAim component requires a CharacterTag component on the same GameObject or a parent.");
            }
            
            if (enemyLayer.value == 0)
            {
                enemyLayer = LayerMask.GetMask("Enemy"); 
            }
        }
        public void FindAndSetTarget()
        {
            targetEnemy = FindClosestEnemy();
        }

        private Transform FindClosestEnemy()
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, autoAimRange, enemyLayer);

            Transform closestEnemy = null;
            float shortestDistance = Mathf.Infinity;

            foreach (Collider hitCollider in hitColliders)
            {
                CharacterTag targetTag = hitCollider.GetComponentInParent<CharacterTag>();
                
                if (targetTag != null && targetTag.characterTag != ownerTag.characterTag)
                {
                    float distanceToEnemy = Vector3.Distance(transform.position, hitCollider.transform.position);

                    if (distanceToEnemy < shortestDistance)
                    {
                        shortestDistance = distanceToEnemy;
                        closestEnemy = hitCollider.transform;
                    }
                }
            }
            
            return closestEnemy;
        }

        public void ClearTarget()
        {
            targetEnemy = null;
        }
        public Transform GetTarget()
        {
            return targetEnemy;
        }

        public float GetAutoAimSpeed()
        {
            return autoAimSpeed;
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, autoAimRange);
        }
    }
}