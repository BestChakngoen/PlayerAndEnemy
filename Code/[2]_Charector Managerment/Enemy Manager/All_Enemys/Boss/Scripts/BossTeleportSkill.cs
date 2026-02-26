using UnityEngine;

namespace BasicEnemy.Enemy.Wendigo_FolkFall
{
    [System.Serializable]
    public class BossTeleportSkill
    {
        [Header("Teleport Cooldown")]
        public float teleportCooldown = 10f;  // ตั้งค่า Cooldown ได้อิสระที่นี่
        private float lastTeleportTime = 0f;

        [Header("Teleport Settings")]
        public float teleportOffsetDistance = 2.0f;
        public GameObject reappearVFX;
        public AudioClip reappearWarningSFX;

        private BossFSM fsm;

        public void Initialize(BossFSM fsm)
        {
            this.fsm = fsm;
            // ให้เริ่มจับเวลาตั้งแต่เริ่มเกม (เพื่อให้บอสไม่เปิดมาแล้ววาร์ปใส่ทันที)
            lastTeleportTime = Time.time; 
        }

        // ฟังก์ชันสำหรับเช็คว่าคูลดาวน์เสร็จหรือยัง
        public bool CanUse()
        {
            return Time.time - lastTeleportTime >= teleportCooldown;
        }

        // ฟังก์ชันสำหรับรีเซ็ตเวลาคูลดาวน์
        public void TriggerCooldown()
        {
            lastTeleportTime = Time.time;
        }

        public void PerformTeleport(Transform player)
        {
            if (player == null) return;

            Vector3 teleportPos = player.position - (player.forward * teleportOffsetDistance);
            teleportPos.y = fsm.transform.position.y;

            var agent = fsm.GetComponent<UnityEngine.AI.NavMeshAgent>();
            var cc = fsm.GetComponent<CharacterController>();

            if (cc != null) cc.enabled = false;

            if (agent != null && agent.isActiveAndEnabled)
            {
                agent.Warp(teleportPos);
            }
            else
            {
                fsm.transform.position = teleportPos;
            }

            if (cc != null) cc.enabled = true;

            fsm.LookAtPlayerImmediate();

            if (reappearVFX != null)
                Object.Instantiate(reappearVFX, fsm.transform.position, fsm.transform.rotation);
            
            if (reappearWarningSFX != null)
                AudioSource.PlayClipAtPoint(reappearWarningSFX, fsm.transform.position);
        }
    }
}