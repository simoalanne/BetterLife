using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

namespace Casino.Roulette
{
    /// <summary>
    /// This class is responsible for handling the bets in the roulette game.
    /// <summary>
    public class RouletteBetHandler : MonoBehaviour
    {
        [SerializeField] private float _initialBalance = 1000; // The initial balance of the player. Managed in this script for now.
        [SerializeField] private TMP_Text _balanceText; // Text to display the balance.
        [SerializeField] private TMP_Text _totalWinAmountText; // Text to display the total win amount.
        [SerializeField] private TMP_Text _winningNumberDetails; // Text to display the winning number details such as color, odd/even, etc.
        [SerializeField] private TMP_Text _outOfBalanceText; // Text to display when player runs out of balance completely.
        [SerializeField] private float _textOnScreenTime = 5; // Time to display the winning number details and total win amount in the screen after bet settlement.
        [SerializeField] private RouletteSpinner _rouletteSpinner; // Reference to the RouletteSpinner script.
        [SerializeField] private TMP_Text _noBalanceText;
        [SerializeField] private float _noBalanceTextScreenTime = 0.75f;
        [SerializeField] private Button _resetBetsButton;
        [SerializeField] private Button _spinButton;
        [SerializeField] private Button _exitTableButton;
        private readonly List<(string, float)> _activeBets = new(); // Tuple to store the bet type and amount.
        private readonly List<KeyValuePair<int[], float>> _activeMultibleNumberBets = new(); // List to store the multiple number bets such as street, corner, etc.
        private float _balancePlacedInActiveBets; // Variable to store the balance placed in active bets.

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
            { "1 row", (winningNumber) => new int[] { 3, 6, 9, 12, 15, 18, 21, 24, 27, 30, 33, 36 }.Contains(winningNumber) },
            { "2 row", (winningNumber) => new int[] { 2, 5, 8, 11, 14, 17, 20, 23, 26, 29, 32, 35 }.Contains(winningNumber) },
            { "3 row", (winningNumber) => new int[] { 1, 4, 7, 10, 13, 16, 19, 22, 25, 28, 31, 34 }.Contains(winningNumber) },
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
            { "1 row", 3 },
            { "2 row", 3 },
            { "3 row", 3 },
            { "Corner", 8 },
            { "Street", 11 },
            { "Split", 17 },
            { "Six Line", 5 }
        };

        void Awake()
        {
            _outOfBalanceText.gameObject.SetActive(false); // Set the out of balance text to inactive.
            _balanceText.text = "Balance: " + _initialBalance + " euros"; // Set the text to display the initial balance.
            for (int i = 0; i <= 36; i++)
            {
                int capturedNumber = i; // Capture the loop variable
                betConditions[capturedNumber.ToString()] = (winningNumber) => winningNumber == capturedNumber; // Add a condition for each number bet.
                betPayouts[capturedNumber.ToString()] = 36; // The payout for a winning number bet is 36:1.
            }

        }

        public bool PlaceBet(string betType, float amount)
        {
            if (amount > _initialBalance)
            {
                StartCoroutine(ShowNoBalanceText()); // Show a text to inform the player that they don't have enough balance.
                return false; // Return false if the bet amount is greater than the balance.
            }

            if (_resetBetsButton.interactable == false) // Enable the reset bets button when the first bet is placed.
            {
                _resetBetsButton.interactable = true;
            }

            if (_spinButton.interactable == false) // Enable the spin button when the first bet is placed.
            {
                _spinButton.interactable = true;
            }

            if (betType.Contains(" and ")) // Check if the bet type contains the word " and ", which means it's a multiple number bet.
            {

                string[] betNumbersString = betType.Split(" and ");
                int[] betNumbers = Array.ConvertAll(betNumbersString, int.Parse);
                _activeMultibleNumberBets.Add(new KeyValuePair<int[], float>(betNumbers, amount));
            }
            else
            {
                _activeBets.Add((betType, amount)); // Add the bet to the Tuple list.
            }

            _initialBalance -= amount; // Deduct the bet amount from the balance.
            _balanceText.text = "Balance: " + _initialBalance + " euros"; // Update the balance text.
            _balancePlacedInActiveBets += amount; // Add the bet amount to the balance placed in active bets.
            return true; // Return true if the bet is placed successfully.
        }

