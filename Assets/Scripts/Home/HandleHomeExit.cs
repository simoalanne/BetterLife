using Helpers;
using NaughtyAttributes;
using ScriptableObjects;
using UnityEngine;

namespace Home
{
    public class HandleHomeExit : MonoBehaviour
    {
        [SerializeField, Scene] private string citySceneName = "City";
        [SerializeField, Scene] private string gameEndingSceneName = "GameOverCutscene";
        [SerializeField] private Conversation exitNotAllowed;

        private void OnTriggerEnter2D(Collider2D other)
        {
            // There are 3 different scenarios:
            // 1. The player has not read the goodbye note yet -> show dialogue
            // 2. The player has not paid their loans in time -> go to game ending
            // 3. The player is allowed to exit -> load city scene
            if (!Services.PlayerManager.StoryProperties.HasReadGoodbyeNote)
            {
                exitNotAllowed.Start();
                return;
            }

            // Yes PlayerHUD should not have anything to do with this data
            if (Services.PlayerHUD.ActiveLoan?.daysToRepay is 0)
            {
                gameEndingSceneName.LoadScene();
                return;
            }
            citySceneName.LoadScene();
        }
    }
}
