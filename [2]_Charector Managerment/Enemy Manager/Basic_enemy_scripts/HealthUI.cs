using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthUI : MonoBehaviour
{
    public Slider healthSlider;
    public Slider easeHealthSlider;
    public float lerpSpeed = 0.05f;
    public Transform cameraTransform;

    private Coroutine healthBarCoroutine;

    void Awake()
    {
        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        if (cameraTransform != null)
        {
            transform.LookAt(transform.position + cameraTransform.forward);
        }
    }

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        easeHealthSlider.value = currentHealth / maxHealth;
        
        if (healthBarCoroutine != null)
        {
            StopCoroutine(healthBarCoroutine);
        }
        
        healthBarCoroutine = StartCoroutine(UpdateEaseSlider(currentHealth, maxHealth));
    }

    private IEnumerator UpdateEaseSlider(float currentHealth, float maxHealth)
    {
        float targetValue = currentHealth / maxHealth;
        
        while (Mathf.Abs(healthSlider.value - targetValue) > 0.01f)
        {
            healthSlider.value = Mathf.Lerp(healthSlider.value, targetValue, lerpSpeed);
            yield return null; 
        }
        
        healthSlider.value = targetValue;
    }
}