using System.Collections;
using Helpers;
using TMPro;
using UnityEngine;

public class DisplayMoney : MonoBehaviour
{
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private float updateDuration = 1.5f;

    public void UpdateMoneyText(float previousMoney, float newMoney)
    {
        StartCoroutine(UpdateMoneyAnimation(previousMoney, newMoney));
    }

    private IEnumerator UpdateMoneyAnimation(float previousMoney, float newMoney)
    {
        yield return new WaitUntil(() => !Services.SceneLoader.IsLoading);

        yield return FunctionLibrary.DoOverTime(updateDuration, progress =>
        {
            var currentMoney = Mathf.Lerp(previousMoney, newMoney, progress);
            moneyText.text = $"{Mathf.RoundToInt(currentMoney)} €";
        });

        moneyText.text = $"{newMoney} €";
    }
}
