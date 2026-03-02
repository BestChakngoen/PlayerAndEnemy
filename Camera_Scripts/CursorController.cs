using UnityEngine;
using UnityEngine.SceneManagement;
using GameSystem;

public class CursorController : MonoBehaviour
{
    [Header("Event Listening")]
    public GameStateEventSO GameStateChangedChannel;

    [Header("Gameplay Scenes")]
    public string[] gameplayScenes = { "Act 1 Map", "Act 2 Map", "Act 3 Boss Map" };

    private bool isForceUnlocked = false;

    private void Start()
    {
        UpdateCursorForCurrentScene();
    }

    private void OnEnable()
    {
        if (GameStateChangedChannel != null)
        {
            GameStateChangedChannel.OnEventRaised.AddListener(HandleGameStateChanged);
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        if (GameStateChangedChannel != null)
        {
            GameStateChangedChannel.OnEventRaised.RemoveListener(HandleGameStateChanged);
        }
        SceneManager.sceneLoaded -= OnSceneLoaded;
        
        // บังคับปลดล็อกเมาส์เสมอเมื่อสคริปต์นี้ถูกปิดการทำงาน (เช่น ตอนกำลังจะเปลี่ยน Scene)
        SetCursorState(false);
    }

    private void OnDestroy()
    {
        // ป้องกันการเกิดบั๊กเมาส์ล็อคค้างเมื่อ Object ถูกทำลาย
        SetCursorState(false);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isForceUnlocked = false; 
        UpdateCursorForCurrentScene();
    }

    public void ForceUnlockCursor()
    {
        isForceUnlocked = true;
        SetCursorState(false);
    }

    private void HandleGameStateChanged(GameState newState)
    {
        if (isForceUnlocked) return;

        if (!IsGameplayScene())
        {
            SetCursorState(false);
            return;
        }

        if (newState == GameState.Gameplay)
        {
            SetCursorState(true);
        }
        else if (newState == GameState.Paused || newState == GameState.GameOver)
        {
            SetCursorState(false);
        }
    }

    private void UpdateCursorForCurrentScene()
    {
        if (isForceUnlocked)
        {
            SetCursorState(false);
            return;
        }

        if (IsGameplayScene())
        {
            bool isGameplayState = true;
            if (GameStateManager.Instance != null)
            {
                isGameplayState = GameStateManager.Instance.CurrentState == GameState.Gameplay;
            }
            SetCursorState(isGameplayState);
        }
        else
        {
            SetCursorState(false);
        }
    }

    private bool IsGameplayScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        foreach (string sceneName in gameplayScenes)
        {
            if (currentScene.Contains(sceneName))
            {
                return true;
            }
        }
        return false;
    }

    private void SetCursorState(bool isLocked)
    {
        Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isLocked;
    }
}