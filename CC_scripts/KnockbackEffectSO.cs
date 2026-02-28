using UnityEngine;

namespace CCSystem
{
    [CreateAssetMenu(fileName = "NewKnockbackEffect", menuName = "CC/Knockback")]
    public class KnockbackEffectSO : CCEffectSO
    {
        [SerializeField] private float force = 10f;

        public override void Apply(GameObject target, Vector3 sourcePosition)
        {
            ICrowdControlReceiver receiver = target.GetComponentInChildren<ICrowdControlReceiver>();
            if (receiver != null)
            {
                Vector3 direction = (target.transform.position - sourcePosition).normalized;
                direction.y = 0;
                receiver.ApplyKnockback(direction, force, duration);
            }
        }
    }
}