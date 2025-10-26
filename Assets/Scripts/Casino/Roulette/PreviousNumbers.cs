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
        [Header("Customization")]
        [SerializeField] private Color redColor = new(0.5f, 0, 0);
        [SerializeField] private Color blackColor = new(50f / 255f, 50f / 255f, 50f / 255f);
        [SerializeField] private Color greenColor = new(0, 0.5f, 0);
        
        [SerializeField] private string sceneNameToSaveOn = "Roulette";
        [SerializeField] private int howManyToStore = 32;
        [SerializeField] private GameObject prevNumberPrefab;
        [SerializeField] private Button clearPreviousNumbersButton;
        [SerializeField] private TMP_Text redNumbersPercentage;
        [SerializeField] private TMP_Text blackNumbersPercentage;
        [SerializeField] private TMP_Text zeroNumberPercentage;

        private List<int> _previousNumbers = new();
        private GridLayoutGroup _grid;


        private void Start()
        {
            _filePath = $"{Application.persistentDataPath}/previousNumbers.txt";
            _grid = GetComponentInChildren<GridLayoutGroup>();
            LoadNumbers();
            CalculatePercentages();

            SceneManager.activeSceneChanged += OnActiveSceneChanged;
            Services.RouletteSpinner.OnRoundComplete += AddNumber;
            clearPreviousNumbersButton.onClick.AddListener(ClearNumbers);
        }

        private void LoadNumbers()
        {
            if (!File.Exists(_filePath)) return;

            _previousNumbers = File.ReadAllText(_filePath)
                .Split(',')
                .TakeLast(howManyToStore)
                .Select(n =>
                {
                    var parsed = int.Parse(n);
                    AddToGrid(parsed);
                    return parsed;
                })
                .ToList();
        }

        private void AddToGrid(int num)
        {
            var n = Instantiate(prevNumberPrefab, _grid.transform);
            n.name = num.ToString();
            var isRed = RouletteConstants.OutsideBetsDict[OutsideBet.Red].Numbers.Contains(num);
            var isBlack = RouletteConstants.OutsideBetsDict[OutsideBet.Black].Numbers.Contains(num);
            n.GetComponent<Image>().color = isRed ? redColor : isBlack ? blackColor : greenColor;
            n.GetComponentInChildren<TMP_Text>().text = num.ToString();
        }

        private void AddNumber(int number)
        {
            clearPreviousNumbersButton.interactable = true;

            if (_previousNumbers.Count >= howManyToStore)
            {
                _previousNumbers.RemoveAt(0);
                Destroy(_grid.transform.GetChild(0).gameObject);
            }

            _previousNumbers.Add(number);
            AddToGrid(number);
            CalculatePercentages();
        }

        private void CalculatePercentages()
        {
            var redCount =
                _previousNumbers.Count(n => RouletteConstants.OutsideBetsDict[OutsideBet.Red].Numbers.Contains(n));
            var blackCount = _previousNumbers.Count(n =>
                RouletteConstants.OutsideBetsDict[OutsideBet.Black].Numbers.Contains(n));
            var zeroCount = _previousNumbers.Count(n => n == 0);

            var totalCount = Mathf.Clamp(_previousNumbers.Count, 1, howManyToStore) / 100f;

            var redPercentage = Mathf.RoundToInt(redCount / totalCount);
            var blackPercentage = Mathf.RoundToInt(blackCount / totalCount);
            var zeroPercentage = Mathf.RoundToInt(zeroCount / totalCount);

            redNumbersPercentage.text = $"Red numbers: {redPercentage}%";
            blackNumbersPercentage.text = $"Black numbers: {blackPercentage}%";
            zeroNumberPercentage.text = $"Zero: {zeroPercentage}%";
        }

        private void OnApplicationQuit() => SaveNumbers();

        private void SaveNumbers()
        {
            if (_previousNumbers.Count is 0) return;

            var directoryPath = Path.GetDirectoryName(_filePath)!;

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            File.WriteAllText(_filePath, string.Join(",", _previousNumbers));
        }
        
        public void ClearNumbers()
        {
            _previousNumbers.Clear();

            _grid.transform.Cast<Transform>().ToList().ForEach(child => Destroy(child.gameObject));

            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }

            CalculatePercentages();
        }

        private void OnActiveSceneChanged(Scene current, Scene _)
        {
            if (current.name == sceneNameToSaveOn)
            {
                SaveNumbers();
            }
        }

        private void OnDestroy() => SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }
}
