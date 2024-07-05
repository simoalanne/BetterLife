using UnityEngine;
using UnityEngine.EventSystems;


namespace Casino.Roulette
{
    public class BetInfoTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private ButtonHighlight _buttonHighlight;
        private DisplayBetInfo _displayBetInfo;

        private void Awake()
        {
            _buttonHighlight = FindObjectOfType<ButtonHighlight>();
            _displayBetInfo = FindObjectOfType<DisplayBetInfo>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _buttonHighlight.HandlePointerEnter(gameObject);
            _displayBetInfo.SetBetInfo(gameObject.name);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _buttonHighlight.HandlePointerExit(gameObject);
            _displayBetInfo.HideBetInfo();
        }
    }
}
