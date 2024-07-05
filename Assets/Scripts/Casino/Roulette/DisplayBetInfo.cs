using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace Casino.Roulette
{
    public class DisplayBetInfo : MonoBehaviour
    {
        /* Distance between the cursor pointer position (not the cursor itself but the pointer position!) 
        and the info background top side position */
        [Header("Bet Info Settings")]
        [SerializeField] private Vector2 _infoBoxOffsetFromCursor = new(0, -50);
        [Range(0, 100)]
        [SerializeField] private float _infoTextBackgroundPadding = 66f;

        private CanvasGroup _canvasGroup;
        private Image _betInfoBackground;
        private TMP_Text _betInfo;

        /* These are meant to be controlled by an ingame setting */
        [Header("Bet Info Display Settings")]
        [SerializeField] private bool _showBetType = true;
        [SerializeField] private bool _showBetOdds = true;

        public bool ShowBetType { get => _showBetType; set => _showBetType = value; }
        public bool ShowBetOdds { get => _showBetOdds; set => _showBetOdds = value; }

        void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _betInfoBackground = GetComponent<Image>();
            _betInfo = GetComponentInChildren<TMP_Text>();
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
            _betInfoBackground.rectTransform.position = Vector2.zero; 
        }

        void Update()
        {
            // Convert screen position to local position in canvas
            RectTransform canvasRectTransform = _betInfoBackground.canvas.GetComponent<RectTransform>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, Input.mousePosition, _betInfoBackground.canvas.worldCamera, out Vector2 localCursor);
            // Apply the offset and update the position
            _betInfoBackground.rectTransform.anchoredPosition = localCursor + _infoBoxOffsetFromCursor -
                new Vector2(0, _betInfoBackground.rectTransform.sizeDelta.y / 2); // Subtract half of the background height so that the offset is from the top side
        }

        public void SetBetInfo(string betInfo)
        {
            if (!_showBetType && !_showBetOdds)
            {
                Debug.Log("Both bet type and odds set not to be shown");
                return;
            }

            _betInfo.text = "";

            bool isDigit = int.TryParse(betInfo, out int _);
            if (isDigit)
            {
                if (_showBetType)
                {
                    _betInfo.text += $"Straight {betInfo}\n";
                }

                if (_showBetOdds)
                {
                    _betInfo.text += "35:1";
                }
            }
            else if (betInfo.Contains(" and"))
            {
                int totalAndCount = betInfo.Split(" and ").Length;
                string temp = totalAndCount switch

                {
                    2 => "Split\n1:17",
                    3 => "Street\n1:11",
                    4 => "Corner\n1:8",
                    6 => "Six Line\n1:5",
                    _ => "error"
                };

                string[] tempArr = temp.Split("\n");

                if (_showBetType)
                {
                    _betInfo.text += tempArr[0] + "\n";
                }

                if (_showBetOdds)
                {
                    _betInfo.text += tempArr[1];
                }

            }
            else
            {
                string temp = betInfo switch
                {
                    "1st 12" => "1st 12\n1:2",
                    "2nd 12" => "2nd 12\n1:2",
                    "3rd 12" => "3rd 12\n1:2",
                    "Right column" => "Right column\n1:2",
                    "Middle column" => "Middle column\n1:2",
                    "Left column" => "Left column\n1:2",
                    "Red" => "Red\n1:1",
                    "Black" => "Black\n1:1",
                    "Odd" => "Odd\n1:1",
                    "Even" => "Even\n1:1",
                    "1 to 18" => "1 to 18\n1:1",
                    "19 to 36" => "19 to 36\n1:1",
                    _ => "error"
                };

                string[] tempArr = temp.Split("\n");

                if (_showBetType)
                {
                    _betInfo.text += tempArr[0] + "\n";
                }

                if (_showBetOdds)
                {
                    _betInfo.text += tempArr[1];
                }
            }
            CalculateBackgroundSize();
        }

        /// <summary>
        /// Calculates the size of the background based on the text size.
        /// Useful because the text size varies between different bets.
        /// Also adds padding to the size according to the value set in the inspector.
        /// </summary>
        private void CalculateBackgroundSize()
        {
            // Calculate the size of the background based on the text size
            Vector2 textSize = _betInfo.GetPreferredValues();
            Debug.Log(textSize);
            _betInfoBackground.rectTransform.sizeDelta = textSize + new Vector2(_infoTextBackgroundPadding, _infoTextBackgroundPadding); // Add padding to the size
            _canvasGroup.alpha = 1; // Make the background visible after calculating the size
        }

        /// <summary>
        /// Hides the bet info background. Is called when the cursor exits the trigger area.
        /// This is needed because the background would otherwise stay visible when the cursor
        /// completely exits the trigger area.
        /// </summary>
        public void HideBetInfo()
        {
            _canvasGroup.alpha = 0;
        }
    }
}
