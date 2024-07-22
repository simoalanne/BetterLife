using UnityEngine;
using Player;

public class ShopkeeperInsult : MonoBehaviour, IOptionalSentence
{
    [SerializeField, Tooltip("If player has this much money or less the insult will trigger")] private int _moneyRequiredForInsult = 20;
    private bool _displayOptionalSentence = false;
    
    void Start()
    {
        if (PlayerManager.Instance.MoneyInBankAccount <= 20)
        {
            _displayOptionalSentence = true;
        }
    }
    public bool DisplayOptionalSentence()
    {
        return _displayOptionalSentence;
    }
}
