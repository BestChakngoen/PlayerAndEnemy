using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject damageTextPrefab;
    private SceneManger _sceneManger;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        _sceneManger = GetComponent<SceneManger>();

    }

    private void Start()
    {
        //_sceneManger.LoadSceneAdditive("Level_1_Scene");
    }
}