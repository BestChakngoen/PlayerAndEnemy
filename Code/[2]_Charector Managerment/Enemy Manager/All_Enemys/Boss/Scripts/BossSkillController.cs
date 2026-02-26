using UnityEngine;

namespace BasicEnemy.Enemy.Wendigo_FolkFall
{
    // ตัวนี้คือ MonoBehaviour ตัวเดียวที่คุณต้องนำไปแปะบน Boss
    public class BossSkillController : MonoBehaviour
    {
        // สกิลแต่ละตัวจะถูกแสดงผลเป็นหมวดหมู่ใน Inspector ทันที
        [SerializeField] public BossRoarSkill roarSkill = new BossRoarSkill();
        [SerializeField] public BossJumpAttackSkill jumpAttackSkill = new BossJumpAttackSkill();
        [SerializeField] public BossTeleportSkill teleportSkill = new BossTeleportSkill();

        private BossFSM fsm;

        void Awake()
        {
            fsm = GetComponent<BossFSM>();
            
            // ส่งค่า FSM (this) ไปให้แต่ละสกิลเก็บไว้ใช้งาน
            roarSkill.Initialize(fsm);
            jumpAttackSkill.Initialize(fsm, this);
            teleportSkill.Initialize(fsm);
        }

        // ตัวส่งต่อ Event จาก Animator ไปให้ JumpAttackSkill
        public void OnJumpAttackAnimationStart() => jumpAttackSkill.OnAnimationStart();
        public void OnJumpAttackAnimationEnd() => jumpAttackSkill.OnAnimationEnd();
    }
}