using System.Collections;
using System.Collections.Generic;
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
    private int money = 1000;

    // Array of card objects on the table
    public GameObject[] hand;

    // Index of next card to be turned over
    public int cardIndex = 0;
    
    // Tracking aces for 1 to 11 conversions
    List<CardScript> aceList = new List<CardScript>();

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
        if(cardValue == 1)
        {
            aceList.Add(hand[cardIndex].GetComponent<CardScript>());
        }
        // AceCheck();
        cardIndex++;
        return handValue;
    }
}
