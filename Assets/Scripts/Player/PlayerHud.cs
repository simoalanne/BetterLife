using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CanvasGroup))]
public class PlayerHUD : MonoBehaviour
{
    public static PlayerHUD Instance { get; private set; }

    private CanvasGroup _hudCanvasGroup;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        _hudCanvasGroup = GetComponent<CanvasGroup>();

        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    void OnActiveSceneChanged(Scene current, Scene next)
    {
        if (next.name == "MainMenu" || next.name == "Roulette" || next.name == "BlackJack" || next.name == "Slots")
        {
            ShowHud(false);
        }
        else if (current.name == "MainMenu" || current.name == "Roulette" || current.name == "BlackJack" || current.name == "Slots")
        {
            ShowHud(true);
        }
    }

    public void ShowHud(bool show)
    {
        if (show)
        {
            _hudCanvasGroup.alpha = 1;
            _hudCanvasGroup.blocksRaycasts = true;
        }
        else
        {
            _hudCanvasGroup.alpha = 0;
            _hudCanvasGroup.blocksRaycasts = false;
        }
    }

    void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }
}
