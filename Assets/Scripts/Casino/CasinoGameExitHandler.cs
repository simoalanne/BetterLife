using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace Casino
{
    /// <summary>
    /// Reusable script that can be used in combination with an exit button to correctly exit out of a casino game scene.
    /// If the player has a PlayerManager instance (i.e. they came from story mode), they can be sent back to story mode.
    /// otherwise, can be sent back to the main menu.
    /// </summary>
    public class CasinoGameExitHandler : MonoBehaviour
    {
        [SerializeField, Scene] private string mainMenuScene = "MainMenu";
        [SerializeField, Scene] private string storyModeScene = "Casino";

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                var prevScene = Services.PlayerManager.PreviousSceneName;
                if (prevScene == mainMenuScene || prevScene is null)
                {
                    Services.SceneLoader.LoadScene(mainMenuScene);
                    return;
                }
                Services.SceneLoader.LoadScene(storyModeScene);
            });

            var moneyHandler = Services.MoneyHandler;
            moneyHandler.OnFirstBetPlaced += () => GetComponent<Button>().interactable = false;
            moneyHandler.OnAllBetsCleared += () => GetComponent<Button>().interactable = true;
        }
    }
}
