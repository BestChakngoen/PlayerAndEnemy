using UnityEngine;
using PlayerInputs.Core;
using CoreSystem;
using GameManger;

namespace PlayerInputs
{
    public class PlayerCombatController : MonoBehaviour, IPlayerCombat, IActionLockable
    {
        [Header("Combo Settings")]
        [SerializeField] private int maxCombo = 4;
        [SerializeField] private float comboResetTime = 0.8f;
        [SerializeField] private PlayerAnimationFacade anim;
        
        [Header("Damage Settings")]
        [SerializeField] private float baseDamage = 15f;
        [SerializeField] private float[] comboDamageMultipliers = { 1.0f, 1.2f, 1.5f, 2.0f };
        [SerializeField] private ConeOverlapAttacker coneAttacker;

        [Header("Auto Aim Settings")]
        [SerializeField] private bool enableAutoAim = true;
        [SerializeField] private float autoAimRadius = 6f;
        [Range(0f, 360f)]
        [SerializeField] private float autoAimMaxAngle = 360f;
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField] private Transform playerRoot;

        [Header("Audio")]
        [SerializeField] private AudioClip[] attackSounds;

        private int currentCombo;
        private bool isAttacking;
        private bool isRolling;
        private float comboResetTimer;
        private bool canMove = true;
        private PlayerStateController stateController;
        private bool isActionLocked = false;
        
        public bool CanMove => canMove;

        private void Awake()
        {
            if (playerRoot == null) playerRoot = transform.root;
            if (coneAttacker == null) coneAttacker = GetComponent<ConeOverlapAttacker>();
            stateController = GetComponentInParent<PlayerStateController>();

            anim.OnAttackEnd += ResetAttackState;
            anim.OnCanMove += EnableMovement;
            anim.OnCanNotMove += DisableMovement;
            anim.OnRollStart += HandleRollStart;
            anim.OnRollEnd += HandleRollEnd;
            anim.OnDealDamage += HandleDealDamage;
        }

        private void Update()
        {
            if (isAttacking || isActionLocked) return;

            if (comboResetTimer > 0)
            {
                comboResetTimer -= Time.deltaTime;
                if (comboResetTimer <= 0) currentCombo = 0;
            }
        }

        public void LockAction()
        {
            isActionLocked = true;
            isAttacking = false;
            isRolling = false;
            canMove = true;
            currentCombo = 0;
            comboResetTimer = 0f;
        }

        public void UnlockAction()
        {
            isActionLocked = false;
        }

        public void ResetCombatState()
        {
            isAttacking = false;
            isRolling = false;
            isActionLocked = false;
            canMove = true;
            currentCombo = 0;
            comboResetTimer = 0f;
        }

        public void Attack()
        {
            if (isActionLocked || isAttacking || isRolling || (stateController != null && !stateController.CanControl)) return;

            if (enableAutoAim)
            {
                PerformAutoAim();
            }

            canMove = false;
            isAttacking = true;
            currentCombo = (currentCombo % maxCombo) + 1;
            
            if (attackSounds != null && attackSounds.Length > 0 && AudioManager.Instance != null)
            {
                AudioClip clip = attackSounds[Random.Range(0, attackSounds.Length)];
                AudioManager.Instance.PlaySFX(clip, transform.position);
            }

            anim.PlayAttack(currentCombo);
        }

        private void HandleDealDamage()
        {
            if (coneAttacker != null)
            {
                float multiplier = 1.0f;
                int comboIndex = currentCombo - 1;
                if (comboIndex >= 0 && comboIndex < comboDamageMultipliers.Length)
                {
                    multiplier = comboDamageMultipliers[comboIndex];
                }

                coneAttacker.Attack(baseDamage * multiplier);
            }
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

        private void OnDestroy()
        {
            if (anim != null)
            {
                anim.OnAttackEnd -= ResetAttackState;
                anim.OnCanMove -= EnableMovement;
                anim.OnCanNotMove -= DisableMovement;
                anim.OnRollStart -= HandleRollStart;
                anim.OnRollEnd -= HandleRollEnd;
                anim.OnDealDamage -= HandleDealDamage;
            }
        }
    }
}