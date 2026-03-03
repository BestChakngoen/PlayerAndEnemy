using UnityEngine;

namespace PlayerInputs
{
    public class PlayerStateController : MonoBehaviour
    {
        public bool CanControl { get; private set; } = true;
        
        public void SetControl(bool value)
        {
            CanControl = value;
        }
    }
}