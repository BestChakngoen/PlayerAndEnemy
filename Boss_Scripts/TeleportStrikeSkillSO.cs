using BasicEnemy;
using UnityEngine;

namespace Boss.scripts
{
    [CreateAssetMenu(fileName = "TeleportStrikeSkill", menuName = "Boss Skills/Teleport Strike")]
    public class TeleportStrikeSkillSO : BossSkillSO
    {
        public float minTriggerDistance = 5f;
        public float minTeleportAwayDistance = 5f;
        public float maxTeleportAwayDistance = 12f;

        public override bool CanExecute(BossFSM fsm)
        {
            if (fsm.playerTransform == null) return false;
            
            float distance = Vector3.Distance(fsm.BossTransform.position, fsm.playerTransform.position);
            return distance >= minTriggerDistance;
        }

        public override void Execute(BossFSM fsm)
        {
            fsm.NextState = new BossTeleportSwipeState(fsm, minTeleportAwayDistance, maxTeleportAwayDistance);
            fsm.CurrentState.StateStage = StateEvent.EXIT;
        }
    }
}