
using UnityEngine;
using Unity.Cinemachine;

namespace Others
{
    public class CameraController : MonoBehaviour
    {
        [Header("Press C to switch camera brain")] 
        [Header("Hold MouseRight and use arrows to move camera, Use pageUp/pageDown to up/down")] 
        [Header("Movement Settings")] 
        [SerializeField] private float moveSpeed = 5.0f; 

        [SerializeField] private CinemachineBrain cinemachineBrain;
        [SerializeField] private float sprintSpeedMultiplier = 2.0f; 

        [Header("Look Settings")] 
        [SerializeField] private float lookSpeed = 2.0f; 

        [SerializeField] private float lookXLimit = 80.0f; 

        private float rotationX = 0; 


        void Start()
        {
            rotationX = transform.localEulerAngles.x;
            
            if (rotationX > 180)
            {
                rotationX -= 360;
            }
            
        }

        void Update()
        {
            if (Input.GetMouseButton(1)) 
            {
                Cursor.lockState = CursorLockMode.Locked;
                //Cursor.visible = false;
                
                HandleLookInput();
                HandleMovementInput();
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                //Cursor.visible = true;
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                ToggleCinemachineBrain();
            }
        }
        void ToggleCinemachineBrain()
        {
            if (cinemachineBrain != null)
            {
                cinemachineBrain.enabled = !cinemachineBrain.enabled;
                Debug.Log("Cinemachine Brain is now " + (cinemachineBrain.enabled ? "Enabled" : "Disabled"));
            }
        }
        void HandleLookInput()
        {
            float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
            float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

            transform.Rotate(Vector3.up * mouseX);
            
            rotationX -= mouseY;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit); 
            transform.localRotation = Quaternion.Euler(rotationX, transform.localEulerAngles.y, 0);
        }

        void HandleMovementInput()
        {
            float currentMoveSpeed = moveSpeed;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                currentMoveSpeed *= sprintSpeedMultiplier;
            }

            Vector3 moveDirection = Vector3.zero;
            
            if (Input.GetKey(KeyCode.UpArrow))
            {
                moveDirection += transform.forward;
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                moveDirection -= transform.forward;
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                moveDirection -= transform.right;
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                moveDirection += transform.right;
            }
            
            if (Input.GetKey(KeyCode.PageUp)) 
            {
                moveDirection += Vector3.up; 
            }

            if (Input.GetKey(KeyCode.PageDown)) 
            {
                moveDirection -= Vector3.up;
            }
            else if (Input.GetKey(KeyCode.RightControl)) 
            {
                moveDirection -= Vector3.up;
            }
            
            if (moveDirection.magnitude > 0) 
            {
                moveDirection.Normalize();
            }
            
            transform.position += moveDirection * currentMoveSpeed * Time.deltaTime;
        }
    }
}