using UnityEngine;

namespace BasicEnemy
{
    [CreateAssetMenu(fileName = "EnrageBuff", menuName = "Buffs/Enrage Buff Data")]
    public class EnrageBuffData : BuffData
    {
        [Header("Enrage Settings")]
        public GameObject auraEffectPrefab;
        
        [Header("Stat Modifiers")]
        public float healthThreshold = 0.5f;
        public float speedMultiplier = 1.4f;
        public float attackSpeedMultiplier = 1.5f;
        public float damageMultiplier = 1.3f;

        [Range(0, 1)]
        public float damageReduction = 0.4f;

        [Range(0, 1)]
        public float skillCDReduction = 0.3f;

        [Header("Regeneration")]
        public float regenPercent = 0.01f;
        public float regenInterval = 2f;


        public override Buff CreateBuff()
        {
            return new EnrageBuff(this); 
        }
    }
}