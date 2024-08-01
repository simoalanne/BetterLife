using System.Collections;
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
    public GameObject hideCard;
    public Sprite[] chipSprites;
    public GameObject[] chipObjects;
    [SerializeField] private GameObject flyingCardPrefab;
    [SerializeField] private float cardTravelTime = 2f;
    private int playerCardIndex = 0;
    private int dealerCardIndex = 0;
    // Access the player and dealers script
    public PlayerScript playerScript;
    public PlayerScript dealerScript;
    private bool standClicked = false;

    // Public text to modify hud texts
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI dealerScoreText;
    public TextMeshProUGUI betsText;
    public TextMeshProUGUI cashText;
    public TextMeshProUGUI mainText; // Will alert the player: running out of money, round over etc.

    // Card hiding dealers 2nd card
    // How much is the bet
    int totalBet = 0;
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
        if (totalBet == 0)
        {
            return;
        }
        // Reset Round, hide text, prep for new hand
        playerScript.ResetHand();
        dealerScript.ResetHand();
        // Hide dealer hand score at the start of dealing 
        mainText.gameObject.SetActive(false);
        dealerScoreText.gameObject.SetActive(false);
        FindObjectOfType<DeckScript>().Shuffle();
        StartCoroutine(DealCardAnimation(2, true));
        StartCoroutine(DealCardAnimation(2, false));
        // Update the score displayed
        scoreText.text = "Hand: " + playerScript.handValue.ToString();
        // Enable to hide one of the dealers cards
        // Adjust buttons visibility to not allow button presses during dealing
        dealButton.gameObject.SetActive(false);
        hitButton.gameObject.SetActive(true);
        standButton.gameObject.SetActive(true);
        betButton.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);
    }

    private void HitClicked()
    {
        StartCoroutine(DealCardAnimation(1, true));
    }

    private void StandClicked()
    {
        standClicked = true;
        standButton.gameObject.SetActive(false);
        hitButton.gameObject.SetActive(false);
        hideCard.SetActive(false);
        StartCoroutine(HitDealer());
    }

    IEnumerator DealCardAnimation(int amountToDealAtOnce, bool isPlayer, float delayBetweenCards = 0.1f)
    {
        hitButton.interactable = false;
        for (int i = 0; i < amountToDealAtOnce; i++)
        {
            yield return new WaitForSeconds(delayBetweenCards);
            float timer = 0;
            var targetTransform = isPlayer ? playerScript.hand[playerCardIndex].transform : dealerScript.hand[dealerCardIndex].transform;
            var currentTransform = transform;
            if (isPlayer)
            {
                playerCardIndex++;
            }
            else
            {
                dealerCardIndex++;
            }
            var card = Instantiate(flyingCardPrefab, transform.position, Quaternion.identity);
            while (timer < cardTravelTime)
            {
                timer += Time.deltaTime;
                card.transform.position = Vector3.Lerp(currentTransform.position, targetTransform.position, timer / cardTravelTime);
                yield return null;
            }
            card.transform.position = targetTransform.position;
            Destroy(card);
            if (isPlayer)
            {
                playerScript.GetCard();
                scoreText.text = "Hand: " + playerScript.handValue.ToString();
            }
            else
            {
                if (amountToDealAtOnce > 1)
                {
                    hideCard.SetActive(true);
                }
                dealerScript.GetCard();
                dealerScoreText.gameObject.SetActive(true);
                var handValue = hideCard.activeSelf ? dealerScript.handValue - dealerScript.hand[0].GetComponent<CardScript>().Value : dealerScript.handValue;
                dealerScoreText.text = "Dealer: " + handValue.ToString();
            }
            if (playerScript.handValue >= 21 || dealerScript.handValue >= 21) // check for bust or blackjack
            {
                RoundOver();
            }
            hitButton.interactable = true;
        }
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
        totalBet += (int)incomingBet;
        betsText.text = "Bet: " + totalBet.ToString() + "€";
        playerScript.AdjustMoney((int)-incomingBet);
        cashText.text = "Money: " + playerScript.GetMoney().ToString() + "€";
        chipObjects[placedChips].GetComponent<SpriteRenderer>().sprite = chipSprites[chipIndex];
        chipObjects[placedChips].GetComponent<SpriteRenderer>().enabled = true;
        placedChips++;

    }

    private IEnumerator HitDealer()
    {
        while (dealerScript.handValue < 17 && dealerScript.cardIndex < 10)
        {
            yield return StartCoroutine(DealCardAnimation(1, false));
        }
        RoundOver();
    }

    // Check for winner and loser, hand is over
    private void RoundOver()
    {

        // Booleans for bust and blackjack/21
        bool playerBust = playerScript.handValue > 21;
        bool dealerBust = dealerScript.handValue > 21;
        bool player21 = playerScript.handValue == 21;
        bool dealer21 = dealerScript.handValue == 21;

        hideCard.SetActive(false);
        // Check for winner
        if (player21 && dealer21 || playerBust && dealerBust || playerScript.handValue == dealerScript.handValue)
        {
            mainText.text = "Push!";
        }
        if (dealerBust || (player21 && !dealer21) || (!playerBust && playerScript.handValue > dealerScript.handValue))
        {
            mainText.text = "You win!";
            playerScript.AdjustMoney(totalBet * 2);
        }
        else if (playerBust || (dealer21 && !player21) || dealerScript.handValue > playerScript.handValue)
        {
            mainText.text = "You lose!";
        }

        hitButton.gameObject.SetActive(false);
        standButton.gameObject.SetActive(false);
        dealButton.gameObject.SetActive(true);
        betButton.gameObject.SetActive(true);
        mainText.gameObject.SetActive(true);
        dealerScoreText.gameObject.SetActive(true);
        dealerScoreText.text = "Dealer: " + dealerScript.handValue.ToString();
        cashText.text = "Money: " + playerScript.GetMoney().ToString() + "€";
        standClicked = false;
        totalBet = 0;
        betsText.text = "Bets: " + totalBet.ToString() + "€";
        placedChips = 0;

        backButton.gameObject.SetActive(true);
        playerCardIndex = 0;
        dealerCardIndex = 0;
        for (int i = 0; i < chipObjects.Length; i++)
        {
            chipObjects[i].GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
