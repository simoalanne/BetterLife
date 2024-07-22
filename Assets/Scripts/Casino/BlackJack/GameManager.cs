using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Casino;
using Audio;

public class GameManager : MonoBehaviour
{
    public Button dealButton;
    public Button hitButton;
    public Button standButton;
    public Button betButton;
    public Button backButton;

    public Sprite[] chipSprites;
    public GameObject[] chipObjects;

    // Access the player and dealers script
    public PlayerScript playerScript;
    public PlayerScript dealerScript;

    private int standClicks = 0;

    // Public text to modify hud texts
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI dealerScoreText;
    public TextMeshProUGUI betsText;
    public TextMeshProUGUI cashText;
    public TextMeshProUGUI mainText; // Will alert the player: running out of money, round over etc.

    // Card hiding dealers 2nd card
    public GameObject hideCard;
    // How much is the bet
    int pot = 0;
    int placedChips = 0;
    private SoundEffectPlayer _soundEffectPlayer;

    // Start is called before the first frame update
    void Start()
    {
        cashText.text = "Money: " + playerScript.GetMoney().ToString() + "€";
        dealButton.onClick.AddListener(() => DealClicked());
        hitButton.onClick.AddListener(() => HitClicked());
        standButton.onClick.AddListener(() => StandClicked());
        betButton.onClick.AddListener(() => BetClicked());
        hitButton.gameObject.SetActive(false);
        standButton.gameObject.SetActive(false);
        _soundEffectPlayer = FindObjectOfType<SoundEffectPlayer>();
    }
    private void DealClicked()
    {
        if (pot == 0)
        {
            return;
        }
        // Reset Round, hide text, prep for new hand
        playerScript.ResetHand();
        dealerScript.ResetHand();
        // Hide dealer hand score at the start of dealing 
        mainText.gameObject.SetActive(false);
        dealerScoreText.gameObject.SetActive(false);
        GameObject.Find("Deck").GetComponent<DeckScript>().Shuffle();
        playerScript.StartHand();
        dealerScript.StartHand();
        // Update the score displayed
        scoreText.text = "Hand: " + playerScript.handValue.ToString();
        dealerScoreText.text = "Hand: " + dealerScript.handValue.ToString();
        // Enable to hide one of the dealers cards
        hideCard.GetComponent<Renderer>().enabled = true;
        // Adjust buttons visibility to not allow button presses during dealing
        dealButton.gameObject.SetActive(false);
        hitButton.gameObject.SetActive(true);
        standButton.gameObject.SetActive(true);
        betButton.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);
    }

    private void HitClicked()
    {
        if (playerScript.cardIndex <= 8)
        {
            playerScript.GetCard();
            scoreText.text = "Hand: " + playerScript.handValue.ToString();
            if (playerScript.handValue > 20)
            {
                RoundOver();
            }
        }
    }

    private void StandClicked()
    {
        standClicks++;
        if (standClicks > 1)
        {
            RoundOver();
        }
        HitDealer();
    }

    private void BetClicked()
    {
        float incomingBet = GameObject.Find("BetSizeUI").GetComponent<BetSizeManager>().CurrentBetSize;
        int chipIndex = GameObject.Find("BetSizeUI").GetComponent<BetSizeManager>().CurrentChipIndex;

        if (mainText.gameObject.activeSelf)
        {
            mainText.gameObject.SetActive(false);
        }

        if (incomingBet > playerScript.GetMoney() || placedChips >= 20)
        {
            return;
        }
        _soundEffectPlayer.PlaySoundEffect(Random.Range(0, _soundEffectPlayer.AudioClipCount));
        backButton.gameObject.SetActive(false);
        pot += (int)(incomingBet * 2);
        betsText.text = "Bets: " + (pot / 2).ToString() + "€";
        playerScript.AdjustMoney((int)-incomingBet);
        cashText.text = "Money: " + playerScript.GetMoney().ToString() + "€";
        chipObjects[placedChips].GetComponent<SpriteRenderer>().sprite = chipSprites[chipIndex];
        chipObjects[placedChips].GetComponent<SpriteRenderer>().enabled = true;
        placedChips++;

    }

    private void HitDealer()
    {
        while (dealerScript.handValue < 16 && dealerScript.cardIndex < 10)
        {
            dealerScript.GetCard();
            dealerScoreText.text = "Hand: " + dealerScript.handValue.ToString();
            if (dealerScript.handValue > 20)
            {
                RoundOver();
            }
        }
    }

    // Check for winner and loser, hand is over
    private void RoundOver()
    {
        // Booleans for bust and blackjack/21
        bool playerBust = playerScript.handValue > 21;
        bool dealerBust = dealerScript.handValue > 21;
        bool player21 = playerScript.handValue == 21;
        bool dealer21 = dealerScript.handValue == 21;
        // If stand has been clicked less than twice, no 21s or busts, quit function
        if (standClicks < 2 && !playerBust && !dealerBust && !player21 && !dealer21)
        {
            return;
        }
        bool roundOver = true;
        // All bust, bets returned
        if (playerBust && dealerBust)
        {
            mainText.text = "All Bust: Bets Returned.";
            playerScript.AdjustMoney(pot / 2);
        }
        // Check for dealer win
        else if (playerBust || (!dealerBust && dealerScript.handValue > playerScript.handValue))
        {
            mainText.text = "The House Wins.";
        }
        // Chack for player win
        else if (dealerBust || playerScript.handValue > dealerScript.handValue)
        {
            mainText.text = "The Player Wins.";
            playerScript.AdjustMoney(pot);
        }
        // Check for tie
        else if (playerScript.handValue == dealerScript.handValue)
        {
            mainText.text = "Push: Bets Returned.";
            playerScript.AdjustMoney(pot / 2);
        }
        else
        {
            roundOver = false;
        }
        // Set up ui for next move/hand/turn
        if (roundOver)
        {
            hitButton.gameObject.SetActive(false);
            standButton.gameObject.SetActive(false);
            dealButton.gameObject.SetActive(true);
            betButton.gameObject.SetActive(true);
            mainText.gameObject.SetActive(true);
            dealerScoreText.gameObject.SetActive(true);
            hideCard.GetComponent<Renderer>().enabled = false;
            cashText.text = "Money: " + playerScript.GetMoney().ToString() + "€";
            standClicks = 0;
            pot = 0;
            betsText.text = "Bets: " + pot.ToString() + "€";
            placedChips = 0;
            backButton.gameObject.SetActive(true);
            for (int i = 0; i < chipObjects.Length; i++)
            {
                chipObjects[i].GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }
}
