using UnityEngine;

namespace BasicEnemy
{
    public class AirborneState : CharacterStateComponent
    {
        private Vector3 _currentVelocity; 

        protected override void OnStateEnter(CC_Data data, Vector3 direction)
        {
            AirborneCCData airborneData = data as AirborneCCData;
            if (airborneData == null) return;

            CurrentData = data;
        
            Vector3 initialForceHorizontal = direction.normalized * airborneData.baseMagnitude;
            Vector3 initialForceVertical = Vector3.up * airborneData.verticalLaunchForce;

            _currentVelocity = initialForceHorizontal + initialForceVertical;
        }

        public override Vector3 CalculateForce(float deltaTime)
        {
            AirborneCCData airborneData = CurrentData as AirborneCCData;
            if (airborneData == null) return Vector3.zero;
        
            _currentVelocity.y -= Manager.gravity * deltaTime;

            Vector3 horizontalVelocity = new Vector3(_currentVelocity.x, 2, _currentVelocity.z);
        
            {
                // ใช้ค่า Drag Coefficient เฉพาะ Airborne
                horizontalVelocity = Vector3.Lerp(horizontalVelocity, Vector3.zero, deltaTime * airborneData.airborneDragCoefficient);
            }

            _currentVelocity = new Vector3(horizontalVelocity.x, _currentVelocity.y, horizontalVelocity.z);
        
            return _currentVelocity;
        }
    }
}