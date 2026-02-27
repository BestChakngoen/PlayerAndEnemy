using BasicEnemy;
using UnityEngine;

namespace Boss.scripts
{
    [CreateAssetMenu(fileName = "TeleportAwaySkill", menuName = "Boss Skills/Teleport Away")]
    public class TeleportAwaySkillSO : BossSkillSO
    {
        [Tooltip("ระยะที่ Player เข้าใกล้แล้วบอสจะวาร์ปหนี (ควรมากกว่า Melee Trigger Distance เล็กน้อย)")]
        public float triggerDistance = 4.0f; 
        
        [Tooltip("โอกาสการตอบสนองต่อวินาที (ยิ่งเยอะ บอสยิ่งไหวตัวทันและวาร์ปไวเมื่อเข้าใกล้)")]
        public float reactionRate = 3.0f; 

        public float minTeleportDistance = 8f;
        public float maxTeleportDistance = 15f;

        public override bool CanExecute(BossFSM fsm)
        {
            // สกิลนี้จะทำงานก็ต่อเมื่อบอสกำลังถอยหลังอยู่เท่านั้น
            if (!(fsm.CurrentState is BossWalkBackState)) return false;
            if (fsm.playerTransform == null) return false;

            float distance = Vector3.Distance(fsm.BossTransform.position, fsm.playerTransform.position);
            
            // ถ้า Player พยายามวิ่งตามมาในระยะอันตราย
            if (distance <= triggerDistance)
            {
                // ใช้การสุ่มแบบถ่วงเวลา (Time.deltaTime) เพื่อให้บอสไม่ได้วาร์ปแบบหุ่นยนต์เป๊ะๆ 
                // แต่มีจังหวะหน่วงแบบสุ่มเหมือนตระหนักได้ว่าถูกตาม
                if (Random.value < reactionRate * Time.deltaTime)
                {
                    return true;
                }
            }
            return false;
        }

        public override void Execute(BossFSM fsm)
        {
            fsm.NextState = new BossTeleportAwayState(fsm, minTeleportDistance, maxTeleportDistance);
            if (fsm.CurrentState != null)
            {
                fsm.CurrentState.StateStage = StateEvent.EXIT;
            }
        }
    }
}