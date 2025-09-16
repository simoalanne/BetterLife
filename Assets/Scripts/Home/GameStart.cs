using UnityEngine;

namespace Home
{
    public class GameStart : MonoBehaviour
    {
        private void Start()
        {
            // Show some dialogue at game start
            if (Player.PlayerManager.Instance.HasReadGoodbyeNote) return;
            GetComponent<DialogueTrigger>()?.TriggerDialogue();
        }
    }
}
