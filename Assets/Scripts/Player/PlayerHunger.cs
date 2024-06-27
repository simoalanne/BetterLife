using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class PlayerHunger : MonoBehaviour
    {
        [SerializeField] private Sprite[] _hungerSprites; // Array to store the hunger sprites. Last index is not hungry, first index is hungry.
        [SerializeField] private Image _spritesArea;

        void Awake()
        {
            StartCoroutine(UpdateHunger());
        }

        IEnumerator UpdateHunger()
        {
            for (int i = _spritesArea.transform.childCount - 1; i >= 0; i--)
            {
                for (int j = 0; j < _hungerSprites.Length; j++)
                {
                    _spritesArea.transform.GetChild(i).GetComponent<Image>().sprite = _hungerSprites[j];
                    yield return new WaitForSeconds(0.5f);
                }
            }

            foreach (Transform child in _spritesArea.transform)
            {
                StartCoroutine(PingPongJiggle(child.GetComponent<RectTransform>()));
            }
        }

        IEnumerator PingPongJiggle(RectTransform rect)
        {
            float a = rect.anchoredPosition.y;
            float b = rect.anchoredPosition.y - 2.5f;
            float jiggleDuration = 0.25f;

            while (true)
            {
                float t = 0;
                while (t < jiggleDuration)
                {
                    t += Time.deltaTime;
                    rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, Mathf.Lerp(a, b, Mathf.PingPong(t / jiggleDuration, 1)));
                    yield return null;
                }
            }
        }
    }
}
