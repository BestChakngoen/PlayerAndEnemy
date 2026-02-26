using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class BossHealthUI : MonoBehaviour
{
    public GameObject healthBarContainer;
    public Slider healthSlider;
    public Slider easeHealthSlider;
    public float lerpSpeed = 0.05f;

    private Coroutine healthBarCoroutine;
    
    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        easeHealthSlider.value = currentHealth / maxHealth;
        
        if (healthBarCoroutine != null)
        {
            StopCoroutine(healthBarCoroutine);
        }
        
        healthBarCoroutine = StartCoroutine(UpdateHealthSlider(currentHealth, maxHealth));
    }

    private IEnumerator UpdateHealthSlider(float currentHealth, float maxHealth)
    {
        float targetValue = currentHealth / maxHealth;
        
        while (Mathf.Abs(healthSlider.value - targetValue) > 0.01f)
        {
            healthSlider.value = Mathf.Lerp(healthSlider.value, targetValue, lerpSpeed);
            yield return null;
        }
        
        healthSlider.value = targetValue;
    }
    public void ShowHealthBar()
    {
        if (healthBarContainer != null)
        {
            healthBarContainer.SetActive(true);
        }
    }

    public void HideHealthBar()
    {
        if (healthBarContainer != null)
        {
            healthBarContainer.SetActive(false);
        }
    }
}
