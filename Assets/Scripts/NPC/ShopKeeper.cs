using System.Collections;
using DialogueSystem;
using Helpers;
using NaughtyAttributes;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace NPC
{
    [RequireComponent(typeof(SpriteRenderer), typeof(Light2D), typeof(Collider2D))]
    public class ShopKeeper : MonoBehaviour, IInteractable
    {
        [Header("Shop Keeper initial interaction")]
        [SerializeField] private Conversation oneAndOnlyInteraction;
        [SerializeField] private Conversation afterVanished;
        [SerializeField] private float vanishDuration = 1f;

        [Header("Cursed shopkeeper")]
        [SerializeField] private Color cursedColor = Color.red;
        [SerializeField] private Conversation forfeitTheGame;
        [SerializeField, Scene] private string gameOverScene = "GameOverCutscene";

        private void Awake()
        {
            if (!Services.PlayerManager.StoryProperties.HasTalkedToShopkeeper) return;
            GetComponent<SpriteRenderer>().color = cursedColor;
            GetComponent<Light2D>().enabled = true;
        }

        public void Interact()
        {
            if (Services.PlayerManager.StoryProperties.HasTalkedToShopkeeper)
            {
                var wantedToForfeit = false;
                forfeitTheGame.Start(state =>
                {
                    if (state is DialogueState.YesClicked) wantedToForfeit = true;

                    if (state is DialogueState.DialogueFinished && wantedToForfeit)
                        StartCoroutine(Forfeit());
                });
                return;
            }

            oneAndOnlyInteraction.Start(onStateChange: state =>
            {
                if (state is not DialogueState.DialogueFinished) return;
                Services.PlayerManager.StoryProperties.HasTalkedToShopkeeper = true;
                StartCoroutine(Vanish());
            });
        }

        private IEnumerator Forfeit()
        {
            var playerSprite = Services.PlayerManager.GetComponent<SpriteRenderer>();
            yield return FadeSprite(playerSprite);
            gameOverScene.LoadScene();
        }

        private IEnumerator Vanish()
        {
            gameObject.layer = LayerMask.NameToLayer("Default");
            var spriteRenderer = GetComponent<SpriteRenderer>();
            yield return FadeSprite(spriteRenderer);
            afterVanished.Start();
        }

        private IEnumerator FadeSprite(SpriteRenderer spriteRenderer)
        {
            Services.InputManager.EnablePlayerInput(false);
            var initialColor = spriteRenderer.color;
            yield return FunctionLibrary.DoOverTime(vanishDuration, t =>
            {
                var color = initialColor;
                color.a = Mathf.Lerp(1f, 0f, t);
                spriteRenderer.color = color;
            });
        }
    }
}
