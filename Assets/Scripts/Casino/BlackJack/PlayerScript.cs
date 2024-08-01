using System.Collections.Generic;
using Player;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    // --- This script is for BOTH dealer and player.
    [SerializeField] private bool isPlayer; // Dealer cant get jokers so this is needed
    // Get other scripts
    public CardScript cardScript;
    public DeckScript deckScript;

    // Total value of players/dealers hand
    public int handValue = 0;

    // Betting money
    private int money;

    // Array of card objects on the table
    public GameObject[] hand;

    // Index of next card to be turned over
    public int cardIndex = 0;

    void Awake()
    {
        money = (int)PlayerManager.Instance.MoneyInBankAccount;
    }

    // Add a hand to the player/dealer's hand
    public void GetCard()
    {
        var cardToUse = hand[cardIndex].GetComponent<CardScript>();
        deckScript.DealCard(cardToUse);
        while (cardToUse.Value == -1 && !isPlayer) // Dealer cant get jokers so the game is more fair
        {
            Debug.Log("Dealer got a joker, drawing again");
            deckScript.DealCard(cardToUse);
        }
        hand[cardIndex].GetComponent<SpriteRenderer>().enabled = true;

        CalculateHandValue();

        cardIndex++;
    }

    void CalculateHandValue()
    {
        int tempHandValue = 0;
        int aceCount = 0;
        int jokerCount = 0;

        // First pass: calculate initial hand value and count aces and jokers
        foreach (GameObject card in hand)
        {
            int value = card.GetComponent<CardScript>().Value;
            if (value == 1)
            {
                aceCount++;
            }
            else if (value == -1)
            {
                jokerCount++;
            }
            else
            {
                tempHandValue += value;
            }
        }

        // Adjust for aces
        for (int i = 0; i < aceCount; i++)
        {
            if (tempHandValue + 11 <= 21)
            {
                tempHandValue += 11;
            }
            else
            {
                tempHandValue += 1;
            }
        }

        // Adjust for jokers
        for (int i = 0; i < jokerCount; i++)
        {
            tempHandValue += Mathf.Clamp(21 - tempHandValue, 1, 12); // Joker is a free win for player pretty much
        }

        handValue = tempHandValue;
    }

    // Add or subtract money, for bets
    public void AdjustMoney(int amount)
    {
        money += amount;
    }

    // Get function for money
    public int GetMoney()
    {
        PlayerManager.Instance.MoneyInBankAccount = money; // Update the money in bank account
        return money;
    }


    // Hides all cards, resets needed variables
    public void ResetHand()
    {
        for (int i = 0; i < hand.Length; i++)
        {
            hand[i].GetComponent<CardScript>().ResetCard();
            hand[i].GetComponent<Renderer>().enabled = false;
        }
        cardIndex = 0;
        handValue = 0;
    }
}
