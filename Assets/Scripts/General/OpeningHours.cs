using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Player;

public class OpeningHours : MonoBehaviour
{
    [Header("Opening hours (24h format)")]
    [Range(0, 24)]
    [SerializeField] private int _openFrom;
    [Range(0, 24)]
    [SerializeField] private int _openTo;
    [SerializeField] private GameObject _closedSign;
    private const float SIGN_SCALE_TIME = 0.25f;
    private const float SIGN_FADE_TIME = 0.5f;
    private const float TYPE_SPEED = 0.02f;
    private string OpenFromTwelveHour => _openFrom > 12 ? $"{_openFrom - 12} pm" : $"{_openFrom} am";
    private string OpenToTwelveHour => _openTo > 12 ? $"{_openTo - 12} pm" : $"{_openTo} am";

    public bool IsOpen => GameTimer.Instance.FullHours >= _openFrom && GameTimer.Instance.FullHours < _openTo;

    public void ShowClosedSign()
    {
        GameTimer.Instance.IsPaused = true;
        PlayerManager.Instance.DisableInputs();
        var copy = Instantiate(_closedSign, PlayerHUD.Instance.GetComponent<Canvas>().transform);
        copy.SetActive(false);
        StartCoroutine(ScaleIn(copy));
    }

    private IEnumerator ScaleIn(GameObject closedSign)
    {
        var rectTransform = closedSign.GetComponent<RectTransform>();
        rectTransform.localScale = new Vector3(0, 0, 0);
        closedSign.SetActive(true);
        while (rectTransform.localScale.x < 1)
        {
            rectTransform.localScale += Vector3.one * Time.deltaTime / SIGN_SCALE_TIME;
            yield return null;
        }
        rectTransform.localScale = Vector3.one;
        StartCoroutine(TypeMessage(closedSign));

    }
    private IEnumerator TypeMessage(GameObject closedsign)
    {
        var text = closedsign.GetComponentInChildren<TMP_Text>();
        text.maxVisibleCharacters = 0;
        text.text = $"We're closed!\nOpen from {OpenFromTwelveHour} to {OpenToTwelveHour}";
        while (text.maxVisibleCharacters < text.text.Length)
        {
            text.maxVisibleCharacters++;
            yield return new WaitForSeconds(TYPE_SPEED);
        }
        Debug.Log("Waiting for mouse click");
        StartCoroutine(WaitForMouseClick(closedsign));
    }

    IEnumerator WaitForMouseClick(GameObject closedSign)
    {
        yield return new WaitForSeconds(0.1f); // Wait for a frame to avoid instant click
        while (!Input.GetMouseButtonDown(0))
        {
            yield return null;
        }
        StartCoroutine(FadeAndDestroy(closedSign));
    }

    IEnumerator FadeAndDestroy(GameObject closedSign)
    {
        var color = closedSign.GetComponent<Image>().color;
        while (color.a > 0)
        {
            color.a = 1 - Mathf.Pow(Time.time / SIGN_FADE_TIME, 2);
            yield return null;
        }
        color.a = 0;
        Destroy(closedSign);

        if (TryGetComponent(out DialogueTrigger dialogueTrigger)) // Allows for dialogue to be triggered after the shop is closed
        {
            dialogueTrigger.TriggerDialogue();
            yield break;
        }
        GameTimer.Instance.IsPaused = false;
        PlayerManager.Instance.EnableInputs();
    }
}