        IEnumerator ShowNoBalanceText()
        {
            _noBalanceText.gameObject.SetActive(true); // Set the no balance text to active.
            yield return new WaitForSeconds(_noBalanceTextScreenTime); // Wait for the specified time before resetting the text.
            _noBalanceText.gameObject.SetActive(false); // Set the no balance text to inactive.
        }

        public void CheckWin(int winningNumber)
        {
            float totalWinAmount = 0; // Variable to store the total win amount.
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

            foreach (var bet in _activeMultibleNumberBets) // Iterate through the multiple number bets.
            {
                if (bet.Key.Contains(winningNumber)) // Check if the key array contains the winning number.
                {
                    /* Use the length of the key array to determine the bet type. After splitting the bet type,
                    the length of the array will be the number of numbers in the bet type. E.g
                    For example a bet type of 1,2,3 would be length of 3 meaning it's a street bet.
                    Finally get the winnings from the payout dictionary.*/
                    switch (bet.Key.Length)
                    {
                        case 2:
                            _initialBalance += bet.Value * betPayouts["Split"];
                            totalWinAmount += bet.Value * betPayouts["Split"];
                            break;
                        case 3:
                            _initialBalance += bet.Value * betPayouts["Street"];
                            totalWinAmount += bet.Value * betPayouts["Street"];
                            break;
                        case 4:
                            _initialBalance += bet.Value * betPayouts["Corner"];
                            totalWinAmount += bet.Value * betPayouts["Corner"];
                            break;
                        case 6:
                            _initialBalance += bet.Value * betPayouts["Six Line"];
                            totalWinAmount += bet.Value * betPayouts["Six Line"];
                            break;
                    }
                }
            }
            _winningsText = _winningsText.TrimEnd(','); // Remove trailing comma.
            _winningsText += "number"; // Add the word "number" to the end of the text.

            _activeBets.Clear(); // Clear the active bets list after checking the wins.
            _activeMultibleNumberBets.Clear(); // Clear the active multiple number bets list after checking the wins.
            _winningNumberDetails.text = _winningsText; // Display the winning number details.
            _balanceText.text = "Balance: " + _initialBalance + " euros"; // Update the balance text.
            _totalWinAmountText.text = "You won " + totalWinAmount + " euros"; // Display the total win amount.
            _exitTableButton.interactable = true; // Enable the exit table button after the bets are settled.
            StartCoroutine(ResetTexts()); // Reset the texts after a certain time.
            DeleteChips(); // Delete the chips after the bets are settled.
        }

        void DeleteChips()
        {
            foreach (var bet in FindObjectsOfType<RouletteBet>()) // Not optimal, but scene is small so it'll do.
            {
                if (bet.BetPlaced)
                {
                    bet.DestroyChips();
                }
            }

            _resetBetsButton.interactable = false; // Disable the reset bets button.
            _spinButton.interactable = false; // Disable the spin button since there are no bets.
        }

        public void ResetBets()
        {
            _initialBalance += _balancePlacedInActiveBets; // Add the balance placed in active bets back to the balance.
            _balancePlacedInActiveBets = 0; // Reset the balance placed in active bets.
            _balanceText.text = "Balance: " + _initialBalance + " euros"; // Update the balance text.
            _activeBets.Clear(); // Clear the active bets list.
            _activeMultibleNumberBets.Clear(); // Clear the active multiple number bets list.
            DeleteChips(); // Delete the chips.
        }

        IEnumerator ResetTexts()
        {
            yield return new WaitForSeconds(_textOnScreenTime); // Wait for the specified time before resetting the texts, needs animation later on.
            _totalWinAmountText.text = "";
            _winningNumberDetails.text = "";
            _rouletteSpinner.EnableBettingTable(); // Enable the betting table after the texts are reset.
        }

        public bool CheckIfOutOfBalance()
        {
            if (_initialBalance <= 0) // Check if the balance is less than or equal to 0.
            {
                _outOfBalanceText.gameObject.SetActive(true); // Set the out of balance text to active.
                return true; // Return true if the player is out of balance.
            }
            return false; // Return false if the player is not out of balance.
        }
    }
}
