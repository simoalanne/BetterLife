using System.Collections;
using Helpers;
using UnityEngine;

namespace Casino.BlackJack
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class BlackjackCard : MonoBehaviour
    {
        [SerializeField] private Sprite faceDownSprite;
        private SpriteRenderer _spriteRenderer;
        public CardData Data { get; private set; }

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            faceDownSprite = faceDownSprite ? faceDownSprite : _spriteRenderer.sprite;
        }

        public void Init(CardData data, bool faceUp = true)
        {
            Data = data;
            SetFace(faceUp);
        }

        public IEnumerator Reveal()
        {
            yield return transform.SpriteFlip(new RendererSprite(_spriteRenderer), Data.Sprite);
            SetFace(true);
        }

        private void SetFace(bool faceUp) => _spriteRenderer.sprite = faceUp ? Data.Sprite : faceDownSprite;

        public void DestroyCard() => Destroy(gameObject);
    }
}
