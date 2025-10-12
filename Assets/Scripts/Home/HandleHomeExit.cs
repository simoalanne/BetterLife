using DialogueSystem;
using UnityEngine;

namespace Home
{
    public class HandleHomeExit : MonoBehaviour
    {
        [SerializeField] private SceneLoadTrigger loadToCity;
        [SerializeField] private SceneLoadTrigger loadToGameEnding;
        [SerializeField] private DialogueTrigger exitNotAllowed;


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            // There are 3 different scenarios:
            // 1. The player has not read the goodbye note yet -> show dialogue
            // 2. The player has not paid their loans in time -> go to game ending
            // 3. The player is allowed to exit -> load city scene
            if (!Services.PlayerManager.HasReadGoodbyeNote)
            {
                exitNotAllowed.TriggerDialogue();
                return;
            }

            // Yes PlayerHUD should not have anything to do with this data
            if (Services.PlayerHUD.ActiveLoan?.daysToRepay is 0)
            {
                loadToGameEnding.LoadScene();
                return;
            }

            loadToCity.LoadScene();
        }
    }
}
