using UnityEngine;
using TMPro;
using Player;

public class TestScript : MonoBehaviour
{
    void Start()
    {
        GetComponent<TMP_Text>().text = $" Balance: {PlayerManager.Instance.MoneyInBankAccount}";
    }
}