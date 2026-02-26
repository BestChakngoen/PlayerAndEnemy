namespace PlayerInputs.Core
{
    public interface IPlayerMovement
    {
        void SetMoveInput(UnityEngine.Vector2 input);
        void SetMovementMode(PlayerMovementController.MovementMode mode);
    }

    public interface IPlayerCombat
    {
        bool CanMove { get; }
        void Attack();
    }

    public interface IPlayerStamina
    {
        bool TryRoll();
    }
}