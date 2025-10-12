using DialogueSystem;
using UnityEngine;

namespace Home
{
    public class GameStart : MonoBehaviour
    {
        private void Start()
        {
            // Show some dialogue at game start
            if (Services.PlayerManager.HasReadGoodbyeNote) return;
            GetComponent<DialogueTrigger>()?.TriggerDialogue();
        }
    }
}
