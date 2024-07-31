using UnityEngine;

/// <summary>
/// Deletes all instances that control game logic when the main menu is loaded. Makes it easier to start a new game.
/// </summary>
public class DeleteGameInstances : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var playerManager = FindObjectOfType<Player.PlayerManager>();
        if (playerManager != null)
        {
            Destroy(playerManager.gameObject);
        }
        var playerHUD = FindObjectOfType<PlayerHUD>();
        if (playerHUD != null)
        {
            Destroy(playerHUD.gameObject);
        }

        var gameTimer = FindObjectOfType<GameTimer>();
        if (gameTimer != null)
        {
            Destroy(gameTimer.gameObject);
        }
    }
}
