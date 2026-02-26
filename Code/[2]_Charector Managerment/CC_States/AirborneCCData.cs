using UnityEngine;

namespace BasicEnemy
{
    [CreateAssetMenu(fileName = "AirborneCCData", menuName = "CC_States/CC Data/Airborne")]
    public class AirborneCCData : CC_Data
    {
        public AirborneCCData() { stateType = CCStateType.Airborne; }

        [Header("Airborne Parameters")]
        public float baseMagnitude = 40f; // แรงในแนวราบ
        public float verticalLaunchForce = 25f; // แรงเริ่มต้นในแนวดิ่ง
        public float airborneDragCoefficient = 5f; // Drag ในขณะลอยตัว
    }
}