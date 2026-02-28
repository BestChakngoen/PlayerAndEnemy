using UnityEngine;

namespace CCSystem
{
    public abstract class CCEffectSO : ScriptableObject
    {
        public float duration = 1f;
        public abstract void Apply(GameObject target, Vector3 sourcePosition);
        public virtual void Remove(GameObject target) { }
    }
}