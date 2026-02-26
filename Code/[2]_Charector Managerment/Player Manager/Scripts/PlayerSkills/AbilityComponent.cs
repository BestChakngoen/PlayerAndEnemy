using BasicEnemy;
using UnityEngine;

namespace PlayerSkills
{
    public abstract class AbilityComponent : MonoBehaviour
    {
        public float range = 5f; 
        public CC_Data ccData; 

        protected CC_Manager CasterManager;

        public void Initialize(CC_Manager manager)
        {
            CasterManager = manager;
        }
        
        public abstract void Activate();
        
        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}