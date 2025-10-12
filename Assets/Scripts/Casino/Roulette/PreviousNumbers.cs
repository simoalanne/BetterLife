using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Casino.Roulette
{
    public class PreviousNumbers : MonoBehaviour
    {
        private string _filePath;

        [SerializeField] private int _howManyToStore = 32; // How many numbers visible in the grid.
        [SerializeField] private GameObject _prevNumberPrefab;
        [SerializeField] private Button _clearPreviousNumbersButton;
        [SerializeField] private TMP_Text _redNumbersPercentage;
        [SerializeField] private TMP_Text _blackNumbersPercentage;
        [SerializeField] private TMP_Text _zeroNumberPercentage;

        private List<int> _previousNumbers = new();

        private GridLayoutGroup _gridLayoutGroup;


        void Awake()
        {
            _filePath = Application.persistentDataPath + "/previousNumbers.txt";

            // Save the numbers when scene changes.
            SceneManager.activeSceneChanged += OnActiveSceneChanged;

            _gridLayoutGroup = GetComponentInChildren<GridLayoutGroup>();
            LoadNumbers();

            _clearPreviousNumbersButton.interactable = _previousNumbers.Count > 0;

            CalculatePercentages();
        }

        void LoadNumbers()
        {
            if (!File.Exists(_filePath))
            {
                return;
            }

            string numbers = File.ReadAllText(_filePath);
            string[] numbersArray = numbers.Split(',');

            if (numbersArray.Length > _howManyToStore)
            {
                numbersArray = RemoveExtraNumbers(numbersArray);
            }

            foreach (string number in numbersArray)
            {
                _previousNumbers.Add(int.Parse(number));
            }

            _previousNumbers.ForEach(AddToGrid); // Add the numbers to the grid
        }

        string[] RemoveExtraNumbers(string[] numbersArray)
        {
            int extraNumbers = numbersArray.Length - _howManyToStore;
            return numbersArray.Skip(extraNumbers).ToArray();
        }

        private void AddToGrid(int num)
        {
            var n = Instantiate(_prevNumberPrefab, _gridLayoutGroup.transform);
            n.transform.SetAsFirstSibling(); // should be first child so it is shown first
            n.name = num.ToString();
            var isRed = RouletteConstants.OutsideBetsDict[OutsideBet.Red].Numbers.Contains(num);
            var isBlack = RouletteConstants.OutsideBetsDict[OutsideBet.Black].Numbers.Contains(num);
            n.GetComponentInChildren<TMP_Text>().color = isRed ? Color.red : isBlack ? Color.black : Color.green;
            n.GetComponentInChildren<TMP_Text>().text = num.ToString();
        }

        public void AddNumber(int number)
        {
            _clearPreviousNumbersButton.interactable = true;

            if (_previousNumbers.Count >= _howManyToStore)
            {
                _previousNumbers.RemoveAt(0); // Remove the oldest number from the list
                Destroy(_gridLayoutGroup.transform.GetChild(transform.childCount - 1)
                    .gameObject); // Remove the oldest number from the grid
            }

            _previousNumbers.Add(number);
            AddToGrid(number);
            CalculatePercentages();
        }

        /// <summary>
        /// Calculates the percentages of red and black numbers from the previous numbers.
        /// </summary>
        private void CalculatePercentages()
        {
            var redCount = _previousNumbers.Count(n => RouletteConstants.OutsideBetsDict[OutsideBet.Red].Numbers.Contains(n));
            var blackCount = _previousNumbers.Count(n => RouletteConstants.OutsideBetsDict[OutsideBet.Black].Numbers.Contains(n));
            var zeroCount = _previousNumbers.Count(n => n == 0);
            
            var redPercentage = redCount / (float)_previousNumbers.Count;
            var blackPercentage = blackCount / (float)_previousNumbers.Count;
            var zeroPercentage = zeroCount / (float)_previousNumbers.Count;

            _redNumbersPercentage.text = $"<color=white>Red numbers: </color><color=red>{redPercentage}%</color>";
            _blackNumbersPercentage.text = $"<color=white>Black numbers: </color><color=red>{blackPercentage}%</color>";
            _zeroNumberPercentage.text = $"<color=white>Zero: </color><color=red>{zeroPercentage}%</color>";
        }

        void OnApplicationQuit()
        {
            SaveNumbers();
        }

        void SaveNumbers()
        {
            if (_previousNumbers.Count == 0)
            {
                return;
            }

            string directoryPath = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string numbersToSave = string.Join(",", _previousNumbers);
            using StreamWriter writer = new(_filePath);
            writer.Write(numbersToSave);
        }

        /// <summary>
        /// Removes all previous numbers from the grid and the list,
        /// and deletes the file where the numbers are saved.
        /// </summary>
        public void ClearNumbers()
        {
            _clearPreviousNumbersButton.interactable = false;
            _previousNumbers.Clear();

            foreach (Transform child in _gridLayoutGroup.transform)
            {
                Destroy(child.gameObject);
            }

            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }

            CalculatePercentages();
        }

        void OnActiveSceneChanged(Scene current, Scene next)
        {
            if (current.name == "Roulette")
            {
                SaveNumbers();
            }
        }

        void OnDestroy()
        {
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        }
    }
}
