using System;
using System.Collections;
using Helpers;
using UI;
using UnityEngine;

[RequireComponent(typeof(HideableElement))]
public class HUDAttachablePanel : MonoBehaviour
{
    [SerializeField] private bool closesOnMouseClick;
    
    public event Action OnPanelClosed;
    private HideableElement _hideableElement;

    public void Show() => StartCoroutine(ShowPanel());

    protected IEnumerator ShowPanel()
    {
        Services.GameTimer.IsPaused = true;
        Services.InputManager.EnablePlayerInput(false);
        if (closesOnMouseClick)
        {
            StartCoroutine(HideOnMouseClick());
        }
        _hideableElement ??= GetComponent<HideableElement>();
        _hideableElement.Toggle(false, instant: true);
        yield return _hideableElement.ToggleElement(true, _hideableElement.Duration);
    }

    public void Close() => StartCoroutine(ClosePanel());

    public IEnumerator ClosePanel()
    {
        yield return _hideableElement.ToggleElement(false, _hideableElement.Duration);
        Services.GameTimer.IsPaused = false;
        Services.InputManager.EnablePlayerInput(true);
        OnPanelClosed?.Invoke();
        Destroy(gameObject);
    }
    
    private IEnumerator HideOnMouseClick()
    {
        yield return new WaitForEndOfFrame();
        yield return FunctionLibrary.WaitForMouseClick();
        Close();
    }
}

public abstract class HUDAttachablePanel<T> : HUDAttachablePanel
{
    public void Show(T data) => StartCoroutine(ShowPanel(data));
    
    protected abstract IEnumerator ShowPanel(T data);
}


public static class HUDAttachablePanelExtensions
{
    public static T AttachToHUD<T>(this T panelPrefab, Action onPanelClosed = null)
        where T : HUDAttachablePanel
    {
        var panel = UnityEngine.Object.Instantiate(panelPrefab, Services.PlayerHUD.transform);
        panel.OnPanelClosed += onPanelClosed;
        return panel;
    }
}
