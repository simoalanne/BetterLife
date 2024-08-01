using UnityEngine;
using System.Collections.Generic;

public class DeckScript : MonoBehaviour
{
    [SerializeField] private bool includeJokers = true;
    private const int DeckSize = 52;
    private Sprite[] cardSprites;
    private Dictionary<Sprite, int> cardValueDict = new();
    private List<Sprite> cardSpritesCopy = new();
    int totalJokersFromRandom = 0;

    void Awake()
    {
        cardSprites = CardGameHelper.LoadCardSprites(includeJokers);
    }

    void Start()
    {
        GetCardValues();
        Shuffle();
        RandomTest();
    }

    private void GetCardValues()
    {
        int valueIndex = 1;
        int valueCap = 10;
        for (int i = 0; i < cardSprites.Length; i++)
        {
            if (i >= DeckSize)
            {
                valueIndex = -1; // Jokers
            }
            cardValueDict.Add(cardSprites[i], valueIndex);
            if ((i + 1) % 4 == 0 && valueIndex < valueCap)
            {
                valueIndex++;
            }
        }
        cardSpritesCopy = new List<Sprite>(cardSprites);
    }

    public void Shuffle()
    {
        CardGameHelper.Shuffle(cardSprites);
        cardSpritesCopy = new List<Sprite>(cardSprites);
    }

    public void DealCard(CardScript cardScript)
    {
        int randomIndex = Random.Range(0, cardSpritesCopy.Count);
        var value = cardValueDict[cardSpritesCopy[randomIndex]];
        if (value == -1)
        {
            Debug.Log("Joker drawn 50% chance to draw again");
            bool drawAgain = Random.Range(0, 2) == 0; // 50% chance to draw again
            if (drawAgain)
            {
                Debug.Log("Draw again");
                randomIndex = Random.Range(0, cardSpritesCopy.Count);
            }
        }
        Sprite dealtCard = cardSpritesCopy[randomIndex];
        cardScript.SetSprite(dealtCard);
        cardScript.Value = cardValueDict[dealtCard];
        cardSpritesCopy.RemoveAt(randomIndex);
    }

    void RandomTest()
    {
        for (int j = 0; j < 5; j++)
        {
            for (int i = 0; i < 10000; i++)
            {
                int randomIndex = Random.Range(0, cardSpritesCopy.Count);
                var value = cardValueDict[cardSpritesCopy[randomIndex]];
                if (value == -1)
                {
                    bool drawAgain = Random.Range(0, 2) == 0; // 50% chance to draw again
                    if (drawAgain)
                    {
                        randomIndex = Random.Range(0, cardSpritesCopy.Count);
                        totalJokersFromRandom++;
                    }
                }
                cardSpritesCopy.RemoveAt(randomIndex);
                Shuffle();
            }
            Debug.Log("joker appeared total: " + totalJokersFromRandom + " times from 10000 random draws\nappear chance: " + (float)totalJokersFromRandom / 50000 * 100 + "%");
            totalJokersFromRandom = 0;
        }
    }
}