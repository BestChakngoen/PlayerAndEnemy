using System.Collections.Generic;
using UnityEngine;

namespace Boss.core
{
    public interface IEffectable
    {
        void AddEffect(EffectSO effect);
    }

    public interface ISpeedModifiable
    {
        void MultiplySpeed(float multiplier);
        void DivideSpeed(float multiplier);
    }

    [System.Serializable]
    public class ActiveEffect
    {
        public EffectSO effect;
        public float timer;
    }

    public class StatusEffectHandler : MonoBehaviour, IEffectable
    {
        [SerializeField] private List<ActiveEffect> activeEffects = new List<ActiveEffect>();

        public void AddEffect(EffectSO effect)
        {
            if (effect == null) return;

            var existing = activeEffects.Find(e => e.effect == effect);
            if (existing != null)
            {
                existing.timer = effect.duration; 
            }
            else
            {
                ActiveEffect newEffect = new ActiveEffect { effect = effect, timer = effect.duration };
                activeEffects.Add(newEffect);
                effect.ApplyEffect(gameObject);
            }
        }

        private void Update()
        {
            for (int i = activeEffects.Count - 1; i >= 0; i--)
            {
                activeEffects[i].timer -= Time.deltaTime;
                if (activeEffects[i].timer <= 0)
                {
                    activeEffects[i].effect.RemoveEffect(gameObject);
                    activeEffects.RemoveAt(i);
                }
            }
        }
    }
}