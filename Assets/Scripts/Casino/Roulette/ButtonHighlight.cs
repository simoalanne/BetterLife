using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Casino.Roulette
{
    public class ButtonHighlight : MonoBehaviour
    {
        private Dictionary<string, GameObject> _betObjects = new();
        private Dictionary<string, List<string>> _betCategories = new();
        private Dictionary<string, Coroutine> _activeCoroutines = new();
        private Dictionary<string, Color> _originalColors = new();

        private void Awake()
        {
            foreach (Transform child in GameObject.Find("BettingTable").transform)
            {
                _betObjects.Add(child.name, child.gameObject);
            }

            InitializeBetCategories();
        }

        private void InitializeBetCategories()
        {
            // Initialize your bet categories with corresponding bet names
            _betCategories["Left column"] = new List<string> { "1", "4", "7", "10", "13", "16", "19", "22", "25", "28", "31", "34" };
            _betCategories["Middle column"] = new List<string> { "2", "5", "8", "11", "14", "17", "20", "23", "26", "29", "32", "35" };
            _betCategories["Right column"] = new List<string> { "3", "6", "9", "12", "15", "18", "21", "24", "27", "30", "33", "36" };
            _betCategories["1st 12"] = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12" };
            _betCategories["2nd 12"] = new List<string> { "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24" };
            _betCategories["3rd 12"] = new List<string> { "25", "26", "27", "28", "29", "30", "31", "32", "33", "34", "35", "36" };
            _betCategories["Red"] = new List<string> { "1", "3", "5", "7", "9", "12", "14", "16", "18", "19", "21", "23", "25", "27", "30", "32", "34", "36" };
            _betCategories["Black"] = new List<string> { "2", "4", "6", "8", "10", "11", "13", "15", "17", "20", "22", "24", "26", "28", "29", "31", "33", "35" };
            _betCategories["Odd"] = new List<string> { "1", "3", "5", "7", "9", "11", "13", "15", "17", "19", "21", "23", "25", "27", "29", "31", "33", "35" };
            _betCategories["Even"] = new List<string> { "2", "4", "6", "8", "10", "12", "14", "16", "18", "20", "22", "24", "26", "28", "30", "32", "34", "36" };
            _betCategories["1 to 18"] = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18" };
            _betCategories["19 to 36"] = new List<string> { "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31", "32", "33", "34", "35", "36" };
        }

        public void HandlePointerEnter(GameObject selectedObject)
        {
            ProcessBetNames(selectedObject, true);
        }

        public void HandlePointerExit(GameObject selectedObject)
        {
            ProcessBetNames(selectedObject, false);
        }

        void ProcessBetNames(GameObject selectedObject, bool isSelecting)
        {
            if (selectedObject.name.Contains(" and ")) // If the bet is a multibet
            {
                string[] betNames = selectedObject.name.Split(" and ");
                foreach (string betName in betNames)
                {
                    ChangeBetColor(betName, isSelecting);
                }
            }
            else if (_betCategories.ContainsKey(selectedObject.name)) // If the bet is a category bet
            {
                foreach (string betName in _betCategories[selectedObject.name])
                {
                    ChangeBetColor(betName, isSelecting);
                }

                ChangeBetColor(selectedObject.name, isSelecting); // Change the color of the category button as well
            }
            else
            {
                ChangeBetColor(selectedObject.name, isSelecting);
            }
        }
        void ChangeBetColor(string betName, bool isSelecting)
        {
            if (!_betObjects.TryGetValue(betName, out GameObject bet))
            {
                Debug.LogError($"Bet object with name {betName} not found");
                return;
            }

            var button = bet.GetComponentInChildren<Button>();
            if (button != null)
            {
                var image = button.GetComponent<Image>();
                if (isSelecting)
                {
                    if (!_activeCoroutines.ContainsKey(betName))
                    {
                        // Store the original color if not already stored
                        if (!_originalColors.ContainsKey(betName))
                        {
                            _originalColors[betName] = image.color;
                        }
                        Color originalColor = image.color;
                        Coroutine coroutine = StartCoroutine(FlashColor(button, originalColor));
                        _activeCoroutines.Add(betName, coroutine);
                    }
                }
                else
                {
                    if (_activeCoroutines.TryGetValue(betName, out Coroutine coroutine))
                    {
                        StopCoroutine(coroutine);
                        _activeCoroutines.Remove(betName);
                        // Reset to the original color stored when the coroutine started
                        image.color = _originalColors[betName];
                    }
                }
            }
        }

        private IEnumerator FlashColor(Button button, Color originalColor)
        {
            Image buttonImage = button.GetComponent<Image>();
            Color highlightColor = new Color(
                Mathf.Clamp(originalColor.r + 0.5f, 0, 1),
                Mathf.Clamp(originalColor.g + 0.5f, 0, 1),
                Mathf.Clamp(originalColor.b + 0.5f, 0, 1),
                originalColor.a
            );

            float transitionSpeed = 1.33f; // Adjust this to control the speed of the transition
            float t = 0;

            while (true)
            {
                t += Time.deltaTime * transitionSpeed;
                buttonImage.color = Color.Lerp(originalColor, highlightColor, Mathf.PingPong(t, 1));
                yield return null;
            }
        }
    }
}
