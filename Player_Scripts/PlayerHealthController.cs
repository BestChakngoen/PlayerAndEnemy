using UnityEngine;
using UnityEngine.SceneManagement;
using CoreSystem;
using GameSystem;
using GameManger;

namespace PlayerInputs
{
    [RequireComponent(typeof(Health))]
    public class PlayerHealthController : MonoBehaviour
    {
        private Health health;
        [SerializeField] private PlayerAnimationFacade animationFacade;
        
        [Header("Audio")]
        [SerializeField] private AudioClip[] hitSounds;
        [SerializeField] private AudioClip[] dieSounds;

        private float previousHealth;
        private PlayerInputController inputController;
        private MonoBehaviour movementController;
        private MonoBehaviour physicsController;
        private MonoBehaviour combatController;
        private MonoBehaviour manaSkillController;
        private MonoBehaviour staminaController;
        private MonoBehaviour unityPlayerInput;

        private bool isIncapacitated = false;

        private void Awake()
        {
            health = GetComponent<Health>();
            if (animationFacade == null) animationFacade = GetComponentInChildren<PlayerAnimationFacade>();
            inputController = GetComponent<PlayerInputController>();
            
            previousHealth = health.GetMaxHealth();

            MonoBehaviour[] allBehaviours = GetComponentsInChildren<MonoBehaviour>(true);
            foreach (var mb in allBehaviours)
            {
                if (mb == null) continue;
                string typeName = mb.GetType().Name;
                if (typeName == "PlayerMovementController") movementController = mb;
                if (typeName == "PlayerPhysicsController") physicsController = mb;
                if (typeName == "PlayerCombatController") combatController = mb;
                if (typeName == "PlayerManaSkillController") manaSkillController = mb;
                if (typeName == "PlayerStaminaController") staminaController = mb;
                if (typeName == "PlayerInput") unityPlayerInput = mb; 
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnEnable()
        {
            health.OnDeath += HandleDeath;
            health.OnHealthChanged += HandleHealthChanged;
            if (animationFacade != null)
            {
                animationFacade.OnHitEnd += HandleHitEnd;
                animationFacade.OnStunStateChanged += HandleStunStateChanged;
            }
        }

        private void OnDisable()
        {
            health.OnDeath -= HandleDeath;
            health.OnHealthChanged -= HandleHealthChanged;
            if (animationFacade != null)
            {
                animationFacade.OnHitEnd -= HandleHitEnd;
                animationFacade.OnStunStateChanged -= HandleStunStateChanged;
            }
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void Update()
        {
            if (isIncapacitated)
            {
                BroadcastMessage("ResetVelocity", SendMessageOptions.DontRequireReceiver);
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (health != null && health.GetCurrentHealth() <= 0)
            {
                health.ResetHealth();
                previousHealth = health.GetMaxHealth();
                
                SetPlayerActiveState(true);

                MonoBehaviour ccHandler = GetComponent<CCSystem.ICrowdControlReceiver>() as MonoBehaviour;
                if (ccHandler != null)
                {
                    ccHandler.enabled = true;
                }

                if (animationFacade != null)
                {
                    animationFacade.ResetDeathState();
                    Animator animator = animationFacade.GetAnimator();
                    if (animator != null)
                    {
                        if (animator.gameObject.activeInHierarchy)
                        {
                            animator.Rebind();
                            animator.Update(0f);
                        }
                    }
                }
            }
        }

        private void SetPlayerActiveState(bool isActive)
        {
            PlayerStateController.SetControl(isActive);
            isIncapacitated = !isActive;
            
            if (inputController != null) inputController.enabled = isActive;
            if (movementController != null) movementController.enabled = isActive;
            if (physicsController != null) physicsController.enabled = isActive;
            if (combatController != null) combatController.enabled = isActive;
            if (manaSkillController != null) manaSkillController.enabled = isActive;
            if (staminaController != null) staminaController.enabled = isActive;
            if (unityPlayerInput != null) unityPlayerInput.enabled = isActive;

            if (!isActive)
            {
                BroadcastMessage("ResetVelocity", SendMessageOptions.DontRequireReceiver);
            }
        }

        private void HandleStunStateChanged(bool isStunned)
        {
            if (health != null && health.GetCurrentHealth() <= 0) return;

            SetPlayerActiveState(!isStunned);

            if (isStunned)
            {
                if (animationFacade != null)
                {
                    animationFacade.SetMovementSpeed(0f);
                }
            }
        }

        private void HandleHealthChanged(float currentHealth, float maxHealth)
        {
            if (currentHealth < previousHealth && currentHealth > 0)
            {
                SetPlayerActiveState(false);

                if (animationFacade != null)
                {
                    animationFacade.SetMovementSpeed(0f);
                    animationFacade.PlayHit();
                }

                if (hitSounds != null && hitSounds.Length > 0 && AudioManager.Instance != null)
                {
                    AudioClip clip = hitSounds[Random.Range(0, hitSounds.Length)];
                    AudioManager.Instance.PlaySFX(clip, transform.position);
                }
            }
            previousHealth = currentHealth;
        }

        private void HandleHitEnd()
        {
            if (health != null && health.GetCurrentHealth() > 0)
            {
                bool isStillStunned = animationFacade != null && animationFacade.IsStunned;

                if (!isStillStunned)
                {
                    SetPlayerActiveState(true);
                }
            }
        }

        private void HandleDeath()
        {
            UIManager.IsWin = false;
            
            SetPlayerActiveState(false);

            MonoBehaviour ccHandler = GetComponent<CCSystem.ICrowdControlReceiver>() as MonoBehaviour;
            if (ccHandler != null)
            {
                ccHandler.enabled = false;
            }

            if (dieSounds != null && dieSounds.Length > 0 && AudioManager.Instance != null)
            {
                AudioClip clip = dieSounds[Random.Range(0, dieSounds.Length)];
                AudioManager.Instance.PlaySFX(clip, transform.position);
            }

            if (animationFacade != null)
            {
                Animator animator = animationFacade.GetAnimator();
                if (animator != null)
                {
                    foreach (AnimatorControllerParameter param in animator.parameters)
                    {
                        if (param.type == AnimatorControllerParameterType.Trigger)
                        {
                            animator.ResetTrigger(param.name);
                        }
                        else if (param.type == AnimatorControllerParameterType.Bool)
                        {
                            animator.SetBool(param.name, false);
                        }
                    }
                }

                animationFacade.SetMovementSpeed(0f);
                animationFacade.PlayDie();
            }

            // แก้ไขปัญหา Obsolete โดยเปลี่ยนมาใช้ FindObjectsByType แทน
            Boss.scripts.BossAnimator[] bossAnimators = FindObjectsByType<Boss.scripts.BossAnimator>(FindObjectsSortMode.None);
            foreach (var bossAnim in bossAnimators)
            {
                Animator bAnimator = bossAnim.GetComponent<Animator>();
                if (bAnimator != null)
                {
                    foreach (AnimatorControllerParameter param in bAnimator.parameters)
                    {
                        if (param.type == AnimatorControllerParameterType.Trigger)
                        {
                            bAnimator.ResetTrigger(param.name);
                        }
                        else if (param.type == AnimatorControllerParameterType.Bool)
                        {
                            bAnimator.SetBool(param.name, false);
                        }
                    }
                    bAnimator.SetFloat("Speed", 0f);
                }
            }
        }
    }
}