using UnityEngine;

namespace BasicEnemy
{
    public class StunState : CharacterStateComponent
    {
        protected override void OnStateEnter(CC_Data data, Vector3 direction)
        {
            StunCCData stunData = data as StunCCData;
            if (stunData == null) return;

            CurrentData = data;
        }

        public override Vector3 CalculateForce(float deltaTime)
        {
            return Vector3.zero;
        }
    }
}