using UnityEngine;

namespace BasicEnemy
{
    [System.Serializable]
    public abstract class Buff
    {
        public float Duration;
        public bool IsFinished { get; protected set; }
        
        protected GameObject target;
        protected Health targetHealth;
        protected BuffManager buffManager;

        public Buff(float duration)
        {
            this.Duration = duration;
            this.IsFinished = false;
        }
        public virtual void OnApply(GameObject target)
        {
            this.target = target;
            this.targetHealth = target.GetComponent<Health>();
            this.buffManager = target.GetComponent<BuffManager>();
            
            if (Duration <= 0)
            {
                IsFinished = true;
            }
        }
        public virtual void OnTick(float deltaTime)
        {
            Duration -= deltaTime;
            if (Duration <= 0)
            {
                IsFinished = true;
            }
        }
        public virtual void OnExpire()
        {
            IsFinished = true;
        }
        
        public virtual float ModifyIncomingDamage(float damage)
        {
            return damage;
        }
    }
}