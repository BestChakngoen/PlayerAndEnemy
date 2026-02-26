using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BasicEnemy
{
    [RequireComponent(typeof(BasicEnemyAI))]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(KnockbackState), typeof(AirborneState), typeof(StunState))]
    public class EnemyCC_Controller : CC_Manager
    {
        [Header("[CC Receiver Settings]")]
        [Tooltip("ค่า Gravity ที่ใช้ในการคำนวณ Airborne")]
        public float ccGravity = 20f;

        private CharacterStateComponent _activeState;
        public override float gravity => ccGravity;
        public override CharacterStateComponent ActiveState => _activeState;

        private BasicEnemyAI _ai;
        private CharacterController _cc;

        private Dictionary<System.Type, CharacterStateComponent> _states = new();
        private Coroutine _durationCoroutine;
        private Vector3 _externalForce;

        void Awake()
        {
            _ai = GetComponent<BasicEnemyAI>();
            _cc = GetComponent<CharacterController>();

            foreach (var state in GetComponents<CharacterStateComponent>())
            {
                state.Initialize(this);

                if (!_states.TryAdd(state.GetType(), state))
                {
                    Debug.LogWarning(
                        $"[Controller:{gameObject.name}] Duplicate state {state.GetType().Name}"
                    );
                }
            }
        }

        void Update()
        {
            if (!enabled) return;

            HandleCCStates();
            HandleMovement();
        }

        private void HandleCCStates()
        {
            if (ActiveState == null) return;

            _externalForce = ActiveState.CalculateForce(Time.deltaTime);

            if (ActiveState is AirborneState && _cc.isGrounded)
            {
                // EndState(ActiveState); // เปิดใช้ได้ถ้าต้องการ auto land
            }
        }

        private void HandleMovement()
        {
            if (ActiveState is KnockbackState || ActiveState is AirborneState)
            {
                _cc.Move(_externalForce * Time.deltaTime);
            }
        }

        // ================= CC Interface =================

        public override void EndState(CharacterStateComponent state)
        {
            if (_activeState != state) return;

            _activeState = null;
            _externalForce = Vector3.zero;

            if (_ai != null)
                _ai.enabled = true;

            if (_ai != null && _ai.enemyAnimator != null)
                _ai.enemyAnimator.GetComponent<Animator>().enabled = true;
        }

        public override void ApplyCC(CC_Data data, Vector3 direction)
        {
            if (_ai.isDead) return;

            CharacterStateComponent targetState = null;

            switch (data.stateType)
            {
                case CC_Data.CCStateType.Knockback:
                    _states.TryGetValue(typeof(KnockbackState), out targetState);
                    break;
                case CC_Data.CCStateType.Airborne:
                    _states.TryGetValue(typeof(AirborneState), out targetState);
                    break;
                case CC_Data.CCStateType.Stun:
                    _states.TryGetValue(typeof(StunState), out targetState);
                    break;
            }

            if (targetState == null) return;

            if (ActiveState != null)
                EndState(ActiveState);

            _activeState = targetState;
            _activeState.Enter(data, direction);

            // ❗ หยุด AI แทน NavMesh
            if (_ai != null)
                _ai.enabled = false;

            if (_ai.enemyAnimator != null)
                _ai.enemyAnimator.GetComponent<Animator>().enabled = false;

            if (data.baseDuration > 0)
            {
                if (_durationCoroutine != null)
                    StopCoroutine(_durationCoroutine);

                _durationCoroutine = StartCoroutine(DurationCoroutine(data.baseDuration));
            }
        }

        private IEnumerator DurationCoroutine(float duration)
        {
            yield return new WaitForSeconds(duration);

            if (ActiveState != null)
                EndState(ActiveState);
        }
    }
}
