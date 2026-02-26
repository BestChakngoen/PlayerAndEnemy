using BasicEnemy;
using UnityEngine;

namespace PlayerSkills
{
    public class AreaCCAbility : AbilityComponent
    {
        [Tooltip("Layer Mask ของศัตรู")]
        public LayerMask targetLayer;

        public override void Activate()
        {
            // ใช้ Physics.OverlapSphere เพื่อหาศัตรูทั้งหมดในรัศมี
            Collider[] hits = Physics.OverlapSphere(transform.position, range, targetLayer);

            foreach (var hit in hits)
            {
                // 1. พยายามดึง CC_Manager จากเป้าหมาย
                CC_Manager targetManager = hit.GetComponent<CC_Manager>();
            
                if (targetManager != null && targetManager != CasterManager) // ตรวจสอบไม่ให้ CC ใส่ตัวเอง
                {
                    // 2. คำนวณทิศทางผลัก (จาก Caster ไป Target)
                    Vector3 direction = (hit.transform.position - transform.position).normalized;

                    // 3. เรียก API ของ Manager พร้อมส่ง CCData ของสกิลนี้
                    targetManager.ApplyCC(ccData, direction);
                }
            }
        
            //Debug.Log($"Used {ccData.stateType} Skill. Targets hit: {hits.Length}");
        }
    }
}