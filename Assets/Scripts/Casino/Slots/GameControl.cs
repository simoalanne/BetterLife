using System.Collections;
using UnityEngine;
using TMPro;

public class GameControl : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI prizeText; // palkintoteksti

    [SerializeField]
    private Row[] rows; // Rivit. Järjestä vasemmalta oikealle

    [SerializeField]
    private Transform handle; // Koneen kahvan transform

    [SerializeField]
    private Animator handleAnimation;

    [SerializeField] private Animator _slotMachineAnimation;

    [SerializeField] private float _howLongToSpin = 3f; // Kuinka kauan kaikkien rivien pyöriminen kestää

    private int prizeValue; // Palkintoarvo

    private bool _handlePulled; // Onko kahvaa vedetty

    private void OnMouseDown() // Klikkaa kahvaa
    {
        if (_handlePulled == false)
        {
            _handlePulled = true;
            StartCoroutine(PullHandle());
        }
    }

    private IEnumerator PullHandle() // Handle pulling and event
    {
        // Reset values on each spin
        _slotMachineAnimation.enabled = false;
        prizeValue = 0;
        prizeText.text = "";

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
                "Strawberry" => 200,
                "Plum" => 400,
                "Pineapple" => 600,
                "Cherry" => 800,
                "Orange" => 1500,
                "Melon" => 3000,
                "Lemon" => 5000,
                "Grapes" => 6000,
                "Seven" => 7000,
                _ => 0
            };
        }
        // Jos kaksi ekaa samaa
        else if (rows[0].StoppedSlot == rows[1].StoppedSlot)
        {
            prizeValue = rows[0].StoppedSlot switch
            {
                "Strawberry" => 100,
                "Plum" => 300,
                "Pineapple" => 500,
                "Cherry" => 700,
                "Orange" => 1000,
                "Melon" => 2000,
                "Lemon" => 4000,
                "Grapes" => 5000,
                "Seven" => 6000,
                _ => 0
            };
        }

        if (prizeValue > 0)
        {
            _slotMachineAnimation.enabled = true;
            prizeText.text = "you won " + prizeValue + " coins!";
        }
        else
        {
            prizeText.text = "No win this time";
        }
        _handlePulled = false;
    }
}