using System;
using UnityEngine;

namespace CoreSystem
{
    public class Health : MonoBehaviour, IDamageable
    {
        [SerializeField] private float maxHealth = 100f;
        public float currentHealth;

        public event Action<float, float> OnHealthChanged;
        public event Action OnDeath;

        private bool isDead;

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        public void TakeDamage(float amount)
        {
            if (isDead) return;

            // ตรวจสอบว่ามีระบบรับดาเมจออนไลน์ติดอยู่หรือไม่
            var networkCombat = GetComponent<OnlineSystem.NetworkCombatSync>();
            if (networkCombat != null && Photon.Pun.PhotonNetwork.IsConnected)
            {
                // โยนหน้าที่การหักเลือดไปให้ระบบ Network จัดการ เพื่อไม่ให้ดาเมจเกิดการทับซ้อน
                networkCombat.TakeDamage(amount);
                return;
            }

            ApplyDamage(amount);
        }

        // แยกฟังก์ชันหักเลือดที่แท้จริงออกมา เพื่อให้สคริปต์ Network มาสั่งงานตรงๆ ได้
        public void ApplyDamage(float amount)
        {
            if (isDead) return;

            currentHealth -= amount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            OnHealthChanged?.Invoke(currentHealth, maxHealth);

            if (currentHealth <= 0)
            {
                isDead = true;
                OnDeath?.Invoke();
            }
        }

        public void SetHealth(float value)
        {
            currentHealth = Mathf.Clamp(value, 0, maxHealth);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);

            if (currentHealth <= 0 && !isDead)
            {
                isDead = true;
                OnDeath?.Invoke();
            }
            else if (currentHealth > 0 && isDead)
            {
                isDead = false;
            }
        }

        public void ResetHealth()
        {
            currentHealth = maxHealth;
            isDead = false;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        public float GetCurrentHealth() => currentHealth;
        public float GetMaxHealth() => maxHealth;
    }
}