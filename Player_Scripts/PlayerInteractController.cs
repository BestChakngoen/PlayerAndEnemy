using UnityEngine;

public class PlayerInteractController : MonoBehaviour
{
    [Header("Raycast")]
    public float interactDistance = 3f;
    public LayerMask interactLayer;

    [Header("Highlight")]
    public bool enableHighlight = true;
    public Color emissiveColor = Color.white;
    public float emissiveIntensity = 2f;

    private IInteractable currentInteractable;
    private Renderer currentRenderer;
    private Color[] originalEmissive;
    private IHoldInteractable currentHold;
    private bool isHolding;
    private IHoldInteractable holdingTarget;



    void Update()
    {
        DetectInteractable();
        
    }


    void DetectInteractable()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer))
        {
            var hitHold = hit.collider.GetComponentInParent<IHoldInteractable>();
            var hitInteract = hit.collider.GetComponentInParent<IInteractable>();

            // 🔴 กำลัง Hold อยู่ แต่ Raycast หลุดเป้าหมาย
            if (isHolding && holdingTarget != null && hitHold != holdingTarget)
            {
                holdingTarget.EndHold(gameObject);
                isHolding = false;
                holdingTarget = null;
            }

            currentHold = hitHold;
            currentInteractable = hitInteract;

            if (hitInteract != null)
            {
                if (currentRenderer != hit.collider.GetComponentInChildren<Renderer>())
                {
                    ClearHighlight();
                    ApplyHighlight(hit.collider.GetComponentInChildren<Renderer>());
                }
                return;
            }
        }

        // 🔴 Raycast ไม่โดนอะไรเลยขณะ Hold
        if (isHolding && holdingTarget != null)
        {
            holdingTarget.EndHold(gameObject);
            isHolding = false;
            holdingTarget = null;
        }

        ClearHighlight();
    }



    public void TryInteract()
    {
        if (currentHold != null && currentHold.RequiresHold)
        {
            isHolding = true;
            holdingTarget = currentHold;
            currentHold.BeginHold(gameObject);
        }
        else if (currentInteractable != null)
        {
            currentInteractable.Interact(gameObject);
            ClearHighlight();
        }
    }



    public void StopInteract()
    {
        if (currentHold != null)
        {
            currentHold.EndHold(gameObject);
            isHolding = false;
        }
    }

    // ================= Highlight =================

    void ApplyHighlight(Renderer renderer)
    {
        if (!enableHighlight || renderer == null) return;

        currentRenderer = renderer;
        var mats = currentRenderer.materials;
        originalEmissive = new Color[mats.Length];

        for (int i = 0; i < mats.Length; i++)
        {
            if (!mats[i].HasProperty("_EmissiveColor")) continue;

            originalEmissive[i] = mats[i].GetColor("_EmissiveColor");
            mats[i].EnableKeyword("_EMISSION");
            mats[i].SetColor("_EmissiveColor", emissiveColor * emissiveIntensity);
        }
    }

    void ClearHighlight()
    {
        if (currentRenderer == null) return;

        var mats = currentRenderer.materials;
        for (int i = 0; i < mats.Length; i++)
        {
            if (mats[i].HasProperty("_EmissiveColor"))
                mats[i].SetColor("_EmissiveColor", originalEmissive[i]);
        }

        currentRenderer = null;
        currentInteractable = null;
    }
}
