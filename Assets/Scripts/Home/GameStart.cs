using Helpers;
using ScriptableObjects;
using UnityEngine;

namespace Home
{
    public class GameStart : MonoBehaviour
    {
        [SerializeField] private Conversation startingDialogue;
        [SerializeField] private float delayBeforeStarting = 0.5f;

        private void Start()
        {
            if (Services.PlayerManager.StoryProperties.HasReadGoodbyeNote) return;
            this.DoAfterDelay(() => startingDialogue.Start(), delayBeforeStarting);
        }
    }
}
