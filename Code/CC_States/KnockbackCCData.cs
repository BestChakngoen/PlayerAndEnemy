using UnityEngine;

namespace BasicEnemy
{
    [CreateAssetMenu(fileName = "KnockbackCCData", menuName = "CC_States/CC Data/Knockback")]
    public class KnockbackCCData : CC_Data
    {
        public KnockbackCCData() { stateType = CCStateType.Knockback; }

        [Header("Knockback Parameters")]
        public float baseMagnitude = 50f; // ขนาดแรงผลักพื้นฐาน
        public float dragCoefficient = 10f; // ค่า Drag เฉพาะสำหรับสถานะนี้
        [Tooltip("เส้นโค้งการลดแรงตามเวลา (ถ้าไม่มีจะใช้ Drag Coefficient)")]
        public AnimationCurve forceFalloffCurve; 
    }
}