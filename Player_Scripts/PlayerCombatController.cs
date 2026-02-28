using UnityEngine;
using PlayerInputs.Core;

namespace PlayerInputs
{
    public class PlayerCombatController : MonoBehaviour, IPlayerCombat
    {
        [Header("Combo Settings")]
        [SerializeField] private int maxCombo = 4;
        [SerializeField] private float comboResetTime = 0.8f;
        [SerializeField] private PlayerAnimationFacade anim;
        
        [Header("Auto Aim Settings")]
        [SerializeField] private bool enableAutoAim = true;
        [SerializeField] private float autoAimRadius = 6f;
        [Range(0f, 360f)]
        [SerializeField] private float autoAimMaxAngle = 360f;
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private Transform playerRoot;

        private int currentCombo;
        private bool isAttacking;
        private bool isRolling;
        private float comboResetTimer;
        private bool canMove = true;
        
        public bool CanMove => canMove;

        private void Awake()
        {
            if (playerRoot == null) playerRoot = transform.root;

            anim.OnAttackEnd += ResetAttackState;
            anim.OnCanMove += EnableMovement;
            anim.OnCanNotMove += DisableMovement;
            anim.OnRollStart += HandleRollStart;
            anim.OnRollEnd += HandleRollEnd;
        }

        private void Update()
        {
            if (isAttacking) return;

            if (comboResetTimer > 0)
            {
                comboResetTimer -= Time.deltaTime;
                if (comboResetTimer <= 0) currentCombo = 0;
            }
        }

        public void Attack()
        {
            if (isAttacking || isRolling || !PlayerStateController.CanControl) return;

            if (enableAutoAim)
            {
                PerformAutoAim();
            }

            canMove = false;
            isAttacking = true;
            currentCombo = (currentCombo % maxCombo) + 1;
            anim.PlayAttack(currentCombo);
        }

        private void PerformAutoAim()
        {
            if (playerRoot == null) return;

            Collider[] hitColliders = Physics.OverlapSphere(playerRoot.position, autoAimRadius, enemyLayer);
            
            Transform closestTarget = null;
            float closestDistance = Mathf.Infinity;

            foreach (var hitCollider in hitColliders)
            {
                Transform target = hitCollider.transform;
                Vector3 directionToTarget = target.position - playerRoot.position;
                directionToTarget.y = 0; 

                float distanceToTarget = directionToTarget.magnitude;
                float angleToTarget = Vector3.Angle(playerRoot.forward, directionToTarget);

                if (distanceToTarget < closestDistance && angleToTarget <= autoAimMaxAngle / 2f)
                {
                    closestDistance = distanceToTarget;
                    closestTarget = target;
                }
            }

            if (closestTarget != null)
            {
                Vector3 lookDirection = closestTarget.position - playerRoot.position;
                lookDirection.y = 0; 
                
                if (lookDirection != Vector3.zero)
                {
                    playerRoot.rotation = Quaternion.LookRotation(lookDirection);
                }
            }
        }

        private void ResetAttackState()
        {
            isAttacking = false;
            comboResetTimer = comboResetTime;
        }

        private void HandleRollStart()
        {
            isRolling = true;
            isAttacking = false;
            canMove = true;
            currentCombo = 0;
            comboResetTimer = 0f;
        }

        private void HandleRollEnd()
        {
            isRolling = false;
        }

        private void EnableMovement()
        {
            canMove = true;
        }

        private void DisableMovement()
        {
            canMove = false;
        }

        private void OnDrawGizmosSelected()
        {
            if (!enableAutoAim || playerRoot == null) return;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(playerRoot.position, autoAimRadius);
        }

        private void OnDestroy()
        {
            if (anim != null)
            {
                anim.OnAttackEnd -= ResetAttackState;
                anim.OnCanMove -= EnableMovement;
                anim.OnCanNotMove -= DisableMovement;
                anim.OnRollStart -= HandleRollStart;
                anim.OnRollEnd -= HandleRollEnd;
            }
        }
    }
}