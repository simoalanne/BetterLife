using System.Collections;
using UnityEngine;
using TMPro;
using Player;

public class GameControl : MonoBehaviour
{

    [SerializeField]
    private Row[] rows; // Rivit. Järjestä vasemmalta oikealle

    [SerializeField]
    private Transform handle; // Koneen kahvan transform

    [SerializeField]
    private Animator handleAnimation;

    [SerializeField] private Animator _slotMachineAnimation;

    [SerializeField] private float _howLongToSpin = 3f; // Kuinka kauan kaikkien rivien pyöriminen kestää

    [SerializeField] private TMP_Text _winText; // Voittoteksti

    [SerializeField] private TMP_Text _tutorialText; // Ohjeteksti

    [SerializeField] private TMP_Text _balanceText; // Saldo

    [SerializeField] private TMP_Text _betText; // Panos

    private float _betAmount = 50f;

    private int prizeValue; // Palkintoarvo

    private bool _handlePulled; // Onko kahvaa vedetty


    void Awake()
    {
        _tutorialText.gameObject.SetActive(true);
        _betText.text = _betAmount.ToString() + " €";
        _winText.text = "";
    }

    private void Update()
    {
        _balanceText.text = PlayerManager.Instance.MoneyInBankAccount.ToString() + " €";
    }

    private void OnMouseDown() // Klikkaa kahvaa
    {
        if (_handlePulled == false)
        {
            if (PlayerManager.Instance.MoneyInBankAccount < _betAmount)
            {
                _tutorialText.text = "Not enough money!";
                return;
            }

            PlayerManager.Instance.MoneyInBankAccount -= _betAmount;
            _tutorialText.text = "";
            _handlePulled = true;
            StartCoroutine(PullHandle());
        }
    }

    private IEnumerator PullHandle() // Handle pulling and event
    {
        // Reset values on each spin
        _slotMachineAnimation.enabled = false;
        prizeValue = 0;
        _winText.text = "";


        handleAnimation.SetBool("playSpin", true);
        yield return new WaitForSeconds(0.3f);
        handleAnimation.SetBool("playSpin", false);

        // Start all rows rotating
        foreach (var row in rows)
        {
            row.StartRotating();
        }

        yield return new WaitForSeconds(_howLongToSpin); // Wait for the rows to spin for a while

        foreach (var row in rows) // Stop rows one by one, waiting for each to stop before proceeding
        {
            row.Spin = false;
            yield return new WaitUntil(() => row.rowStopped);
        }

        CheckResults();
    }

    private void CheckResults()
    {
        // Jos kaikki symbolit samoja niin voittoo hullusti
        if (rows[0].StoppedSlot == rows[1].StoppedSlot && rows[1].StoppedSlot == rows[2].StoppedSlot)
        {
            prizeValue = rows[0].StoppedSlot switch // newer switch syntax 
            {
                "Strawberry" => 3,
                "Plum" => 4,
                "Pineapple" => 6,
                "Cherry" => 8,
                "Orange" => 10,
                "Melon" => 12,
                "Lemon" => 14,
                "Grapes" => 16,
                "Seven" => 25,
                _ => 0
            };
        }
        // Jos kaksi ekaa samaa
        else if (rows[0].StoppedSlot == rows[1].StoppedSlot)
        {
            prizeValue = rows[0].StoppedSlot switch
            {
                "Strawberry" => 1,
                "Plum" => 3,
                "Pineapple" => 5,
                "Cherry" => 7,
                "Orange" => 1,
                "Melon" => 12,
                "Lemon" => 14,
                "Grapes" => 16,
                "Seven" => 25,
                _ => 0
            };
        }

        if (prizeValue > 0)
        {
            _slotMachineAnimation.enabled = true;
            _winText.text = $"You won {_betAmount * prizeValue} €";
            PlayerManager.Instance.MoneyInBankAccount += _betAmount * prizeValue;
        }
        else
        {
            _winText.text = "No win";
        }
        _handlePulled = false;
    }
}