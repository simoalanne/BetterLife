using UnityEngine;
using System.Collections.Generic;

public class DeckScript : MonoBehaviour
{
    [SerializeField] private bool includeJokers = true;
    private const int DeckSize = 52;
    private Sprite[] cardSprites;
    private Dictionary<Sprite, int> cardValueDict = new();
    private List<Sprite> cardSpritesCopy = new();

    void Awake()
    {
        cardSprites = CardGameHelper.LoadCardSprites(includeJokers);
    }

    void Start()
    {
        GetCardValues();
        Shuffle();
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
        Sprite dealtCard = cardSpritesCopy[randomIndex];
        cardScript.SetSprite(dealtCard);
        cardScript.Value = cardValueDict[dealtCard];
        cardSpritesCopy.RemoveAt(randomIndex);
    }
}