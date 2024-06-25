using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Casino.Roulette
{
    /// <summary>
    /// This class is responsible for handling the bets in the roulette game.
    /// <summary>
    public class RouletteBetHandler : MonoBehaviour
    {
        [Header("Scripts")]
        [SerializeField] private RouletteUIManager _rouletteUIManager;

        private RouletteBet[] _rouletteBets; // Array to store all instances of the RouletteBet scripts.
        private readonly List<(string, float)> _activeBets = new(); // Tuple to store the bet type and amount.
        private readonly List<KeyValuePair<int[], float>> _activeMultiBets = new(); // List to store the multibets and amount.
        private float _balancePlacedInActiveBets; // Variable to store the balance placed in active bets.
        private float _playerBalance; // Variable to store the initial balance.
        private bool _buttonActivationDone = false;
        public float PlayerBalance => _playerBalance; // Property to get the initial balance.
        private readonly List<(string, int)> _betsInOrder = new(); // List to store the bets in order.

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

        /* Dictionary to store the payouts for each bet type.
        Includes the stake which is why the value is one more than the real payout. */
        readonly Dictionary<string, int> betPayouts = new()
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
            { "Corner", 9 },
            { "Street", 12 },
            { "Split", 18 },
            { "Six Line", 6 }
        };

        void Awake()
        {
            _rouletteBets = FindObjectsOfType<RouletteBet>(); // Store references to all instances of the RouletteBet script.

            _playerBalance = Player.PlayerManager.Instance.MoneyInBankAccount; // Get the initial balance from the player manager.
            _rouletteUIManager.SetBalanceAndTotalBetText(_playerBalance, 0); // Set the balance and total bet text.
            for (int i = 0; i <= 36; i++)
            {
                int capturedNumber = i; // Capture the loop variable
                betConditions[capturedNumber.ToString()] = (winningNumber) => winningNumber == capturedNumber; // Add a condition for each number bet.
                betPayouts[capturedNumber.ToString()] = 36;
            }

        }

        public bool PlaceBet(string betType, float amount)
        {
            if (amount > _playerBalance)
            {
                StartCoroutine(_rouletteUIManager.ShowNoBalanceText());
                return false; // Return false if the bet amount is greater than the balance.
            }

            if (_buttonActivationDone == false) // Check if the button activation is done.
            {
                _rouletteUIManager.BetPlaced();
                _buttonActivationDone = true;
            }

            {

            }
            if (betType.Contains(" and ")) // Check if the bet type contains the word " and ", which means it's a multiple number bet.
            {

                string[] betNumbersString = betType.Split(" and ");
                int[] betNumbers = Array.ConvertAll(betNumbersString, int.Parse);
                _activeMultiBets.Add(new KeyValuePair<int[], float>(betNumbers, amount));
                _betsInOrder.Add(("multi", _activeMultiBets.Count - 1)); // Add the bet type and index to the bets in order list.
            }
            else
            {
                _activeBets.Add((betType, amount)); // Add the bet to the Tuple list.
                _betsInOrder.Add(("single", _activeBets.Count - 1)); // Add the bet type and index to the bets in order list.
            }

            _playerBalance -= amount; // Deduct the bet amount from the balance.
            _balancePlacedInActiveBets += amount; // Add the bet amount to the balance placed in active bets.
            _rouletteUIManager.SetBalanceAndTotalBetText(_playerBalance, _balancePlacedInActiveBets); // Set the balance and total bet text.
            return true; // Return true if the bet is placed successfully.
        }

        public void CheckWin(int winningNumber)
        {
            float totalWinAmount = 0; // Variable to store the total win amount.
            string _winningsText = "Winning number is " + winningNumber + ", "; // Text to display the winning number details.

            foreach (var bet in _activeBets) // Iterate through the active bets.
            {
                if (betConditions.ContainsKey(bet.Item1) && betConditions[bet.Item1](winningNumber)) // Check if the bet type exists and the condition for a win is met
                {
                    _playerBalance += bet.Item2 * betPayouts[bet.Item1]; // Add the win amount to the balance.
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

            foreach (var bet in _activeMultiBets) // Iterate through the multiple number bets.
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
                            _playerBalance += bet.Value * betPayouts["Split"];
                            totalWinAmount += bet.Value * betPayouts["Split"];
                            break;
                        case 3:
                            _playerBalance += bet.Value * betPayouts["Street"];
                            totalWinAmount += bet.Value * betPayouts["Street"];
                            break;
                        case 4:
                            _playerBalance += bet.Value * betPayouts["Corner"];
                            totalWinAmount += bet.Value * betPayouts["Corner"];
                            break;
                        case 6:
                            _playerBalance += bet.Value * betPayouts["Six Line"];
                            totalWinAmount += bet.Value * betPayouts["Six Line"];
                            break;
                    }
                }
            }

            StartCoroutine(_rouletteUIManager.DisplayRoundEndTexts(winningNumber, totalWinAmount)); // Display the winning number and winnings.
            ClearBets();
        }

        public void UndoLatestBet()
        {
            if (_betsInOrder.Last().Item1 == "single") // Check if the last bet was a single bet.
            {
                _playerBalance += _activeBets[_betsInOrder.Last().Item2].Item2; // Add the bet amount back to the balance.
                _balancePlacedInActiveBets -= _activeBets[_betsInOrder.Last().Item2].Item2; // Remove the bet amount from the balance placed in active bets.
                _activeBets.RemoveAt(_betsInOrder.Last().Item2); // Remove the bet from the active bets list.
            }
            else
            {
                _playerBalance += _activeMultiBets[_betsInOrder.Last().Item2].Value; // Add the bet amount back to the balance.
                _balancePlacedInActiveBets -= _activeMultiBets[_betsInOrder.Last().Item2].Value; // Remove the bet amount from the balance placed in active bets.
                _activeMultiBets.RemoveAt(_betsInOrder.Last().Item2); // Remove the bet from the active multiple number bets list.
            }

            GameObject lastChip = RouletteBet.AllPlacedChips.Last(); // Get the last chip
            RouletteBet.AllPlacedChips.Remove(lastChip); // Remove the last chip from the list of all placed chips.
            Destroy(lastChip); // Destroy the last chip GameObject
            _betsInOrder.RemoveAt(_betsInOrder.Count - 1); // Remove the bet from the bets in order list.
            _rouletteUIManager.SetBalanceAndTotalBetText(_playerBalance, _balancePlacedInActiveBets); // Set the balance and total bet text.

            if (_betsInOrder.Count == 0) // Disable the buttons since there are no bets to undo anymore.
            {
                _buttonActivationDone = false;
                _rouletteUIManager.NoBetsPlaced();
            }
        }

        void ClearBets()
        {
            _activeBets.Clear(); // Clear the active bets list.
            _activeMultiBets.Clear(); // Clear the active multiple number bets list.
            _balancePlacedInActiveBets = 0; // Reset the balance placed in active bets
            _rouletteUIManager.NoBetsPlaced();
            foreach (var bet in _rouletteBets) // Iterate through the RouletteBet scripts.
            {
                if (bet.BetPlaced)
                {
                    bet.DestroyChips();
                }
            }
            _buttonActivationDone = false; // Reset the button activation.
            _betsInOrder.Clear(); // Clear the bets in order list.
            RouletteBet.AllPlacedChips.Clear(); // Clear the list of all placed chips.
            _rouletteUIManager.SetBalanceAndTotalBetText(_playerBalance, _balancePlacedInActiveBets);
        }

        public void ResetAllBets()
        {
            _playerBalance += _balancePlacedInActiveBets; // Add the balance placed in active bets back to the balance.
            ClearBets(); // Delete the chips.
        }
    }
}
