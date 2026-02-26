using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float intensity = 1.5f;
    private Vector3 originalPos;
    private bool shaking;

    void Awake()
    {
        originalPos = transform.localPosition;
    }

    public void StartShake()
    {
        shaking = true;
    }

    public void StopShake()
    {
        shaking = false;
        transform.localPosition = originalPos;
    }

    void Update()
    {
        if (!shaking) return;

        transform.localPosition = originalPos + Random.insideUnitSphere * intensity;
    }
}