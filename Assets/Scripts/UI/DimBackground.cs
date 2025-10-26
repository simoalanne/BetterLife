using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    /// <summary> Dims the background and detects clicks on it. Can be used to close UI panels when clicking outside of them. </summary>
    [RequireComponent(typeof(EventTrigger), typeof(CanvasGroup))]
    public class DimBackground : MonoBehaviour
    {
        [SerializeField] private float dimAlpha = 0.5f;
        private EventTrigger _eventTrigger;
        private CanvasGroup _canvasGroup;

        public Action OnBackgroundClicked;

        public void Dim()
        {
            _canvasGroup.alpha = dimAlpha;
            _canvasGroup.blocksRaycasts = true;
        }


        public void UnDim()
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;
        }

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            UnDim();
            _eventTrigger = GetComponent<EventTrigger>();
            var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
            entry.callback.AddListener(_ => OnBackgroundClicked?.Invoke());
            _eventTrigger.triggers.Add(entry);
        }
    }
}
