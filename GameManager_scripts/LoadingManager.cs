using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using GameSystem;

public class LoadingManager : MonoBehaviour
{
    public LoadingUI loadingUI;
    public float minimumLoadingTime = 1.5f;

    private void Start()
    {
        if (!string.IsNullOrEmpty(SceneManger.targetSceneName))
        {
            StartCoroutine(LoadSceneAsyncProcess(SceneManger.targetSceneName));
        }
    }

    private IEnumerator LoadSceneAsyncProcess(string sceneName)
    {
        float elapsedTime = 0f;
        
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            elapsedTime += Time.unscaledDeltaTime;
            
            float sceneProgress = Mathf.Clamp01(operation.progress / 0.9f);
            float timeProgress = Mathf.Clamp01(elapsedTime / minimumLoadingTime);
            
            float currentProgress = Mathf.Min(sceneProgress, timeProgress);

            if (loadingUI != null)
            {
                loadingUI.UpdateProgress(currentProgress);
            }

            if (operation.progress >= 0.9f && elapsedTime >= minimumLoadingTime)
            {
                if (loadingUI != null)
                {
                    loadingUI.UpdateProgress(1f);
                }
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}