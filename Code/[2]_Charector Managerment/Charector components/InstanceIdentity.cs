using UnityEngine;

namespace BasicEnemy
{
    [DisallowMultipleComponent] 
    public class InstanceIdentity : MonoBehaviour
    {
        [SerializeField]
        private int uniqueInstanceID; 

        public int UniqueInstanceID => uniqueInstanceID; 

        void Awake()
        {
            uniqueInstanceID = gameObject.GetInstanceID();
        }
        public int GetID()
        {
            if (uniqueInstanceID == 0)
            {
                return gameObject.GetInstanceID(); 
            }
            return uniqueInstanceID; 
        }
    }
}