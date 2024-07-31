using System.Collections;
using UnityEngine;
using TMPro;

public class DisplayMoney : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text _moneyText;

    [Header("Animation")]
    [SerializeField] private float _updateDuration = 1.5f; // How long the animation takes to update the money amount.

    public void UpdateMoneyText(float previousMoney, float newmoney)
    {
        StartCoroutine(UpdateMoneyAnimation(previousMoney, newmoney));
    }

    private IEnumerator UpdateMoneyAnimation(float previousMoney, float newMoney)
    {
        while (SceneLoader.Instance.IsLoading)
        {
            yield return null;
        }

        float elapsedTime = 0;
        while (elapsedTime < _updateDuration)
        {
            _moneyText.text = Mathf.RoundToInt(Mathf.Lerp(previousMoney, newMoney, elapsedTime / _updateDuration)) + " €";
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _moneyText.text = $"{newMoney} €";
    }
}