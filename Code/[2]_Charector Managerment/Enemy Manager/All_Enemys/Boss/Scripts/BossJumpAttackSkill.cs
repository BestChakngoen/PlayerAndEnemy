using UnityEngine;
using System.Collections;
using BasicEnemy;

namespace BasicEnemy.Enemy.Wendigo_FolkFall
{
    [System.Serializable]
    public class BossJumpAttackSkill
    {
        [Header("Jump Attack Settings")]
        public float jumpAttackCooldown = 30f;
        public float damageRadius = 3f;
        public int jumpDamage = 50;
        public float fixedJumpTime = 0.5f;
        public LayerMask targetLayer;

        [Header("References")]
        public JumpAttackWarning warningIndicator;
        public GameObject landingVFXTemplate;

        private float currentCDReduction = 0f;
        private float lastJumpAttackTime = -30f;
        private bool isJumping = false;
        private Vector3 lockedTargetPosition;
        private Coroutine jumpMoveCoroutine;

        private BossFSM fsm;
        private BossSkillController controller;

        public bool IsJumping => isJumping;

        public void Initialize(BossFSM fsm, BossSkillController controller)
        {
            this.fsm = fsm;
            this.controller = controller; // เก็บ Controller ไว้เรียกใช้ Coroutine
            
            if (targetLayer.value == 0) targetLayer = LayerMask.GetMask("Player");
            if (landingVFXTemplate != null) landingVFXTemplate.SetActive(false);
        }

        public bool CanUse()
        {
            if (isJumping) return false;
            float effectiveCooldown = jumpAttackCooldown * (1f - currentCDReduction);
            return Time.time - lastJumpAttackTime >= effectiveCooldown;
        }

        public void ApplyCooldownModifier(float reduction)
        {
            currentCDReduction = reduction;
        }

        public void StartSequence()
        {
            if (isJumping) return;
            controller.StartCoroutine(JumpAttackSequence());
        }

        private IEnumerator JumpAttackSequence()
        {
            isJumping = true;
            fsm.StopMovement();
            lastJumpAttackTime = Time.time;

            if (warningIndicator != null)
            {
                warningIndicator.StartTrackingAndWarning(fsm.GetPlayerTransform());
                yield return new WaitForSeconds(warningIndicator.indicatorDuration * 0.95f);
                lockedTargetPosition = warningIndicator.LockTargetPosition();
                yield return new WaitForSeconds(warningIndicator.indicatorDuration * 0.05f);
            }
            else
            {
                lockedTargetPosition = fsm.GetPlayerTransform().position;
                yield return new WaitForSeconds(0.5f);
            }

            if (fsm != null && fsm.gameObject.activeInHierarchy)
            {
                fsm.bossAnimator.TriggerJumpAttack();
            }
            else
            {
                isJumping = false;
                warningIndicator?.StopWarning();
            }
        }

        public void OnAnimationStart()
        {
            warningIndicator?.StopWarning();
            if (jumpMoveCoroutine != null) controller.StopCoroutine(jumpMoveCoroutine);
            jumpMoveCoroutine = controller.StartCoroutine(MoveToTargetCoroutine(lockedTargetPosition));
        }

        public void OnAnimationEnd()
        {
            warningIndicator?.StopWarning();
            if (jumpMoveCoroutine != null)
            {
                controller.StopCoroutine(jumpMoveCoroutine);
                jumpMoveCoroutine = null;
            }

            fsm.transform.position = lockedTargetPosition;
            PerformDamageCheck();
            SpawnLandingVFX();
            isJumping = false;
        }

        private IEnumerator MoveToTargetCoroutine(Vector3 targetPosition)
        {
            Vector3 startPosition = fsm.transform.position;
            float timer = 0f;
            while (timer < fixedJumpTime)
            {
                float t = timer / fixedJumpTime;
                fsm.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                timer += Time.deltaTime;
                yield return null;
            }
            fsm.transform.position = targetPosition;
            jumpMoveCoroutine = null;
        }

        private void PerformDamageCheck()
        {
            Collider[] hits = Physics.OverlapSphere(fsm.transform.position, damageRadius, targetLayer);
            foreach (Collider hit in hits)
            {
                Health targetHealth = hit.GetComponent<Health>();
                if (targetHealth != null) targetHealth.TakeDamage(jumpDamage);
            }
        }

        private void SpawnLandingVFX()
        {
            if (landingVFXTemplate == null) return;
            GameObject vfxInstance = Object.Instantiate(landingVFXTemplate, fsm.transform.position, Quaternion.identity);
            vfxInstance.SetActive(true);
            ParticleSystem ps = vfxInstance.GetComponent<ParticleSystem>();
            Object.Destroy(vfxInstance, ps != null ? ps.main.duration + ps.main.startLifetimeMultiplier : 1f);
        }
    }
}