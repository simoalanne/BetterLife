using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Player;

public class EndGame : MonoBehaviour
{
    [Header("End Game Settings")]
    [SerializeField] private float _moneyToPay = 100000;
    [Header("Texts")]
    [SerializeField] private TMP_Text _endGameText;
    [Header("Buttons")]
    [SerializeField] private Button _endGameButton;
    [SerializeField] private Button _cancelButton;

    void Awake()
    {
        _endGameButton.onClick.AddListener(PayMoneyAndEndGame);
        _cancelButton.onClick.AddListener(CancelEndGame);
        _endGameText.text = $"Pay {_moneyToPay} to end the game?";
    }

    void OnEnable()
    {
        _endGameButton.interactable = PlayerManager.Instance.MoneyInBankAccount >= _moneyToPay;
    }

    public void PayMoneyAndEndGame()
    {
        PlayerManager.Instance.ResetMoney(); // Reset the money to the original amount.
        Destroy(gameObject);
        Debug.Log("End game.");
    }

    public void CancelEndGame()
    {
        PlayerManager.Instance.EnablePlayerMovement();
        PlayerManager.Instance.EnablePlayerInteract();
        Time.timeScale = 1;
        Destroy(gameObject);
    }
}
