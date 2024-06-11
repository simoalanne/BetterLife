using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Casino.Roulette
{
    /// <summary>
    /// This class is responsible for instantiating the roulette table with all the bet buttons.
    /// </summary>
    public class CreateBettingTable : MonoBehaviour
    {
        [SerializeField] private Color _betButtonEdgesColor = new(242f / 255f, 210f / 255f, 113f / 255f);
        [SerializeField] private GameObject _betButtons; // The prefab that can be used to create the bet buttons.
        [SerializeField] private GameObject _multibleNumberBetButtons; // The prefab that can be used to create the in between buttons.
        [SerializeField] private Sprite _redAndBlackSprite; // The sprite that can be used to create the red and black buttons.
        private int _specialCaseIndex = 1;

        void Awake()
        {
            Color green = new(0.05f, 0.5f, 0.05f);
            Color black = new(55f / 255f, 55f / 255f, 55f / 255f);
            Color red = new(0.5f, 0, 0);
            Color background = Camera.main.backgroundColor;

            /* Long Tuple list to store the bet type, color, position, and size of the bet buttons. 
            Hardcoded for now but could do some maths to calculate the positions and sizes based on table size.
            Normal bet buttons like the numbers are 100x120 in size and 100 apart from each other.
            Special buttons are adjusted by using the size parameter to scale them accordingly.
            The bet type MUST match the bet type in the RouletteBetHandler class to work. */
            List<(string, Color, Vector2, Vector2)> betColorPositionSize = new()
            {
                ("0", green, new Vector2(0,0), new Vector2(1,3)), // Background is green so this fits
                ("3", red, new Vector2(100,0), new Vector2(1,1)),
                ("6", black, new Vector2(200,0), new Vector2(1,1)),
                ("9", red, new Vector2(300,0), new Vector2(1,1)),
                ("12", black, new Vector2(400,0), new Vector2(1,1)),
                ("15", black, new Vector2(500,0), new Vector2(1,1)),
                ("18", red, new Vector2(600,0), new Vector2(1,1)),
                ("21", black, new Vector2(700,0), new Vector2(1,1)),
                ("24", red, new Vector2(800,0), new Vector2(1,1)),
                ("27", black, new Vector2(900,0), new Vector2(1,1)),
                ("30", red, new Vector2(1000,0), new Vector2(1,1)),
                ("33", black, new Vector2(1100,0), new Vector2(1,1)),
                ("36", red, new Vector2(1200,0), new Vector2(1,1)),
                ("2 to 1", background, new Vector2(1300,0), new Vector2(1,1)),
                ("2", black, new Vector2(100, -120), new Vector2(1,1)),
                ("5", red, new Vector2(200, -120), new Vector2(1,1)),
                ("8", black, new Vector2(300, -120), new Vector2(1,1)),
                ("11", black, new Vector2(400, -120), new Vector2(1,1)),
                ("14", red, new Vector2(500, -120), new Vector2(1,1)),
                ("17", black, new Vector2(600, -120), new Vector2(1,1)),
                ("20", red, new Vector2(700, -120), new Vector2(1,1)),
                ("23", black, new Vector2(800, -120), new Vector2(1,1)),
                ("26", red, new Vector2(900, -120), new Vector2(1,1)),
                ("29", black, new Vector2(1000, -120), new Vector2(1,1)),
                ("32", red, new Vector2(1100, -120), new Vector2(1,1)),
                ("35", black, new Vector2(1200, -120), new Vector2(1,1)),
                ("2 to 1", background, new Vector2(1300, -120), new Vector2(1,1)),
                ("1", red, new Vector2(100, -240), new Vector2(1,1)),
                ("4", black, new Vector2(200, -240), new Vector2(1,1)),
                ("7", red, new Vector2(300, -240), new Vector2(1,1)),
                ("10", black, new Vector2(400, -240), new Vector2(1,1)),
                ("13", black, new Vector2(500, -240), new Vector2(1,1)),
                ("16", red, new Vector2(600, -240), new Vector2(1,1)),
                ("19", red, new Vector2(700, -240), new Vector2(1,1)),
                ("22", black, new Vector2(800, -240), new Vector2(1,1)),
                ("25", red, new Vector2(900, -240), new Vector2(1,1)),
                ("28", black, new Vector2(1000, -240), new Vector2(1,1)),
                ("31", red, new Vector2(1100, -240), new Vector2(1,1)),
                ("34", black, new Vector2(1200, -240), new Vector2(1,1)),
                ("2 to 1", background, new Vector2(1300, -240), new Vector2(1,1)),
                ("1st 12", background, new Vector2(100, -360), new Vector2(4,1)),
                ("2nd 12", background, new Vector2(500, -360), new Vector2(4,1)),
                ("3rd 12", background, new Vector2(900, -360), new Vector2(4,1)),
                ("1-18", background, new Vector2(100, -480), new Vector2(2,1)),
                ("Even", background, new Vector2(300, -480), new Vector2(2,1)),
                ("Red", red, new Vector2(500, -480), new Vector2(2,1)),
                ("Black", black, new Vector2(700, -480), new Vector2(2,1)),
                ("Odd", background, new Vector2(900, -480), new Vector2(2,1)),
                ("19-36", background, new Vector2(1100, -480), new Vector2(2,1)),
            };

            foreach (var (bet, color, position, size) in betColorPositionSize) // Iterate through the Tuple list.
            {
                GameObject betButton = Instantiate(_betButtons, transform); // Instantiate the bet buttons starting from top left all the way to the bottom right.
                betButton.transform.SetParent(transform); // Set the parent of the bet button to the current object to keep the hierarchy clean.

                if (bet == "2 to 1")
                {
                    betButton.name = _specialCaseIndex + " row"; // Do this to match the bet type in the RouletteBetHandler class.
                    betButton.GetComponentInChildren<TMP_Text>().rectTransform.rotation = Quaternion.Euler(0, 0, 90); // Rotate the text to make it vertical as in standard roulette tables.
                    betButton.GetComponentInChildren<TMP_Text>().fontSize = 35; // Decrease the font size to make it fit.
                    betButton.GetComponentInChildren<TMP_Text>().wordSpacing = -15; // Decrease the word spacing to make it fit.
                    _specialCaseIndex++;
                }
                else
                {
                    betButton.name = bet; // Set the name of the bet button to the bet type. This MUST currently match the bet type in the RouletteBetHandler class to work.
                }

                betButton.transform.Find("Button").GetComponent<Image>().color = color; // Set the color of the bet button, so basically just the background color behind the betting text.
                betButton.GetComponent<RectTransform>().anchoredPosition = position; // Set the position of the bet button.
                betButton.GetComponent<RectTransform>().sizeDelta = betButton.GetComponent<RectTransform>().sizeDelta * size; // Set the size of the bet button by scaling the size delta. Don't use localScale as it will mess up the Font sizes for example.

                if (bet != "Red" && bet != "Black") // Red or Black don't have text on them
                {
                    betButton.GetComponentInChildren<TMP_Text>().text = bet; // Set the text of the bet button to the bet type.
                }
                else
                {
                    betButton.GetComponentInChildren<Button>().GetComponent<Image>().sprite = _redAndBlackSprite; // Set the sprite of the red and black buttons.
                }

                foreach (Transform child in betButton.transform)
                {
                    if (child.GetComponent<Button>() == null)
                    {
                        child.GetComponent<Image>().color = _betButtonEdgesColor; // Set the color of the edges of the bet buttons. The button itself has its own color.
                    }
                }
            }

            // Create the buttons for the splits, streets, corners and six lines. "and" is used as a separator to make these easier to handle in other scripts.
            Vector2 startingPoint = new(95, -5);

            // Create vertical rows splits first
            for (int i = 0; i < 36; i += 3) // first row
            {
                MultiBetButtons($"{i} and {i + 3}", startingPoint, new Vector2(10, 110));
                startingPoint += new Vector2(100, 0);
            }

            startingPoint = new Vector2(95, -125); // second row
            for (int i = 0; i < 36; i += 3)
            {
                if (i == 0)
                {
                    MultiBetButtons($"{i} and {i + 3 - 1}", startingPoint, new Vector2(10, 110));
                }
                else
                {
                    MultiBetButtons($"{i - 1} and {i + 3 - 1}", startingPoint, new Vector2(10, 110));
                }

                startingPoint += new Vector2(100, 0);
            }

            startingPoint = new Vector2(95, -245); // third row
            for (int i = 0; i < 36; i += 3)
            {
                if (i == 0)
                {
                    MultiBetButtons($"{i} and {i + 3 - 2}", startingPoint, new Vector2(10, 110));
                }
                else
                {
                    MultiBetButtons($"{i - 2} and {i + 3 - 2}", startingPoint, new Vector2(10, 110));
                }

                startingPoint += new Vector2(100, 0);
            }

            // Create horizontal splits
            Vector2 startingPoint2 = new(105, -115);
            for (int i = 3; i <= 36; i += 3)
            {
                MultiBetButtons($"{i} and {i - 1}", startingPoint2, new Vector2(90, 10));
                startingPoint2 += new Vector2(100, 0);
            }

            startingPoint2 = new Vector2(105, -235);
            for (int i = 3; i <= 36; i += 3)
            {
                MultiBetButtons($"{i - 1} and {i - 2}", startingPoint2, new Vector2(90, 10));
                startingPoint2 += new Vector2(100, 0);
            }

            // Create street bets
            Vector2 startingPoint3 = new(105, -355);
            for (int i = 1; i <= 34; i += 3)
            {
                MultiBetButtons($"{i} and {i + 1} and {i + 2}", startingPoint3, new Vector2(90, 10));
                startingPoint3 += new Vector2(100, 0);
            }

            // special case for 0, 1, 2 and 0, 2, 3
            MultiBetButtons("0 and 2 and 3", new Vector2(95, -115), new Vector2(10, 10));
            MultiBetButtons("0 and 1 and 2", new Vector2(95, -235), new Vector2(10, 10));

            // Create corner bets
            Vector2 startingPoint4 = new(195, -115);
            for (int i = 2; i < 35; i += 3)
            {
                MultiBetButtons($"{i} and {i + 1} and {i + 3} and {i + 4}", startingPoint4, new Vector2(10, 10));
                startingPoint4 += new Vector2(100, 0);
            }

            startingPoint4 = new Vector2(195, -235);
            for (int i = 1; i < 34; i += 3)
            {
                MultiBetButtons($"{i} and {i + 1} and {i + 3} and {i + 4}", startingPoint4, new Vector2(10, 10));
                startingPoint4 += new Vector2(100, 0);
            }

            // Finally create six line bets
            Vector2 startingPoint5 = new(195, -355);
            for (int i = 1; i < 34; i += 3)
            {
                MultiBetButtons($"{i} and {i + 1} and {i + 2} and {i + 3} and {i + 4} and {i + 5}", startingPoint5, new Vector2(10, 10));
                startingPoint5 += new Vector2(100, 0);
            }

            void MultiBetButtons(string betName, Vector2 position, Vector2 Size)
            {
                GameObject multiBetButton = Instantiate(_multibleNumberBetButtons, transform); // Instantiate the bet buttons starting from top left all the way to the bottom right.
                multiBetButton.transform.SetParent(transform);
                multiBetButton.name = betName; // Set the name of the bet button to the bet type. This MUST currently match the bet type in the RouletteBetHandler class to work.
                multiBetButton.GetComponent<RectTransform>().anchoredPosition = position;
                multiBetButton.GetComponent<RectTransform>().sizeDelta = Size;
                multiBetButton.GetComponent<Image>().color = _betButtonEdgesColor;
            }
        }
    }
}
