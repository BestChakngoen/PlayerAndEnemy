using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayerInputs;

namespace CCSystem
{
    [System.Serializable]
    public class ActiveCC
    {
        public CCEffectSO effect;
        public float timer;
    }

    public class CrowdControlHandler : MonoBehaviour, ICrowdControlReceiver
    {
        [SerializeField] private List<ActiveCC> activeCCs = new List<ActiveCC>();
        [SerializeField] private PlayerAnimationFacade animationFacade;

        private CharacterController characterController;
        private PlayerStateController stateController;
        private int stunCount = 0;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            stateController = GetComponentInParent<PlayerStateController>();
            if (animationFacade == null) animationFacade = GetComponentInChildren<PlayerAnimationFacade>();
        }

        private void Update()
        {
            for (int i = activeCCs.Count - 1; i >= 0; i--)
            {
                activeCCs[i].timer -= Time.deltaTime;
                if (activeCCs[i].timer <= 0)
                {
                    activeCCs[i].effect.Remove(gameObject);
                    activeCCs.RemoveAt(i);
                }
            }
        }

        public void AddCC(CCEffectSO ccEffect, Vector3 sourcePosition)
        {
            if (ccEffect == null) return;

            var existing = activeCCs.Find(e => e.effect == ccEffect);
            if (existing != null)
            {
                existing.timer = ccEffect.duration;
            }
            else
            {
                activeCCs.Add(new ActiveCC { effect = ccEffect, timer = ccEffect.duration });
                ccEffect.Apply(gameObject, sourcePosition);
            }
        }

        public void ApplyKnockback(Vector3 direction, float force, float duration)
        {
            StartCoroutine(KnockbackRoutine(direction, force, duration));
        }

        private IEnumerator KnockbackRoutine(Vector3 direction, float force, float duration)
        {
            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                float currentForce = Mathf.Lerp(force, 0f, timer / duration);
                
                if (characterController != null)
                {
                    // เช็คก่อนเสมอว่าถูกเปิดใช้งานอยู่หรือไม่ ถ้าถูกปิดไปแล้วก็จะไม่ Move 
                    if (characterController.enabled)
                    {
                        characterController.Move(direction * currentForce * Time.deltaTime);
                    }
                }
                else
                {
                    transform.position += direction * currentForce * Time.deltaTime;
                }
                
                yield return null;
            }
        }

        public void ApplyStun(float duration)
        {
            stunCount++;
            if (stunCount == 1)
            {
                if (stateController != null) stateController.SetControl(false);
                if (animationFacade != null) animationFacade.SetStunState(true);
            }
        }

        public void RemoveStun()
        {
            stunCount--;
            if (stunCount <= 0)
            {
                stunCount = 0;
                if (stateController != null) stateController.SetControl(true);
                if (animationFacade != null) animationFacade.SetStunState(false);
            }
        }
    }
}