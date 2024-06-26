using UnityEngine;
using UnityEngine.EventSystems;


namespace Casino.Roulette
{
    public class HighlightTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private ButtonHighlight _buttonHighlight;

        private void Awake()
        {
            _buttonHighlight = FindObjectOfType<ButtonHighlight>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _buttonHighlight.HandlePointerEnter(gameObject);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _buttonHighlight.HandlePointerExit(gameObject);
        }
    }
}
