using UnityEngine;
using TMPro;

public class DamageTextManager : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float timeToLive = 2f;
    public float startScale = 2f;
    public float endScale = 0.5f;

    private TextMeshProUGUI textMesh;
    private float timer;
    private Transform cameraTransform;
    private Vector3 initialPosition;

    void Awake()
    {
        textMesh = GetComponentInChildren<TextMeshProUGUI>();
        timer = 0f;

        transform.localScale = Vector3.one * startScale;
        Destroy(gameObject, timeToLive);

        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        transform.position += new Vector3(0, moveSpeed * Time.deltaTime, 0);
        
        float t = Mathf.Clamp01(timer / (timeToLive / 2f));
        float currentScale = Mathf.Lerp(startScale, endScale, t);
        transform.localScale = Vector3.one * currentScale;
        float fadeT = timer / timeToLive;
        Color currentColor = textMesh.color;
        currentColor.a = Mathf.Lerp(1f, 0f, fadeT);
        textMesh.color = currentColor;

        if (cameraTransform != null)
        {
            Vector3 lookDirection = transform.position - cameraTransform.position;
            Quaternion rotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = rotation;
        }
    }

    public void SetDamageText(float damageAmount)
    {
        textMesh.text = damageAmount.ToString();
    }
}