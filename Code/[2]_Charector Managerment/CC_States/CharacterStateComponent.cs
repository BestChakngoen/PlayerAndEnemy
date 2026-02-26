using UnityEngine;

namespace BasicEnemy
{
    public abstract class CharacterStateComponent : MonoBehaviour
    {
        protected CC_Data CurrentData;
        protected CC_Manager Manager;

        public void Initialize(CC_Manager manager)
        {
            Manager = manager;
        }

        // ===== Public Lifecycle API =====

        public void Enter(CC_Data data, Vector3 direction)
        {
            CurrentData = data;
            OnStateEnter(data, direction);
        }

        public void Exit()
        {
            OnStateExit();
        }

        // ===== Core Logic (Protected) =====

        protected abstract void OnStateEnter(CC_Data data, Vector3 direction);
        protected virtual void OnStateExit() { }

        public abstract Vector3 CalculateForce(float deltaTime);
    }
}