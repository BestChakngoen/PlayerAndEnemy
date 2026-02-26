using UnityEngine;
using BasicEnemy;

namespace BasicEnemy.Enemy.Wendigo_FolkFall
{
    [System.Serializable]
    public class BossRoarSkill
    {
        [Header("Roar Attack")]
        public float roarRange = 2.0f;
        public float roarDamage = 10f;
        public float roarHitInterval = 0.5f;
        public float roarKnockbackForce = 5f;
        public float roarCooldown = 10f;
        public GameObject roarVFX;
        public VulnerabilityDebuffData roarDebuffData;

        private float lastRoarTime = -10f;
        private BossFSM fsm;

        // รับ FSM เข้ามาใช้งานเหมือนตอนทำ State
        public void Initialize(BossFSM fsm)
        {
            this.fsm = fsm;
        }

        public bool CanUse()
        {
            return Time.time - lastRoarTime >= roarCooldown;
        }

        public void TriggerCooldown()
        {
            lastRoarTime = Time.time;
        }

        public void PerformHit(Transform player)
        {
            if (player == null || Vector3.Distance(fsm.transform.position, player.position) > roarRange * 1.5f) return;

            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null) playerHealth.TakeDamage(roarDamage);

            Rigidbody playerRb = player.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                Vector3 knockbackDir = (player.position - fsm.transform.position).normalized;
                knockbackDir.y = 0.2f;
                playerRb.AddForce(knockbackDir * roarKnockbackForce, ForceMode.Impulse);
            }
        }

        public void ApplyDebuff(Transform player)
        {
            if (player == null || roarDebuffData == null) return;
            BuffManager playerBuffManager = player.GetComponent<BuffManager>();
            if (playerBuffManager != null)
            {
                playerBuffManager.AddBuff(roarDebuffData.CreateBuff());
            }
        }
    }
}