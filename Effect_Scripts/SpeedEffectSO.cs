using UnityEngine;

namespace Boss.core
{
    [CreateAssetMenu(fileName = "SpeedEffect", menuName = "Effects/Speed Effect")]
    public class SpeedEffectSO : EffectSO
    {
        [Range(0.1f, 3.0f)]
        public float speedMultiplier = 0.7f; 

        public override void ApplyEffect(GameObject target)
        {
            // เปลี่ยนมาใช้ GetComponentsInChildren เพื่อหา Interface ในตัวลูกด้วย
            ISpeedModifiable[] modifiables = target.GetComponentsInChildren<ISpeedModifiable>();
            
            foreach (var modifiable in modifiables)
            {
                modifiable.MultiplySpeed(speedMultiplier);
            }
        }

        public override void RemoveEffect(GameObject target)
        {
            // เปลี่ยนมาใช้ GetComponentsInChildren เพื่อหา Interface ในตัวลูกด้วย
            ISpeedModifiable[] modifiables = target.GetComponentsInChildren<ISpeedModifiable>();
            
            foreach (var modifiable in modifiables)
            {
                modifiable.DivideSpeed(speedMultiplier);
            }
        }
    }
}