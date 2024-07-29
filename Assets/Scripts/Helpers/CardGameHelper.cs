
using UnityEngine;
using System.Linq;

public static class CardGameHelper
{
    private const string CardsPath = "Cards"; 
    public static void Shuffle<T>(T[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }
    }

    /// <summary>
    /// Loads all card sprites from the Resources folder
    /// </summary>
    public static Sprite[] LoadCardSprites(bool includeJokers = true)
    {
        var cardArray = Resources.LoadAll<Sprite>(CardsPath);
        return includeJokers ? cardArray : cardArray.Take(52).ToArray();
    }
}