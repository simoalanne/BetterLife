using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    // --- This script is for BOTH dealer and player.

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

    // Tracking aces for 1 to 11 conversions
    List<CardScript> aceList = new List<CardScript>();

    void Awake()
    {
        money = (int)PlayerManager.Instance.MoneyInBankAccount;
    }

    public void StartHand()
    {
        GetCard();
        GetCard();
    }

    // Add a hand to the player/dealer's hand
    public int GetCard()
    {
        // Get a card, use DealCard to assign sprite and value to the card on the table
        int cardValue = deckScript.DealCard(hand[cardIndex].GetComponent<CardScript>());
        // Show card on game screen
        hand[cardIndex].GetComponent<Renderer>().enabled = true;
        // Add the cards value to the hand total value
        handValue += cardValue;
        // if value is 1, then the card is an ace
        if (cardValue == 1)
        {
            aceList.Add(hand[cardIndex].GetComponent<CardScript>());
        }
        AceCheck();
        cardIndex++;
        return handValue;
    }

    // Needed for ace conversion, 1 to 11 or vice versa
    public void AceCheck()
    {
        // For each ace in the acelist check
        foreach (CardScript ace in aceList)
        {
            if (handValue + 10 < 22 && ace.GetValueOfCard() == 1)
            {
                // if converting, adjust ace value and hand
                ace.SetValue(11);
                handValue += 10;
            }
            else if (handValue > 21 && ace.GetValueOfCard() == 11)
            {
                // if converting, adjust ace value and hand
                ace.SetValue(1);
                handValue -= 10;
            }
        }
    }

    // Add or subtract money, for bets
    public void AdjustMoney(int amount)
    {
        money += amount;
    }

    // Get function for money
    public int GetMoney()
    {
        PlayerManager.Instance.MoneyInBankAccount = money; 
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
        aceList = new List<CardScript>();
    }
}
