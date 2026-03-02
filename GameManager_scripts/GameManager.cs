using UnityEngine;
using GameSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private SceneManger _sceneManger;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _sceneManger = SceneManger.Instance;
    }
}