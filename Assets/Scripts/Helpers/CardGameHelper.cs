using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Helpers
{
    public static class CardGameHelper
    {
        private const string CardsPath = "Cards";

        /// <summary>
        /// Loads all card sprites from the Resources folder. Expected order is that aces come first then regular cards,
        /// then finally jokers (if included). first 4 sprites are skipped since they contain face-down and other irrelevant cards.
        /// </summary>
        public static List<Sprite> LoadCardSprites(bool includeJokers = true)
        {
            var cardArray = Resources.LoadAll<Sprite>(CardsPath);
            const int jokers = 4;
            return cardArray.Take(52 + (includeJokers ? jokers : 0)).ToList();
        }

        public static IEnumerator AnimateCardToPosition(GameObject spawnedCard, Vector2 start, Vector2 end,
            float travelTime)
        {
            var elapsed = 0f;
            while (elapsed < travelTime)
            {
                elapsed += Time.deltaTime;
                spawnedCard.transform.position = Vector2.Lerp(start, end, elapsed / travelTime);
                yield return null;
            }
        }
    }
}
