using UnityEngine;

namespace BasicEnemy
{
    public class InstantHealBuff : Buff
    {
        private float healAmount;

        public InstantHealBuff(float amount) : base(0) 
        {
            this.healAmount = amount;
        }

        public override void OnApply(GameObject target)
        {
            base.OnApply(target); 
        
            if (targetHealth != null)
            {
                targetHealth.Heal(healAmount);
            }
        }
    }
}