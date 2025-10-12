using DialogueSystem;
using UnityEngine;

namespace City
{
    [RequireComponent(typeof(SceneLoadTrigger))]
    public class OpeningHours : MonoBehaviour, IInteractable
    {
        [SerializeField, Range(0, 24)] private int openFromInclusive;
        [SerializeField, Range(0, 24)] private int openToExclusive;
        [SerializeField] private ClosedSign closedSign;
        private SceneLoadTrigger _sceneLoadTrigger;

        private void Awake()
        {
            _sceneLoadTrigger = GetComponent<SceneLoadTrigger>();
        }

        public bool CanInteract { get; set; } = true;

        public void Interact()
        {
            if (IsOpen())
            {
                _sceneLoadTrigger.LoadScene();
                return;
            }
            ShowClosedSign();
        }

        private bool IsOpen()
        {
            var currentHour = Services.GameTimer.Hours;
            if (openFromInclusive < openToExclusive)
            {
                return currentHour >= openFromInclusive && currentHour < openToExclusive;
            }

            // If the place closes after midnight, handle the wrap-around
            return currentHour >= openFromInclusive || currentHour < openToExclusive;
        }

        private void ShowClosedSign()
        {
            Services.GameTimer.IsPaused = true;
            Services.PlayerManager.DisableInputs();
            var sign = Instantiate(closedSign, Services.PlayerHUD.transform);
            sign.OnSignClosed += () =>
            {
                CanInteract = true;
                Services.GameTimer.IsPaused = false;
                Services.PlayerManager.EnableInputs();
                if (TryGetComponent(out DialogueTrigger trigger))
                {
                    trigger.TriggerDialogue();
                    return;
                }
                Services.PlayerManager.EnableInputs();
            };
            sign.Show(FormatHour(openFromInclusive), FormatHour(openToExclusive));
        }

        private static string FormatHour(int hour) => hour < 10 ? $"0{hour}" : hour.ToString();
    }
}
