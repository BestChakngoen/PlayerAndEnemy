using UnityEngine;
using PlayerInputs.Core;

namespace PlayerInputs
{
    public class PlayerManaSkillController : MonoBehaviour, IActionLockable
    {
        public float maxMana = 100;
        public float regenRate = 5;
        public float skillCost = 40;
        public float cooldown = 10;

        private float mana;
        private float[] timers;
        private PlayerStateController stateController;
        private bool isActionLocked = false;

        void Awake()
        {
            mana = maxMana;
            if (timers == null) timers = new float[3];
            stateController = GetComponentInParent<PlayerStateController>();
        }

        void Update()
        {
            if (stateController != null && !stateController.CanControl) return;

            mana = Mathf.Min(maxMana, mana + regenRate * Time.deltaTime);
            
            if (timers != null)
            {
                for (int i = 0; i < timers.Length; i++)
                    timers[i] -= Time.deltaTime;
            }
        }

        public void LockAction()
        {
            isActionLocked = true;
        }

        public void UnlockAction()
        {
            isActionLocked = false;
        }

        public void ResetMana()
        {
            mana = maxMana;
            isActionLocked = false;
            if (timers != null)
            {
                for (int i = 0; i < timers.Length; i++) timers[i] = 0f;
            }
        }

        public void CastSkill(int index)
        {
            if (isActionLocked) return;
            if (timers == null || index < 0 || index >= timers.Length) return;
            if (timers[index] > 0 || mana < skillCost) return;

            mana -= skillCost;
            timers[index] = cooldown;
        }
    }
}