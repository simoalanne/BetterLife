using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckScript : MonoBehaviour
{
    public Sprite[] cardSprites;
    private int[] cardValues = new int[53];
    private int currentIndex;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void GetCardValues()
    {
        int num = 0;
        // Loop to assign values to the cards
        for (int i = 0; i < cardSprites.Length; i++)
        {
            num = i;
            // Count up to the amount of cards, 52
            num %= 13;
            // If there is a remainder after x/13, then use the remainder
            // Is used as the value, unless over 10, then use 10
            if (num > 10 || num == 0)
            {
                num = 10;
            }
            cardValues[i] = num++;
        }
        currentIndex = 1;
    }
}
