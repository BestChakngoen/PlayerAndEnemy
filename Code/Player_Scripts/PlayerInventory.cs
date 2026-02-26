using UnityEngine;
using UnityEngine.InputSystem; 
using TMPro; 
using GameManger;

namespace BasicEnemy
{
    public class PlayerInventory : MonoBehaviour
    {
        [Header("Heal Item Settings")] 
        public int currentHealPotions = 0;
        public int healAmountPerPotion = 20;

        [Header("Cooldown Settings")] 
        public float useCooldown = 1f;
        private float nextUseTime = 0f;

        [Header("UI & Dependencies")] 
        public TextMeshProUGUI potionCountText;
        private Health playerHealth;
        private PlayerControls playerControls;
        
        public GameObject useHealVFXPrefab;

        void Awake()
        {
            playerHealth = GetComponent<Health>();
            playerControls = new PlayerControls();
        }

        void OnEnable()
        {
            playerControls.Enable();
            playerControls.Player.Use.performed += OnHealPerformed;
            UpdatePotionUI();
        }
        void OnDisable()
        {
            playerControls.Player.Use.performed -= OnHealPerformed;
            playerControls.Disable();
        }
        void Update()
        {
            if (nextUseTime > Time.time)
            {

            }
        }
        public void AddPotion()
        {
            currentHealPotions++;
            UpdatePotionUI();
        }

        public void OnHealPerformed(InputAction.CallbackContext context)
        {
            UsePotion();
        }
        private void UsePotion()
        {
            if (playerHealth == null) return;
            
            if (currentHealPotions > 0 && Time.time >= nextUseTime)
            {
                playerHealth.Heal(healAmountPerPotion);
                currentHealPotions--;
                nextUseTime = Time.time + useCooldown; 
                
                SpawnUseHealVFX();
                UpdatePotionUI();
                
                //Debug.Log($"Used Potion. Healed {healAmountPerPotion}. Remaining: {currentHealPotions}");
            }
            else if (currentHealPotions == 0)
            {
                //Debug.Log("No Heal Potions left.");
            }
            else if (Time.time < nextUseTime)
            {
                //Debug.Log("Heal Potion is on cooldown.");
            }
        }
        public void UpdatePotionUI()
        {
            if (potionCountText != null)
            {
                potionCountText.text = currentHealPotions.ToString();
            }
        }
        private void SpawnUseHealVFX()
        {
            if (useHealVFXPrefab == null) 
            {
                Debug.LogWarning("Use Heal VFX Prefab is not assigned in PlayerInventory.");
                return;
            }
            
            GameObject vfxInstance = Instantiate(
                useHealVFXPrefab, 
                transform.position, 
                Quaternion.identity, 
                this.transform 
            );
            
            ParticleSystem ps = vfxInstance.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                float duration = ps.main.duration + ps.main.startLifetime.constantMax;
                Destroy(vfxInstance, duration);
            }
            else
            {
                Destroy(vfxInstance, 3f); 
            }
        }
    }
}