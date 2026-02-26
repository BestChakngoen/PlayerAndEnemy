using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class JumpScareParryManager : MonoBehaviour
{
    public static JumpScareParryManager Instance;

    [Header("Timing")]
    [SerializeField] private float minTriggerTime = 20f;
    [SerializeField] private float maxTriggerTime = 40f;
    [SerializeField] private float parryTimeLimit = 1f;

    [Header("Parry Gauge")]
    [SerializeField] private int requiredParryCount = 8;

    [Header("References")]
    [SerializeField] private GameObject jumpScareUI;
    [SerializeField] private CanvasGroup fadePanel;
    [SerializeField] private CameraShake cameraShake;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private int freezeLayerIndex = 1; // JumpScareFreeze
 
    [SerializeField] private GameManger.PlayerDeathHandler playerDeathHandler;
    
    [SerializeField] private ParryGaugeUI parryGaugeUI;

    // --- ส่วนที่เพิ่มใหม่ (Audio) ---
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;      // ตัวเล่นเสียง
    [SerializeField] private AudioClip jumpScareClip;      // ไฟล์เสียงผีหลอก (เสียงกรี๊ด/เสียงดัง)
    [SerializeField] private AudioClip successClip;        // (เสริม) เสียงเมื่อรอด (เช่น เสียงถอนหายใจ)
    // ----------------------------

    private float timer;
    private int currentParry;
    private bool isActive;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
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

    void ScheduleNextJumpScare()
    {
        Invoke(nameof(TriggerJumpScare), Random.Range(minTriggerTime, maxTriggerTime));
    }

    void TriggerJumpScare()
    {
        if (GameManger.GameStateManager.Instance.CurrentState 
            != GameManger.GameState.Gameplay)
            return;

        isActive = true;
        timer = 0;
        currentParry = 0;

        jumpScareUI.SetActive(true);
        parryGaugeUI.Setup(requiredParryCount);

        cameraShake.StartShake();
        PlayerStateController.SetControl(false);
        
        if (playerAnimator != null)
        {
            playerAnimator.SetLayerWeight(freezeLayerIndex, 1f);
        }

        // --- เพิ่มการเล่นเสียง ---
        if (audioSource != null && jumpScareClip != null)
        {
            audioSource.clip = jumpScareClip;
            audioSource.Play(); // เล่นเสียงกรี๊ด
        }
    }

    public void OnParryInput()
    {
        if (!isActive) return;

        currentParry++;
        parryGaugeUI.AddParry();

        if (currentParry >= requiredParryCount)
        {
            SuccessParry();
        }
    }


    void SuccessParry()
    {
        StartCoroutine(ParrySuccessRoutine());
    }
    
    void OnEnable()
    {
        // CancelInvoke(); // *ระวัง: ปกติ OnEnable ไม่ควร CancelInvoke ที่ตั้งไว้ใน Start ทันที ถ้า Object นี้ไม่ได้ถูกปิด/เปิดบ่อยๆ
        // แต่ถ้า Logic เกมคุณต้อง Reset เมื่อเปิดใหม่ ก็ใช้ได้ครับ
    }

    void FailParry()
    {
        isActive = false;
        StartCoroutine(ParryFailRoutine());
        CancelInvoke(nameof(TriggerJumpScare));
    }

    IEnumerator ParrySuccessRoutine()
    {
        isActive = false;
        cameraShake.StopShake();

        // --- หยุดเสียงผีเมื่อผู้เล่นรอด ---
        if (audioSource != null)
        {
            audioSource.Stop(); 
            // ถ้าอยากให้มีเสียง "รอดแล้ว" ให้ใส่ successClip
            if(successClip != null) audioSource.PlayOneShot(successClip);
        }

        yield return FadeOut();

        jumpScareUI.SetActive(false);
        parryGaugeUI.Hide();

        yield return FadeIn();
        
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

        cameraShake.StopShake();
        parryGaugeUI.Hide();
        
        // หมายเหตุ: กรณีตาย ปกติเราจะปล่อยให้เสียง jumpScareClip เล่นต่อไปจนจบ หรือให้ playerDeathHandler จัดการเสียงตาย

        yield return FadeOut();
        
        jumpScareUI.SetActive(false);
        
        if (playerDeathHandler != null)
        {
            playerDeathHandler.KillByJumpScare();
        }
    }

    IEnumerator FadeOut()
    {
        while (fadePanel.alpha < 1)
        {
            fadePanel.alpha += Time.deltaTime * 3f;
            yield return null;
        }
    }

    IEnumerator FadeIn()
    {
        while (fadePanel.alpha > 0)
        {
            fadePanel.alpha -= Time.deltaTime * 2f;
            yield return null;
        }
    }
}