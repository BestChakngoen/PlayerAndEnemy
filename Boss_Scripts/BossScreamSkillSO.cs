using BasicEnemy;
using UnityEngine;
using Boss.core;
using CCSystem;

namespace Boss.scripts
{
    [CreateAssetMenu(fileName = "BossScreamSkill", menuName = "Boss Skills/Scream Skill")]
    public class BossScreamSkillSO : BossSkillSO
    {
        public float effectRange = 10f;
        public CCEffectSO stunEffect;

        public override bool CanExecute(BossFSM fsm)
        {
            if (!(fsm.CurrentState is BossIdleState)) return false;
            if (fsm.playerTransform == null) return false;
            
            float distance = Vector3.Distance(fsm.BossTransform.position, fsm.playerTransform.position);
            return distance <= effectRange;
        }

        public override void Execute(BossFSM fsm)
        {
            fsm.NextState = new BossScreamState(fsm, this);
            fsm.CurrentState.StateStage = StateEvent.EXIT;
        }
    }
}