using UnityEngine;

public class CursorController : MonoBehaviour
{
    void Start()
    {
        SetCursorState(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetCursorState(Cursor.lockState != CursorLockMode.Locked);
        }
    }

    private void SetCursorState(bool isLocked)
    {
        Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isLocked;
    }
}