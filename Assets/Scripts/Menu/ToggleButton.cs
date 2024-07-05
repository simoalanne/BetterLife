using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour
{
    [SerializeField] private float _duration = 0.075f; // Duration should be short for a toggle button
    [SerializeField] private Color _onColor = new(0f, 0.5f, 0f);
    [SerializeField] private Color _offColor = new(0.5f, 0f, 0f);
    [SerializeField] private RectTransform _knobrectTransform;
    private Button _toggleButton;
    private float _amountToMove;
    private bool _isOn = false;

    private void Awake()
    {
        _amountToMove = gameObject.GetComponent<RectTransform>().sizeDelta.x - _knobrectTransform.sizeDelta.x - 10; // 10 is the padding
        _toggleButton = GetComponent<Button>();
        _toggleButton.onClick.AddListener(ToggleSetting);
    }

    public void SetInitialStatus(bool isOn)
    {
        _knobrectTransform.anchoredPosition = new Vector2(5,0); // Initial position is the off position.
        _isOn = isOn; 
        _knobrectTransform.anchoredPosition += new Vector2(_isOn ? _amountToMove : 0, 0); // Move to right if it is on
        _toggleButton.image.color = _isOn ? _onColor : _offColor; // Change color according to the status
    }

    private void ToggleSetting()
    {
        _toggleButton.interactable = false;
        StartCoroutine(MoveButton());
    }

    private IEnumerator MoveButton()
    {
        _isOn = !_isOn;
        float elapsedTime = 0;
        Vector2 startPosition = _knobrectTransform.anchoredPosition;
        Vector2 endPosition = _knobrectTransform.anchoredPosition + new Vector2(_isOn ? _amountToMove : -_amountToMove, 0);

        while (elapsedTime < _duration)
        {
            _knobrectTransform.anchoredPosition = Vector2.Lerp(startPosition, endPosition, elapsedTime / _duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _knobrectTransform.anchoredPosition = endPosition;
        _toggleButton.image.color = _isOn ? _onColor : _offColor;
        _toggleButton.interactable = true;
    }
}
