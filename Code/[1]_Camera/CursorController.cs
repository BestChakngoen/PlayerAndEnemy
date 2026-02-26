using UnityEngine;

public class CursorController : MonoBehaviour
{
    void Start()
    {
        // ล็อค Cursor ไว้ที่กลางหน้าจอและซ่อนมันไป
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // แนะนำ: เพิ่มเงื่อนไขให้กดปุ่ม (เช่น Esc) เพื่อเรียกเมาส์กลับมาเวลาต้องการ Debug หรือออกหน้าเมนู
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // สลับสถานะการล็อค
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}