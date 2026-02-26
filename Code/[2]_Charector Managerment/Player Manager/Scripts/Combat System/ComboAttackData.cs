using UnityEngine;

namespace PlayerInputs
{
    [CreateAssetMenu(fileName = "NewAttack", menuName = "Combat/Attack Data")]
    public class ComboAttackData : ScriptableObject
    {
        public int ComboIndex = 1;
        public float ComboInputWindow = 0.5f;
        public ComboAttackData NextCombo;
    }
}