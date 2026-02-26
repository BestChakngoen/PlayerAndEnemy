using UnityEngine;

namespace BasicEnemy
{
    public class PoisonDebuff : Buff
    {
        private float damagePerSecond;
        private float tickTimer;

        public PoisonDebuff(float duration, float dps) : base(duration)
        {
            this.damagePerSecond = dps;
            this.tickTimer = 0f;
        }

        public override void OnTick(float deltaTime)
        {
            base.OnTick(deltaTime); 

            tickTimer += deltaTime;
            if (tickTimer >= 1.0f) 
            {
                tickTimer -= 1.0f;
                if (targetHealth != null)
                {
                    targetHealth.TakeDamage(damagePerSecond);
                }
            }
        }
    }

}