using UnityEngine;

namespace Boss.core
{
    public abstract class EffectSO : ScriptableObject
    {
        public string effectName;
        public float duration;
        public bool isBuff;

        public abstract void ApplyEffect(GameObject target);
        public abstract void RemoveEffect(GameObject target);
    }
}