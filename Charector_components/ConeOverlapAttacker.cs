using System;
using UnityEngine;

namespace CoreSystem
{
    public class ConeOverlapAttacker : MonoBehaviour
    {
        public float attackRadius = 3f;
        [Range(0f, 360f)]
        public float attackAngle = 90f;
        public LayerMask targetLayer;
        public Transform attackOrigin;
        
        [Header("Debug")]
        public bool showGizmos = true;

        public event Action<GameObject> OnTargetHit;

        private void Awake()
        {
            if (attackOrigin == null) attackOrigin = transform;
        }

        public void Attack(float damageAmount)
        {
            Collider[] hitColliders = Physics.OverlapSphere(attackOrigin.position, attackRadius, targetLayer);

            foreach (var hitCollider in hitColliders)
            {
                Health targetHealth = hitCollider.GetComponentInParent<Health>();
                if (targetHealth != null && targetHealth.GetCurrentHealth() <= 0)
                {
                    continue;
                }

                Vector3 directionToTarget = hitCollider.transform.position - attackOrigin.position;
                directionToTarget.y = 0;

                float angleToTarget = Vector3.Angle(attackOrigin.forward, directionToTarget);

                if (angleToTarget <= attackAngle / 2f)
                {
                    IDamageable damageable = hitCollider.GetComponentInParent<IDamageable>();
                    if (damageable != null)
                    {
                        damageable.TakeDamage(damageAmount);
                        OnTargetHit?.Invoke(hitCollider.gameObject);
                    }
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (!showGizmos) return;

            Transform origin = attackOrigin != null ? attackOrigin : transform;
            
            Gizmos.color = Color.red;
            Vector3 pos = origin.position;
            Vector3 forward = origin.forward;

            Vector3 rightBound = Quaternion.Euler(0, attackAngle / 2f, 0) * forward;
            Vector3 leftBound = Quaternion.Euler(0, -attackAngle / 2f, 0) * forward;

            Gizmos.DrawLine(pos, pos + rightBound * attackRadius);
            Gizmos.DrawLine(pos, pos + leftBound * attackRadius);

            int segments = 20;
            float angleStep = attackAngle / segments;
            Vector3 prevPoint = pos + leftBound * attackRadius;

            for (int i = 1; i <= segments; i++)
            {
                float currentAngle = (-attackAngle / 2f) + (angleStep * i);
                Vector3 currentDir = Quaternion.Euler(0, currentAngle, 0) * forward;
                Vector3 currentPoint = pos + currentDir * attackRadius;

                Gizmos.DrawLine(prevPoint, currentPoint);
                prevPoint = currentPoint;
            }
        }
    }
}