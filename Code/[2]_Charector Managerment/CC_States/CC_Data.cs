using UnityEngine;

namespace BasicEnemy
{
    public abstract class CC_Data : ScriptableObject
    {
        public enum CCStateType { Knockback, Airborne, Stun }
    
        [Header("Base State Info")]
        public CCStateType stateType;
        public float baseDuration = 0.5f; 
    }
}