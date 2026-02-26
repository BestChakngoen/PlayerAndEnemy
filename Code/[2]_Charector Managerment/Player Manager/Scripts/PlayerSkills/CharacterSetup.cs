using BasicEnemy;
using UnityEngine;

namespace PlayerSkills
{
    public class CharacterSetup : MonoBehaviour
    {
        private CC_Manager _ccManager;
        private AreaCCAbility _knockbackAbility;
        private AreaCCAbility _airborneAbility;
        private AreaCCAbility _stunAbility;

        void Start()
        {
            _ccManager = GetComponent<CC_Manager>();
            if (_ccManager == null)
            {
                //Debug.LogError("CharacterSetup requires CC_Manager component.");
                return;
            }

            // 1. ดึง Component สกิลที่เพิ่มไว้
            AreaCCAbility[] abilities = GetComponentsInChildren<AreaCCAbility>();

            // 2. Map สกิลเข้ากับตัวแปรตาม CC Data Type
            foreach (var ability in abilities)
            {
                ability.Initialize(_ccManager);
                if (ability.ccData == null) continue;

                if (ability.ccData.stateType == CC_Data.CCStateType.Knockback)
                    _knockbackAbility = ability;
                else if (ability.ccData.stateType == CC_Data.CCStateType.Airborne)
                    _airborneAbility = ability;
                else if (ability.ccData.stateType == CC_Data.CCStateType.Stun)
                    _stunAbility = ability;
            }
        }

        void Update()
        {
            /*// กด 1: Knockback
            if (Input.GetKeyDown(KeyCode.Alpha1) && _knockbackAbility != null)
            {
                _knockbackAbility.Activate();
            }

            // กด 2: Airborne
            if (Input.GetKeyDown(KeyCode.Alpha2) && _airborneAbility != null)
            {
                _airborneAbility.Activate();
            }

            // กด 3: Stun
            if (Input.GetKeyDown(KeyCode.Alpha3) && _stunAbility != null)
            {
                _stunAbility.Activate();
            }*/
        }
    }
}