using UnityEngine;
using GameManger;
using System.Collections;

namespace BasicEnemy
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(EnemyCC_Controller))] 
    public class BasicEnemyAI : MonoBehaviour
    {
        public enum EnemyState { Chasing, Observing, ClosingIn, Attacking, Retreating }
        
        [Header("State Control")]
        public EnemyState currentState = EnemyState.Chasing;
        
        [Header("Dependencies")] 
        public Weapon enemyWeapon;
        public EnemyAnimator enemyAnimator; 

        [Header("Movement")] 
        public float moveSpeed = 3.5f; 
        public float runSpeed = 6.0f; 
        public float stoppingDistance = 1.5f;
        public float gravity = -9.81f; 
        public float turnSpeed = 5.0f; 

        [Header("Observation Settings")]
        public float observationRange = 6f; 
        
        [Tooltip("ระยะห่างต่ำสุดที่จะรักษาไว้ (ถ้าใกล้กว่านี้จะวิ่งหนี)")]
        public float minStrafeDist = 4f; 
        [Tooltip("ระยะห่างสูงสุดที่จะรักษาไว้")]
        public float maxStrafeDist = 7f;

        [Range(10f, 90f)] public float minStrafeAngle = 45f; 
        [Range(45f, 135f)] public float maxStrafeAngle = 100f;

        public float minObserveTime = 3f;
        public float maxObserveTime = 5f;
        private float observationTimer;
        private Vector3 strafeTarget;
        
        [Header("Combat")] 
        public float attackRange = 1.8f;
        public float retreatDuration = 1.0f; 
        
        [Tooltip("ระยะทางสูงสุดที่จะไล่กวด Player ในสถานะ ClosingIn ก่อนจะยอมแพ้แล้วกลับไป Observe")]
        public float maxChaseDistance = 40f; // ปรับแต่งได้ตามโจทย์
        private Vector3 closingInStartPos; // เก็บจุดเริ่มไล่
        
        private bool isAttacking = false;

        private Transform playerTransform;
        private CharacterController characterController; 
        public bool isDead = false;
        private Animator animator;
        private EnemyCC_Controller ccController; 
        
        private Vector3 currentVelocity; 

        void Awake()
        {
            characterController = GetComponent<CharacterController>(); 
            ccController = GetComponent<EnemyCC_Controller>(); 
            animator = GetComponent<Animator>();
            if (enemyAnimator == null) enemyAnimator = GetComponent<EnemyAnimator>();

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;

            if (animator != null)
            {
                animator.applyRootMotion = false; 
            }
        }

        void Update()
        {
            if (playerTransform == null || isDead || !enabled) return;

            ApplyGravity();

            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

            switch (currentState)
            {
                case EnemyState.Chasing:
                    HandleChasing(distanceToPlayer);
                    break;
                case EnemyState.Observing:
                    HandleObservation(distanceToPlayer);
                    break;
                case EnemyState.ClosingIn:
                    HandleClosingIn(distanceToPlayer);
                    break;
                case EnemyState.Attacking:
                    MoveTo(Vector3.zero, 0); 
                    enemyAnimator.SetMovementSpeed(0);
                    RotateTowards(playerTransform.position); 
                    break;
                case EnemyState.Retreating:
                    break;
            }
        }

        private void ApplyGravity()
        {
            if (characterController.isGrounded && currentVelocity.y < 0)
            {
                currentVelocity.y = -2f; 
            }
            currentVelocity.y += gravity * Time.deltaTime;
            characterController.Move(currentVelocity * Time.deltaTime);
        }

        private void MoveTo(Vector3 targetPos, float speed)
        {
            if (speed <= 0) return;

            Vector3 direction = (targetPos - transform.position).normalized;
            direction.y = 0; 
            characterController.Move(direction * speed * Time.deltaTime);
        }

        private void RotateTowards(Vector3 targetPosition)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                Quaternion lookRot = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * turnSpeed);
            }
        }

        private void HandleChasing(float distanceToPlayer)
        {
            if (distanceToPlayer < attackRange) 
            {
                 Vector3 directionAway = (transform.position - playerTransform.position).normalized;
                 Vector3 retreatPos = transform.position + directionAway * 2.0f;
                 MoveTo(retreatPos, moveSpeed); 
                 RotateTowards(playerTransform.position); 
                return;
            }

            if (distanceToPlayer <= observationRange)
            {
                StartObserving();
                return;
            }

            MoveTo(playerTransform.position, runSpeed);
            enemyAnimator.SetMovementSpeed(runSpeed);
            RotateTowards(playerTransform.position); 
        }

        private void StartObserving()
        {
            currentState = EnemyState.Observing;
            observationTimer = Random.Range(minObserveTime, maxObserveTime);
            SetNewStrafeTarget();
        }

        private void HandleObservation(float distanceToPlayer)
        {
            observationTimer -= Time.deltaTime;

            if (observationTimer <= 0)
            {
                // เปลี่ยนสถานะเป็น ClosingIn
                currentState = EnemyState.ClosingIn; 
                
                // *** บันทึกตำแหน่งที่เริ่มไล่ล่า ***
                closingInStartPos = transform.position; 
                return;
            }

            // 1. ถ้า Player เข้าใกล้เกินไป -> หันหลังแล้ววิ่งหนี
            if (distanceToPlayer < minStrafeDist)
            {
                Vector3 dirAway = (transform.position - playerTransform.position).normalized;
                Vector3 retreatTarget = transform.position + dirAway * 3.0f; 
                
                if (Physics.Raycast(retreatTarget + Vector3.up * 5f, Vector3.down, out RaycastHit hit, 10f))
                    strafeTarget = hit.point;
                else
                    strafeTarget = retreatTarget;

                MoveTo(strafeTarget, runSpeed);
                enemyAnimator.SetMovementSpeed(runSpeed);
                
                RotateTowards(strafeTarget); 
                return;
            }

            if (Vector3.Distance(strafeTarget, playerTransform.position) < minStrafeDist)
            {
                SetNewStrafeTarget();
            }

            // 2. การเดิน Strafe ปกติ
            Vector3 flatPos = transform.position; flatPos.y = 0;
            Vector3 flatTarget = strafeTarget; flatTarget.y = 0;

            if (Vector3.Distance(flatPos, flatTarget) > 0.5f)
            {
                MoveTo(strafeTarget, moveSpeed);
                enemyAnimator.SetMovementSpeed(moveSpeed);
                RotateTowards(strafeTarget); 
            }
            else
            {
                enemyAnimator.SetMovementSpeed(0);
                RotateTowards(playerTransform.position); 
                SetNewStrafeTarget();
            }
        }

        private void HandleClosingIn(float distanceToPlayer)
        {
            // *** ตรวจสอบระยะทางที่วิ่งมาแล้ว ***
            float distCovered = Vector3.Distance(transform.position, closingInStartPos);
            
            // ถ้าวิ่งไล่มาเกินระยะที่กำหนดแล้วยังไม่ถึงตัว ให้กลับไปสังเกตการณ์
            if (distCovered > maxChaseDistance)
            {
                StartObserving();
                return;
            }

            // Logic เดิม: ถ้าถึงระยะโจมตีก็ตี
            if (distanceToPlayer <= attackRange)
            {
                currentState = EnemyState.Attacking;
                AttackPlayer();
                return;
            }

            // Logic เดิม: วิ่งเข้าหา
            MoveTo(playerTransform.position, runSpeed);
            //enemyAnimator.SetMovementSpeed(runSpeed);
            enemyAnimator.SetChargeSpeed(runSpeed);
            RotateTowards(playerTransform.position);
        }

        private void SetNewStrafeTarget()
        {
            Vector3 dirFromPlayer = (transform.position - playerTransform.position).normalized;
            if (dirFromPlayer == Vector3.zero) dirFromPlayer = transform.forward;

            float randomAngle = Random.Range(minStrafeAngle, maxStrafeAngle);
            if (Random.value > 0.5f) randomAngle = -randomAngle; 

            Quaternion rotation = Quaternion.AngleAxis(randomAngle, Vector3.up);
            Vector3 targetDir = rotation * dirFromPlayer;

            float randomDist = Random.Range(minStrafeDist, maxStrafeDist);
            Vector3 potentialTarget = playerTransform.position + (targetDir * randomDist);

            if (Physics.Raycast(potentialTarget + Vector3.up * 10f, Vector3.down, out RaycastHit hit, 20f))
            {
                strafeTarget = hit.point;
            }
            else
            {
                strafeTarget = transform.position; 
            }
        }

        private void AttackPlayer()
        {
            if (isAttacking) return;
            isAttacking = true;

            enemyAnimator.SetMovementSpeed(0);
            enemyAnimator.SetChargeSpeed(0);
            RotateTowards(playerTransform.position);
            enemyAnimator.TriggerAttack();
        }
        
        public void OnAttackAnimationEnd()
        {
            isAttacking = false;
            StartCoroutine(RetreatRoutine());
        }

        private IEnumerator RetreatRoutine()
        {
            currentState = EnemyState.Retreating;
            
            float elapsed = 0;
            while (elapsed < retreatDuration)
            {
                Vector3 dirAway = (transform.position - playerTransform.position).normalized;
                dirAway.y = 0;

                characterController.Move(dirAway * runSpeed * Time.deltaTime);
                enemyAnimator.SetMovementSpeed(runSpeed);

                Quaternion runRot = Quaternion.LookRotation(dirAway);
                transform.rotation = Quaternion.Slerp(transform.rotation, runRot, Time.deltaTime * 10f);

                elapsed += Time.deltaTime;
                yield return null;
            }
            
            enemyAnimator.SetMovementSpeed(0);
            yield return new WaitForSeconds(0.2f); 
            currentState = EnemyState.Chasing; 
        }

        public void DieLogic()
        {
            if (isDead) return;
            isDead = true;
            if (ccController != null) ccController.enabled = false; 
            enemyAnimator.TriggerDie();
            StopAllCoroutines(); 
            characterController.enabled = false; 
        }

        public void OnDeathAnimationEnd()
        {
            if (WaveManager.Instance != null) WaveManager.Instance.MonsterDied();
            if (UIManager.Instance != null) UIManager.Instance.AddKill();
            Destroy(gameObject);
        }

        public void EnableWeaponCollider() { if (enemyWeapon != null) enemyWeapon.EnableCollider(); }
        public void DisableWeaponCollider() { if (enemyWeapon != null) enemyWeapon.DisableCollider(); }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            DrawWireCircle(transform.position, observationRange); 
            Gizmos.color = Color.red;
            DrawWireCircle(transform.position, attackRange); 

            if (playerTransform != null)
            {
                Gizmos.color = Color.green;
                DrawWireCircle(playerTransform.position, minStrafeDist);
                Gizmos.color = Color.blue;
                DrawWireCircle(playerTransform.position, maxStrafeDist);
            }

            if (currentState == EnemyState.Observing)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawSphere(strafeTarget, 0.3f);
                Gizmos.DrawLine(transform.position, strafeTarget);
            }

            // Debug ระยะ Closing In ถ้ากำลังไล่อยู่
            if (currentState == EnemyState.ClosingIn)
            {
                Gizmos.color = new Color(1, 0.5f, 0, 0.5f); // สีส้ม
                Gizmos.DrawLine(closingInStartPos, transform.position);
                Gizmos.DrawWireSphere(closingInStartPos, 0.5f);
            }
        }

        private void DrawWireCircle(Vector3 center, float radius)
        {
            float angle = 0f;
            Vector3 lastPoint = center + new Vector3(Mathf.Cos(0) * radius, 0, Mathf.Sin(0) * radius);
            for (int i = 1; i <= 32; i++)
            {
                angle += (2f * Mathf.PI) / 32f;
                Vector3 nextPoint = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
                Gizmos.DrawLine(lastPoint, nextPoint);
                lastPoint = nextPoint;
            }
        }
#endif
    }
}