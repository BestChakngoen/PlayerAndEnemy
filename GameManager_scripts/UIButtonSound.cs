using UnityEngine;
using UnityEngine.EventSystems;

namespace GameManger
{
    public class UIButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
    {
        public AudioClip hoverSound;
        public AudioClip clickSound;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (AudioManager.Instance != null && hoverSound != null)
            {
                Vector3 playPosition = Camera.main != null ? Camera.main.transform.position : transform.position;
                AudioManager.Instance.PlaySFX(hoverSound, playPosition);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (AudioManager.Instance != null && clickSound != null)
            {
                Vector3 playPosition = Camera.main != null ? Camera.main.transform.position : transform.position;
                AudioManager.Instance.PlaySFX(clickSound, playPosition);
            }
        }
    }
}