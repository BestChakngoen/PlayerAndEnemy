using UnityEngine;

namespace BasicEnemy.Enemy.Core
{
    public interface IBossContext
    {
        Transform PlayerTransform { get; }
        Transform BossTransform { get; }
        void StopMovement();
        void ResumeMovement();
        void LookAtPlayerImmediate();
        void RotateToPlayerSmoothly(float speed);
    }
}