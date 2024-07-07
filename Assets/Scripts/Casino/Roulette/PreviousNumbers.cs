using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace Casino.Roulette
{
    public class PreviousNumbers : MonoBehaviour
    {
        private string _filePath;

        [SerializeField] private int _howManyToStore = 36; // How many numbers visible in the grid.
        [SerializeField] private GameObject _prevNumberPrefab;
        [SerializeField] private Button _clearPreviousNumbersButton;
        [SerializeField] private TMP_Text _redNumbersPercentage;
        [SerializeField] private TMP_Text _blackNumbersPercentage;
        [SerializeField] private TMP_Text _zeroNumberPercentage;
        private int[] _redNumbers = new int[] { 1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36 };
        private int[] _blackNumbers = new int[] { 2, 4, 6, 8, 10, 11, 13, 15, 17, 20, 22, 24, 26, 28, 29, 31, 33, 35 };

        private List<int> _previousNumbers = new();

        private GridLayoutGroup _gridLayoutGroup;


        void Awake()
        {
            /* Path to the file where the numbers are saved. 
            Save occurs when switching from roulette to another scene or when the application is closed */
            _filePath = Application.persistentDataPath + "/previousNumbers.txt";

            // Save the numbers when scene changes.
            SceneManager.activeSceneChanged += (Scene current, Scene next) =>
            {
                /* Specifically when switching from roulette to another scene.
                Without this line numbers are saved unnecessarily when loading the roulette scene itself. */
                if (current.name == "Roulette")
                {
                    SaveNumbers();
                }
            };

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

        void AddToGrid(int num)
        {
            GameObject n = Instantiate(_prevNumberPrefab, _gridLayoutGroup.transform);
            n.transform.SetAsFirstSibling(); // should be first child so it is shown first
            n.name = num.ToString();
            n.GetComponentInChildren<TMP_Text>().color = _redNumbers.Contains(num) ? Color.red : _blackNumbers.Contains(num) ? Color.white : Color.green;
            n.GetComponentInChildren<TMP_Text>().text = num.ToString();
        }

        public void AddNumber(int number)
        {
            if (_clearPreviousNumbersButton.interactable == false)
            {
                _clearPreviousNumbersButton.interactable = true;
            }

            Debug.Log(_previousNumbers.Count);
            if (_previousNumbers.Count >= _howManyToStore)
            {
                Debug.Log("Removing");
                _previousNumbers.RemoveAt(0); // Remove the oldest number from the list
                Destroy(_gridLayoutGroup.transform.GetChild(transform.childCount - 1).gameObject); // Remove the oldest number from the grid
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
            int redCount = _previousNumbers.Count(n => _redNumbers.Contains(n));
            int blackCount = _previousNumbers.Count(n => _blackNumbers.Contains(n));
            int zeroCount = _previousNumbers.Count(n => n == 0);

            float redPercentage = _previousNumbers.Count > 0 ? (float)Mathf.Round((float)redCount / _previousNumbers.Count * 100) : 0;
            float blackPercentage = _previousNumbers.Count > 0 ? (float)Mathf.Round((float)blackCount / _previousNumbers.Count * 100) : 0;
            float zeroPercentage = _previousNumbers.Count > 0 ? (float)Mathf.Round((float)zeroCount / _previousNumbers.Count * 100) : 0;

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
                Debug.Log("No numbers to save");
                return;
            }

            string directoryPath = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            Debug.Log("Saving to " + _filePath);
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
    }
}
