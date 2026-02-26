using UnityEngine;

namespace BasicEnemy
{
    public class KnockbackState : CharacterStateComponent
    {
        private Vector3 _currentForce; 
        
        protected override void OnStateEnter(CC_Data data, Vector3 direction)
        {
            KnockbackCCData knockbackData = data as KnockbackCCData;
            if (knockbackData == null) return; 

            CurrentData = data;
        
            Vector3 initialForce = direction.normalized * knockbackData.baseMagnitude;
            _currentForce = new Vector3(initialForce.x, 0, initialForce.z); 
        }

        public override Vector3 CalculateForce(float deltaTime)
        {
            KnockbackCCData knockbackData = CurrentData as KnockbackCCData;
            if (knockbackData == null) return Vector3.zero;

            if (_currentForce.magnitude > 0.1f)
            {
                // ใช้ค่า Drag Coefficient จาก Data เฉพาะ
                _currentForce = Vector3.Lerp(_currentForce, Vector3.zero, deltaTime * knockbackData.dragCoefficient);
            
                return _currentForce;
            }
            else
            {
                _currentForce = Vector3.zero;
                if (Manager.ActiveState == this) 
                {
                    OnStateExit(); 
                }
                return Vector3.zero;
            }
        }
    }
}