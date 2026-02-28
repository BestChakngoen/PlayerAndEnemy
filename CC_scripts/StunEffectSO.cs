using UnityEngine;

namespace CCSystem
{
    [CreateAssetMenu(fileName = "NewStunEffect", menuName = "CC/Stun")]
    public class StunEffectSO : CCEffectSO
    {
        public override void Apply(GameObject target, Vector3 sourcePosition)
        {
            ICrowdControlReceiver receiver = target.GetComponentInChildren<ICrowdControlReceiver>();
            if (receiver != null)
            {
                receiver.ApplyStun(duration);
            }
        }

        public override void Remove(GameObject target)
        {
            ICrowdControlReceiver receiver = target.GetComponentInChildren<ICrowdControlReceiver>();
            if (receiver != null)
            {
                receiver.RemoveStun();
            }
        }
    }
}