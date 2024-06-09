using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

namespace Casino.Roulette
{
    /// <summary>
    /// This class is responsible for handling the bets in the roulette game.
    /// <summary>
    public class RouletteBetHandler : MonoBehaviour
    {
        [SerializeField] private int _initialBalance = 100; // The initial balance of the player. Managed in this script for now.
        [SerializeField] private List<(string, int)> _activeBets = new(); // Tuple to store the bet type and amount.
        [SerializeField] private TMP_Text _balanceText; // Text to display the balance.
        [SerializeField] private TMP_Text _totalWinAmountText; // Text to display the total win amount.
        [SerializeField] private TMP_Text _winningNumberDetails; // Text to display the winning number details such as color, odd/even, etc.
        [SerializeField] private float _textOnScreenTime = 5; // Time to display the winning number details and total win amount in the screen after bet settlement.

        readonly Dictionary<string, Func<int, bool>> betConditions = new() // Dictionary to store the conditions for each bet type.
        {
            { "Red", (winningNumber) => new int[] { 1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36 }.Contains(winningNumber) },
            { "Black", (winningNumber) => new int[] { 2, 4, 6, 8, 10, 11, 13, 15, 17, 20, 22, 24, 26, 28, 29, 31, 33, 35 }.Contains(winningNumber) },
            { "1-18", (winningNumber) => winningNumber <= 18 },
            { "19-36", (winningNumber) => winningNumber > 18 },
            { "Even", (winningNumber) => winningNumber % 2 == 0 },
            { "Odd", (winningNumber) => winningNumber % 2 != 0 },
            { "1st 12", (winningNumber) => winningNumber <= 12 },
            { "2nd 12", (winningNumber) => winningNumber > 12 && winningNumber <= 24 },
            { "3rd 12", (winningNumber) => winningNumber > 24 },
            { "1st row", (winningNumber) => new int[] { 3, 6, 9, 12, 15, 18, 21, 24, 27, 30, 33, 36 }.Contains(winningNumber) },
            { "2nd row", (winningNumber) => new int[] { 2, 5, 8, 11, 14, 17, 20, 23, 26, 29, 32, 35 }.Contains(winningNumber) },
            { "3rd row", (winningNumber) => new int[] { 1, 4, 7, 10, 13, 16, 19, 22, 25, 28, 31, 34 }.Contains(winningNumber) },
        };

        readonly Dictionary<string, int> betPayouts = new() // Dictionary to store the payouts for each bet type.
        {
            { "1st 12", 3 },
            { "2nd 12", 3 },
            { "3rd 12", 3 },
            { "1-18", 2 },
            { "Even", 2 },
            { "Odd", 2 },
            { "19-36", 2 },
            { "Red", 2 },
            { "Black", 2 },
            { "1st row", 3 },
            { "2nd row", 3 },
            { "3rd row", 3 }
        };

        void Awake()
        {
            _balanceText.text = "Balance: " + _initialBalance + " euros"; // Set the text to display the initial balance.
            for (int i = 0; i <= 36; i++)
            {
                int capturedNumber = i; // Capture the loop variable
                betConditions[capturedNumber.ToString()] = (winningNumber) => winningNumber == capturedNumber; // Add a condition for each number bet.
                betPayouts[capturedNumber.ToString()] = 36; // The payout for a winning number bet is 36:1.
            }

        }

        public void PlaceBet(string betType, int amount)
        {
            if (amount > _initialBalance)
            {
                return; // Do not place the bet if the amount is greater than the balance, this can be handled in the UI later on.
            }

            _activeBets.Add((betType, amount)); // Add the bet to the Tuple list.
            _initialBalance -= amount; // Deduct the bet amount from the balance.
            _balanceText.text = "Balance: " + _initialBalance + " euros"; // Update the balance text.
        }

        public void CheckWin(int winningNumber)
        {
            int totalWinAmount = 0; // Variable to store the total win amount.
            string _winningsText = "Winning number is " + winningNumber + ", "; // Text to display the winning number details.

            foreach (var bet in _activeBets) // Iterate through the active bets.
            {
                if (betConditions.ContainsKey(bet.Item1) && betConditions[bet.Item1](winningNumber)) // Check if the bet type exists and the condition for a win is met
                {
                    _initialBalance += bet.Item2 * betPayouts[bet.Item1]; // Add the win amount to the balance.
                    totalWinAmount += bet.Item2 * betPayouts[bet.Item1]; // Add the win amount to the total win amount.
                }
            }

            foreach (var condition in betConditions) // Iterate through the bet conditions to display the winning number details.
            {
                if (condition.Value(winningNumber))
                {
                    _winningsText += condition.Key + " "; // Add the bet type to the text. 
                }
            }

            _winningsText = _winningsText.TrimEnd(','); // Remove trailing comma.
            _winningsText += "number"; // Add the word "number" to the end of the text.

            _activeBets.Clear(); // Clear the active bets list after checking the wins.
            _winningNumberDetails.text = _winningsText; // Display the winning number details.
            _balanceText.text = "Balance: " + _initialBalance + " euros"; // Update the balance text.
            _totalWinAmountText.text = "You won " + totalWinAmount + " euros"; // Display the total win amount.
            StartCoroutine(ResetTexts()); // Reset the texts after a certain time.
        }

        IEnumerator ResetTexts()
        {
            yield return new WaitForSeconds(_textOnScreenTime); // Wait for the specified time before resetting the texts, needs animation later on.
            _totalWinAmountText.text = "";
            _winningNumberDetails.text = "";
        }
    }
}
