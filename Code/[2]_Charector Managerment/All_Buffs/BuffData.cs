using UnityEngine;

namespace BasicEnemy
{
    public abstract class BuffData : ScriptableObject
    {
        public float duration;
        
        public abstract Buff CreateBuff();
    }
}