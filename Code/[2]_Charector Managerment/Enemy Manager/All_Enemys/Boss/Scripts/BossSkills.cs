using UnityEngine;
using System.Collections;
using BasicEnemy;

namespace BasicEnemy.Enemy.Wendigo_FolkFall
{
    public class BossSkills : MonoBehaviour
    {
        [Header("Dependencies")]
        public BossFSM bossAI;
        public BossAnimator bossAnimator;
        public JumpAttackWarning warningIndicator;

        [Header("Teleport Settings")]
        public float teleportOffsetDistance = 2.0f; // ระยะที่วาปไปโผล่หลังผู้เล่น
        public GameObject reappearVFX;
        public AudioClip reappearWarningSFX;

        // ... (ตัวแปรเดิม Jump Attack, Roar คงไว้เหมือนเดิม) ...
        [Header("Jump Attack Settings")]
        public float jumpAttackCooldown = 30f;
        public float damageRadius = 3f;
        public int jumpDamage = 50;
        public float fixedJumpTime = 0.5f;
        private float currentCDReduction = 0f;
        public LayerMask targetLayer;

        [Header("Roar Attack")]
        public float roarRange = 2.0f;
        public float roarDamage = 10f;
        public float roarHitInterval = 0.5f;
        public float roarKnockbackForce = 5f;
        public float roarCooldown = 10f;
        private float lastRoarTime = -10f;
        public GameObject roarVFX;
        public AudioClip roarSFX;
        public VulnerabilityDebuffData roarDebuffData;

        [Header("VFX & SFX")]
        public GameObject landingVFXTemplate;

        private float lastJumpAttackTime;
        private bool isJumping = false;
        private Vector3 lockedTargetPosition;
        private Coroutine jumpMoveCoroutine;

        void Awake()
        {
            if (bossAI == null) bossAI = GetComponent<BossFSM>();
            if (bossAnimator == null) bossAnimator = GetComponent<BossAnimator>();
            if (targetLayer.value == 0) targetLayer = LayerMask.GetMask("Player");
            if (landingVFXTemplate != null) landingVFXTemplate.SetActive(false);
        }

        // --- ฟังก์ชันใหม่สำหรับ Teleport ---
        public void PerformTeleportBehindPlayer(Transform player)
        {
            if (player == null) return;

            Debug.Log("behaind");
            // คำนวณตำแหน่งด้านหลังผู้เล่น
            Vector3 behindPosition = player.position - (player.forward * teleportOffsetDistance);
            
            // วาปไปตำแหน่งนั้น
            transform.position = behindPosition;
            
            // หันหน้าเข้าหาผู้เล่น
            bossAI.LookAtPlayerImmediate();

            // เล่น Effect & Sound ตอนวาปโผล่มา
            if (reappearVFX != null) Instantiate(reappearVFX, transform.position, transform.rotation);
            if (reappearWarningSFX != null) AudioSource.PlayClipAtPoint(reappearWarningSFX, transform.position);
        }

        // --- โค้ดเดิมทั้งหมดคงไว้ ---
        public bool CanPerformJumpAttack() { return !isJumping && (Time.time - lastJumpAttackTime >= jumpAttackCooldown * (1f - currentCDReduction)); }
        public void ApplyCooldownModifier(float reduction) { currentCDReduction = reduction; }
        public bool CanRoar() { return Time.time - lastRoarTime >= roarCooldown; }
        public void TriggerRoarSequence(Transform player) { lastRoarTime = Time.time; }
        
        public void StartJumpAttackSequence()
        {
            if (isJumping) return;
            StartCoroutine(JumpAttackSequence());
        }

        private IEnumerator JumpAttackSequence()
        {
            isJumping = true;
            bossAI.StopMovement();
            lastJumpAttackTime = Time.time;

            if (warningIndicator != null)
            {
                warningIndicator.StartTrackingAndWarning(bossAI.GetPlayerTransform());
                yield return new WaitForSeconds(warningIndicator.indicatorDuration * 0.95f);
                lockedTargetPosition = warningIndicator.LockTargetPosition();
                yield return new WaitForSeconds(warningIndicator.indicatorDuration * 0.05f);
            }
            else
            {
                lockedTargetPosition = bossAI.GetPlayerTransform().position;
                yield return new WaitForSeconds(0.5f);
            }

            if (bossAI != null && bossAI.gameObject.activeInHierarchy) bossAnimator.TriggerJumpAttack();
            else { isJumping = false; warningIndicator?.StopWarning(); }
        }

        public void OnJumpAttackAnimationStart()
        {
            warningIndicator?.StopWarning();
            if (jumpMoveCoroutine != null) StopCoroutine(jumpMoveCoroutine);
            jumpMoveCoroutine = StartCoroutine(MoveToTargetCoroutine(lockedTargetPosition));
        }

        public void OnJumpAttackAnimationEnd()
        {
            warningIndicator?.StopWarning();
            if (jumpMoveCoroutine != null) { StopCoroutine(jumpMoveCoroutine); jumpMoveCoroutine = null; }
            transform.position = lockedTargetPosition;
            PerformDamageCheck();
            SpawnLandingVFX();
            isJumping = false;
        }

        private IEnumerator MoveToTargetCoroutine(Vector3 targetPosition)
        {
            Vector3 startPosition = transform.position;
            float timer = 0f;
            while (timer < fixedJumpTime)
            {
                float t = timer / fixedJumpTime;
                transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                timer += Time.deltaTime;
                yield return null;
            }
            transform.position = targetPosition;
            jumpMoveCoroutine = null;
        }

        public void PerformRoarHit(Transform player)
        {
            if (player == null || Vector3.Distance(transform.position, player.position) > roarRange * 1.5f) return;
            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null) playerHealth.TakeDamage(roarDamage);
            Rigidbody playerRb = player.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                Vector3 knockbackDir = (player.position - transform.position).normalized;
                knockbackDir.y = 0.2f;
                playerRb.AddForce(knockbackDir * roarKnockbackForce, ForceMode.Impulse);
            }
        }

        private void SpawnLandingVFX()
        {
            if (landingVFXTemplate == null) return;
            GameObject vfxInstance = Instantiate(landingVFXTemplate, transform.position, Quaternion.identity);
            vfxInstance.SetActive(true);
            ParticleSystem ps = vfxInstance.GetComponent<ParticleSystem>();
            Destroy(vfxInstance, ps != null ? ps.main.duration + ps.main.startLifetimeMultiplier : 1f);
        }

        private void PerformDamageCheck()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, damageRadius, targetLayer);
            foreach (Collider hit in hits)
            {
                Health targetHealth = hit.GetComponent<Health>();
                if (targetHealth != null) targetHealth.TakeDamage(jumpDamage);
            }
        }
    }
}