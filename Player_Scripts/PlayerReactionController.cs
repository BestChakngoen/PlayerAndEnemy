using UnityEngine;
using PlayerInputs.Core;

namespace PlayerInputs
{
    public class PlayerReactionController : MonoBehaviour
    {
        private PlayerAnimationFacade animationFacade;
        private IActionLockable[] lockables;

        void Awake()
        {
            animationFacade = GetComponentInChildren<PlayerAnimationFacade>();
            lockables = GetComponentsInChildren<IActionLockable>();
        }

        void OnEnable()
        {
            if (animationFacade != null)
            {
                animationFacade.OnHitStart += HandleHitStart;
                animationFacade.OnHitEnd += HandleHitEnd;
            }
        }

        void OnDisable()
        {
            if (animationFacade != null)
            {
                animationFacade.OnHitStart -= HandleHitStart;
                animationFacade.OnHitEnd -= HandleHitEnd;
            }
        }

        private void HandleHitStart()
        {
            if (lockables == null) return;
            foreach (var lockable in lockables)
            {
                lockable.LockAction();
            }
        }

        private void HandleHitEnd()
        {
            if (lockables == null) return;
            foreach (var lockable in lockables)
            {
                lockable.UnlockAction();
            }
        }
    }
}