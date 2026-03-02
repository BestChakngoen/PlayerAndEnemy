using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using GameSystem;
using PlayerInputs;
using GameManger; 

public class JumpScareParryManager : MonoBehaviour
{
    public static JumpScareParryManager Instance;

    [Header("Timing")]
    [SerializeField] private float minTriggerTime = 20f;
    [SerializeField] private float maxTriggerTime = 40f;
    [SerializeField] private float parryTimeLimit = 1f;
    [SerializeField] private float spaceBlinkSpeed = 20f;

    [Header("Parry Gauge")]
    [SerializeField] private int requiredParryCount = 8;

    [Header("References")]
    [SerializeField] private GameObject jumpScareUI;
    [SerializeField] private GameObject spaceTextPrompt;
    [SerializeField] private CanvasGroup fadePanel;
    [SerializeField] private int freezeLayerIndex = 1;
    [SerializeField] private ParryGaugeUI parryGaugeUI;

    [Header("Audio")]
    [SerializeField] private AudioClip jumpScareClip;      
    [SerializeField] private AudioClip successClip;        

    private CameraShake cameraShake;
    private Animator playerAnimator;
    private FPSMouseLook fpsCamera;

    private float timer;
    private int currentParry;
    private bool isActive;
    private Coroutine blinkCoroutine;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        FindPlayerComponents();

        if (spaceTextPrompt != null)
        {
            spaceTextPrompt.SetActive(false);
        }
        ScheduleNextJumpScare();
    }

    void Update()
    {
        if (!isActive) return;

        timer += Time.deltaTime;
        if (timer >= parryTimeLimit)
        {
            FailParry();
        }
    }

    private void FindPlayerComponents()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null && PlayerPersistent.Instance != null)
        {
            player = PlayerPersistent.Instance.gameObject;
        }

        if (player != null)
        {
            if (cameraShake == null) cameraShake = player.GetComponentInChildren<CameraShake>();
            if (playerAnimator == null) playerAnimator = player.GetComponentInChildren<Animator>();
            if (fpsCamera == null) fpsCamera = player.GetComponentInChildren<FPSMouseLook>();
        }
    }

    void ScheduleNextJumpScare()
    {
        Invoke(nameof(TriggerJumpScare), Random.Range(minTriggerTime, maxTriggerTime));
    }

    void TriggerJumpScare()
    {
        if (GameStateManager.Instance == null || GameStateManager.Instance.CurrentState != GameState.Gameplay)
            return;

        FindPlayerComponents();

        isActive = true;
        timer = 0;
        currentParry = 0;

        if (jumpScareUI != null) jumpScareUI.SetActive(true);
        if (parryGaugeUI != null) parryGaugeUI.Setup(requiredParryCount);

        if (cameraShake != null) cameraShake.StartShake();
        PlayerStateController.SetControl(false);
        
        if (fpsCamera != null)
        {
            fpsCamera.LockRotation(true);
        }

        if (playerAnimator != null)
        {
            playerAnimator.SetLayerWeight(freezeLayerIndex, 1f);
        }
        
        if (jumpScareClip != null && AudioManager.Instance != null)
        {
            Vector3 soundPos = Camera.main != null ? Camera.main.transform.position : transform.position;
            AudioManager.Instance.PlaySFX(jumpScareClip, soundPos);
        }

        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
        }
        blinkCoroutine = StartCoroutine(BlinkSpaceTextRoutine());
    }

    IEnumerator BlinkSpaceTextRoutine()
    {
        if (spaceTextPrompt == null) yield break;

        CanvasGroup canvasGroup = spaceTextPrompt.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = spaceTextPrompt.AddComponent<CanvasGroup>();
        }

        spaceTextPrompt.SetActive(true);
        float blinkTimer = 0f;

        while (isActive)
        {
            blinkTimer += Time.deltaTime * spaceBlinkSpeed;
            canvasGroup.alpha = Mathf.PingPong(blinkTimer, 1f) > 0.5f ? 1f : 0f;
            yield return null;
        }

        canvasGroup.alpha = 1f;
        spaceTextPrompt.SetActive(false);
    }

    public void OnParryInput()
    {
        if (!isActive) return;

        currentParry++;
        if (parryGaugeUI != null) parryGaugeUI.AddParry();

        if (currentParry >= requiredParryCount)
        {
            SuccessParry();
        }
    }

    void SuccessParry()
    {
        StartCoroutine(ParrySuccessRoutine());
    }
    
    void FailParry()
    {
        isActive = false;
        StartCoroutine(ParryFailRoutine());
        CancelInvoke(nameof(TriggerJumpScare));
    }

    private void StopJumpScareSound()
    {
        if (jumpScareClip == null) return;
        
        AudioSource[] allSources = Object.FindObjectsByType<AudioSource>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (AudioSource src in allSources)
        {
            if (src.clip == jumpScareClip && src.isPlaying)
            {
                src.Stop();
            }
        }
    }

    IEnumerator ParrySuccessRoutine()
    {
        isActive = false;
        
        if (cameraShake != null) cameraShake.StopShake();

        StopJumpScareSound();

        if (successClip != null && AudioManager.Instance != null)
        {
            Vector3 soundPos = Camera.main != null ? Camera.main.transform.position : transform.position;
            AudioManager.Instance.PlaySFX(successClip, soundPos);
        }

        yield return FadeOut();

        if (jumpScareUI != null) jumpScareUI.SetActive(false);
        if (parryGaugeUI != null) parryGaugeUI.Hide();

        if (spaceTextPrompt != null) spaceTextPrompt.SetActive(false);

        yield return FadeIn();
        
        if (fpsCamera != null)
        {
            fpsCamera.LockRotation(false);
        }

        if (playerAnimator != null)
        {
            playerAnimator.SetLayerWeight(freezeLayerIndex, 0f);
        }

        PlayerStateController.SetControl(true);
        ScheduleNextJumpScare();
    }

    IEnumerator ParryFailRoutine()
    {
        isActive = false;

        if (cameraShake != null) cameraShake.StopShake();
        if (parryGaugeUI != null) parryGaugeUI.Hide();
        
        StopJumpScareSound();

        yield return FadeOut();
        
        if (jumpScareUI != null) jumpScareUI.SetActive(false);
        if (spaceTextPrompt != null) spaceTextPrompt.SetActive(false);
        
        if (playerAnimator != null)
        {
            playerAnimator.SetLayerWeight(freezeLayerIndex, 0f);
        }

        yield return FadeIn();

        if (fpsCamera != null)
        {
            fpsCamera.LockRotation(false);
        }

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.SetState(GameState.GameOver);
        }
    }

    IEnumerator FadeOut()
    {
        if (fadePanel == null) yield break;
        while (fadePanel.alpha < 1)
        {
            fadePanel.alpha += Time.deltaTime * 3f;
            yield return null;
        }
    }

    IEnumerator FadeIn()
    {
        if (fadePanel == null) yield break;
        while (fadePanel.alpha > 0)
        {
            fadePanel.alpha -= Time.deltaTime * 2f;
            yield return null;
        }
    }
}