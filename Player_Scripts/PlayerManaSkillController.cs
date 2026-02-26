using UnityEngine;


namespace PlayerInputs
{
    public class PlayerManaSkillController : MonoBehaviour
    {
        public float maxMana = 100;
        public float regenRate = 5;
        public float skillCost = 40;
        public float cooldown = 10;

  private float mana;
        private float[] timers;

        void Awake()
        {
            mana = maxMana;
        }

        void Update()
        {
            if (!PlayerStateController.CanControl) return;

            mana = Mathf.Min(maxMana, mana + regenRate * Time.deltaTime);
            for (int i = 0; i < timers.Length; i++)
                timers[i] -= Time.deltaTime;
        }

        public void CastSkill(int index)
        {
            if (timers[index] > 0 || mana < skillCost) return;

            mana -= skillCost;
            timers[index] = cooldown;

   
        }
    }

}