public static class PlayerStateController
{
    public static bool CanControl { get; private set; } = true;
    public static void SetControl(bool value) => CanControl = value;
}