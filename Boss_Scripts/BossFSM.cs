using UnityEngine;
using BasicEnemy.Enemy.Core;

namespace BasicEnemy.Enemy.Wendigo_FolkFall
{
    public class BossFSM : FiniteStateMachine, IBossContext
    {
        [Header("Boss Logic Settings")]
        public float meleeTriggerDistance = 1.5f;

        [HideInInspector] public BossAnimator bossAnimator;
        [HideInInspector] public Transform playerTransform;

        private Animator animator;
        private bool isDead = false;
        private bool isStopped = false;

        public Transform PlayerTransform => playerTransform;
        public Transform BossTransform => transform;

        private void Awake()
        {
            bossAnimator = GetComponent<BossAnimator>();
            animator = GetComponent<Animator>();

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;
        }

        private void Start()
        {
            CurrentState = new BossIdleState(this);
        }

        protected override void Update()
        {
            if (isDead) return;

            if (playerTransform == null)
            {
                if (!(CurrentState is BossIdleState))
                    CurrentState = new BossIdleState(this);

                base.Update();
                return;
            }

            base.Update();
        }

        public interface IAnimationEventHandler
        {
            void OnAttackAnimationEnd();
            void OnDeathAnimationEnd();
            void OnRoarAnimationEnd();
            void OnActionSequenceEnd();
        }

        public void OnAttackAnimationEnd() => (CurrentState as IAnimationEventHandler)?.OnAttackAnimationEnd();
        public void OnDeathAnimationEnd() => (CurrentState as IAnimationEventHandler)?.OnDeathAnimationEnd();
        public void OnRoarAnimationEnd() => (CurrentState as IAnimationEventHandler)?.OnRoarAnimationEnd();
        public void OnActionSequenceEnd() => (CurrentState as IAnimationEventHandler)?.OnActionSequenceEnd();

        public void DieLogic()
        {
            if (isDead) return;
            isDead = true;

            if (animator != null) animator.speed = 1f;

            NextState = new BossDieState(this);
            CurrentState.StateStage = StateEvent.EXIT;
        }

        public void RotateToPlayerSmoothly(float speed = 5f)
        {
            if (playerTransform == null) return;
            Vector3 direction = playerTransform.position - transform.position;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);
            }
        }

        public void LookAtPlayerImmediate()
        {
            if (playerTransform == null) return;
            Vector3 direction = playerTransform.position - transform.position;
            direction.y = 0;
            transform.forward = direction.normalized;
        }

        public void StopMovement() => isStopped = true;
        public void ResumeMovement() => isStopped = false;
    }
}