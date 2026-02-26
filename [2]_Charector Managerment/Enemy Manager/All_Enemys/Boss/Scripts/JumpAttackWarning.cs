using UnityEngine;

namespace BasicEnemy.Enemy.Wendigo_FolkFall
{
    public class JumpAttackWarning : MonoBehaviour
    {
        [Header("Settings")]
        public float indicatorDuration = 1.5f;
        public GameObject indicatorDecal;    
        public Material warningMaterial;       
        public Material lockedMaterial;          

        private MeshRenderer decalRenderer;
        private Transform targetTransform;
        private bool isTracking = false;
        private Transform originalParent;

        void Awake()
        {
            if (indicatorDecal != null)
            {
                decalRenderer = indicatorDecal.GetComponent<MeshRenderer>();
                indicatorDecal.SetActive(false); 
            }
        }

        public void StartTrackingAndWarning(Transform target)
        {
            if (indicatorDecal == null) return;
        
            targetTransform = target;
            isTracking = true;
            indicatorDecal.SetActive(true);
            if (decalRenderer != null && warningMaterial != null)
            {
                decalRenderer.material = warningMaterial;
            }
        }

        public Vector3 LockTargetPosition()
        {
            isTracking = false;
            
            Vector3 lockedPosition = targetTransform != null ? targetTransform.position : transform.position;
            Vector3 indicatorPos = lockedPosition;
            indicatorPos.y = transform.position.y; 
            transform.position = indicatorPos;
            
            if (transform.parent != null)
            {
                originalParent = transform.parent;
                transform.SetParent(null); 
            }
            if (decalRenderer != null && lockedMaterial != null)
            {
                decalRenderer.material = lockedMaterial;
            }

            return lockedPosition;
        }

        public void StopWarning()
        {
            isTracking = false;
            if (indicatorDecal != null)
            {
                indicatorDecal.SetActive(false);
            }
            if (originalParent != null)
            {
                transform.SetParent(originalParent);
                originalParent = null; 
            }
        }

        void LateUpdate()
        {
            if (isTracking && targetTransform != null)
            {
                Vector3 targetPos = targetTransform.position;
                targetPos.y = transform.position.y; 
                transform.position = targetPos;
            }
        }
    }
}