using UnityEngine;

namespace CCSystem
{
    public interface ICrowdControlReceiver
    {
        void AddCC(CCEffectSO ccEffect, Vector3 sourcePosition);
        void ApplyKnockback(Vector3 direction, float force, float duration);
        void ApplyStun(float duration);
        void RemoveStun();
    }
}