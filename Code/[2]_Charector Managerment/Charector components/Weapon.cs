using BasicEnemy;
using UnityEngine;
using System.Collections.Generic; 

namespace BasicEnemy
{
    public class Weapon : MonoBehaviour
    {
        public float damage = 10f;
        private Collider weaponCollider;
        
        private List<Health> hitTargets = new List<Health>(); 

        private CharacterTag ownerTag;
        private GameObject ownerRoot;

        void Awake()
        {
            weaponCollider = GetComponent<Collider>();
            if (weaponCollider != null)
            {
                weaponCollider.enabled = false;
            }

            ownerRoot = transform.root.gameObject;
            ownerTag = ownerRoot.GetComponent<CharacterTag>();

            if (ownerTag == null)
            {
                Debug.LogError($"Weapon Owner ({ownerRoot.name}) does not have a CharacterTag component!");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == ownerRoot) return;

            Health hitHealth = other.GetComponent<Health>();

            if (hitHealth != null)
            {
                if (hitTargets.Contains(hitHealth))
                {
                    return; 
                }

                CharacterTag targetTag = other.GetComponentInParent<CharacterTag>();

                if (CanAttack(ownerTag, targetTag))
                {
                    hitHealth.TakeDamage(damage);
                    hitTargets.Add(hitHealth);
                }
            }
        }

        private bool CanAttack(CharacterTag owner, CharacterTag target)
        {
            if (owner == null || target == null)
            {
                return true;
            }

            if (owner.characterTag == target.characterTag)
            {
                return false;
            }

            return true;
        }

        public void EnableCollider()
        {
            if (weaponCollider != null)
            {
                weaponCollider.enabled = true;
                hitTargets.Clear();    
            }
        }

        public void DisableCollider()
        {
            if (weaponCollider != null)
            {
                weaponCollider.enabled = false;
            }
        }
    }
}