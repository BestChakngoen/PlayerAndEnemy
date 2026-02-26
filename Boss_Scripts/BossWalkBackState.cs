using UnityEngine;
using BasicEnemy.Enemy.Core;

namespace BasicEnemy.Enemy.Wendigo_FolkFall
{
    public class BossWalkBackState : State
    {
        private BossFSM fsm;
        private float timer;
        private float duration = 1.5f;
        private float speed = 2.5f;

        public BossWalkBackState(BossFSM fsm) : base(fsm)
        {
            this.fsm = fsm;
        }

        public override void Enter()
        {
            timer = duration;
            Animator anim = fsm.GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetFloat("Speed", -1f);
            }
        }

        public override void Update()
        {
            fsm.RotateToPlayerSmoothly(5f);

            Vector3 moveDirection = -fsm.BossTransform.forward;
            fsm.BossTransform.position += moveDirection * speed * Time.deltaTime;

            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                fsm.NextState = new BossScreamState(fsm);
                StateStage = StateEvent.EXIT;
            }
        }
    }
}