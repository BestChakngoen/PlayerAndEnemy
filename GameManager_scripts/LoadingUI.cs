using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingUI : MonoBehaviour
{
    public Slider progressBar;
    public TextMeshProUGUI progressText;

    public void UpdateProgress(float progress)
    {
        if (progressBar != null)
        {
            progressBar.value = progress;
        }

        if (progressText != null)
        {
            progressText.text = Mathf.RoundToInt(progress * 100f) + "%";
        }
    }
}