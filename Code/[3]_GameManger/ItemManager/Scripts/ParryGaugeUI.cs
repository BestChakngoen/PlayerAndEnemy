using UnityEngine;
using UnityEngine.UI;

public class ParryGaugeUI : MonoBehaviour
{
    [SerializeField] private Image gaugeFill;

    private int current;
    private int max;

    public void Setup(int requiredCount)
    {
        max = requiredCount;
        current = 0;
        UpdateGauge();
        gameObject.SetActive(true);
    }

    public void AddParry()
    {
        current++;
        UpdateGauge();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void UpdateGauge()
    {
        gaugeFill.fillAmount = (float)current / max;
    }
}