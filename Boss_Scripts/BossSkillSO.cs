using UnityEngine;

namespace Boss.scripts
{
    public abstract class BossSkillSO : ScriptableObject
    {
        public string skillName;
        public float cooldown;

        public abstract bool CanExecute(BossFSM fsm);
        public abstract void Execute(BossFSM fsm);
    }
}