using UnityEngine;

namespace BasicEnemy
{
    [CreateAssetMenu(fileName = "StunCCData", menuName = "CC_States/CC Data/Stun")]
    public class StunCCData : CC_Data
    {
        public StunCCData() { stateType = CCStateType.Stun; }
        
    }
}