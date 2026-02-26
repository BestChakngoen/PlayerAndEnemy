using UnityEngine;

namespace BasicEnemy
{
    public abstract class CC_Manager : MonoBehaviour
    {
        public abstract float gravity { get; }
        public abstract void EndState(CharacterStateComponent state);
        public abstract CharacterStateComponent ActiveState { get; }
        public abstract void ApplyCC(CC_Data data, Vector3 direction);
    }
}