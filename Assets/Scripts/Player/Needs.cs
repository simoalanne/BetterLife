using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Player;

public class Needs : MonoBehaviour
{
    public static Needs Instance { get; private set; }

    [SerializeField] private float timeScaleDebug = 1f;
    [Header("Animation Settings")]
    [SerializeField] private float _increaseAnimTime = 0.5f;

    [Header("Energy Settings")]
    [SerializeField] private float energy100To0TimeInGameHours = 14f;
    [SerializeField] private float maxEnergy = 500f;
    [SerializeField] private Image energyBar;
    [SerializeField] private TMP_Text energyAmount;
    [SerializeField] private TMP_Text passOutWarning;
    [SerializeField] private DialogueTrigger passOutDialogue; // Dialogue to trigger when energy is depleted.

    private readonly bool increasingEnergy = false;
    private float energy100to0Time;
    public Action OnEnergyDepleted;
    private float currentEnergy;

    [Header("Hunger Settings")]
    [SerializeField] private float hunger100To0TimeInGameHours = 12f;
    [SerializeField] private float maxHunger = 500f;
    [SerializeField] private Image hungerBar;
    [SerializeField] private TMP_Text hungerAmount;
    [SerializeField] private TMP_Text eatWarning;
    [SerializeField] private float _timeBeforeHungerDepletedInvoked = 90f;
    private Coroutine hungerDepletedCoroutine;
    private float hunger100to0Time;
    public Action OnHungerDepleted;
    private float currentHunger;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            HideEnergyAmount();
            HideHungerAmount();
            passOutWarning.gameObject.SetActive(false);
            eatWarning.gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        energy100to0Time = GameTimer.Instance.GameMinuteInRealTimeSeconds * 60 * energy100To0TimeInGameHours;
        currentEnergy = maxEnergy;
        energyBar.fillAmount = 1;

        hunger100to0Time = GameTimer.Instance.GameMinuteInRealTimeSeconds * 60 * hunger100To0TimeInGameHours;
        currentHunger = maxHunger;
        hungerBar.fillAmount = 1;
    }

    void Update()
    {
        Time.timeScale = timeScaleDebug;
        UpdateEnergy();
        UpdateHunger();
    }

    void UpdateEnergy()
    {
        if (!GameTimer.Instance.IsPaused && currentEnergy > 0 && !increasingEnergy)
        {
            currentEnergy -= Time.deltaTime / energy100to0Time * maxEnergy;
            energyBar.fillAmount = currentEnergy / maxEnergy;
            energyAmount.text = $"{Mathf.RoundToInt(currentEnergy)} / {maxEnergy}";

            if (currentEnergy <= 0 && !PlayerManager.Instance.HasPlayerPassedOut)
            {
                DialogueManager.Instance.OnDialogueEnd += TeleportToPlayerBed;
                passOutDialogue.TriggerDialogue();
            }
        }
    }

    void TeleportToPlayerBed()
    {
        Debug.Log("Teleporting to bed");
        PlayerManager.Instance.DisableInputs();
        PlayerManager.Instance.HasPlayerPassedOut = true;
        PlayerManager.Instance.LoadToPlayerBed();
        DialogueManager.Instance.OnDialogueEnd -= TeleportToPlayerBed;
    }

    void UpdateHunger()
    {
        if (!GameTimer.Instance.IsPaused && currentHunger > 0)
        {
            currentHunger -= Time.deltaTime / hunger100to0Time * maxHunger;
            hungerBar.fillAmount = currentHunger / maxHunger;
            hungerAmount.text = $"{Mathf.RoundToInt(currentHunger)} / {maxHunger}";

            if (currentHunger <= 0)
            {
                hungerDepletedCoroutine = StartCoroutine(OnHungerDepletedCoroutine());
            }
        }
    }


    IEnumerator OnHungerDepletedCoroutine()
    {
        eatWarning.gameObject.SetActive(true);
        float timeElapsed = 0;
        while (timeElapsed < _timeBeforeHungerDepletedInvoked)
        {
            if (GameTimer.Instance.IsPaused)
            {
                yield return null;
                continue;
            }

            timeElapsed += Time.deltaTime;
            eatWarning.text = $"Dying in: {Mathf.RoundToInt(_timeBeforeHungerDepletedInvoked - timeElapsed)}s";
            yield return null;
        }

        OnHungerDepleted?.Invoke();
    }

    public void MaxOutEnergy()
    {
        Debug.Log("Maxing out energy");
        currentEnergy = maxEnergy;
        Debug.Log(currentEnergy);
        energyBar.fillAmount = 1;
    }


    private IEnumerator IncreaseEnergyAnimation(float amount)
    {
        float startEnergy = currentEnergy;
        float endEnergy = amount;
        float timeElapsed = 0;

        while (timeElapsed < _increaseAnimTime)
        {
            timeElapsed += Time.deltaTime;
            currentEnergy = Mathf.Lerp(startEnergy, endEnergy, timeElapsed / _increaseAnimTime);
            energyBar.fillAmount = currentEnergy / maxEnergy;
            yield return null;
        }

        currentEnergy = amount;
        energyBar.fillAmount = currentEnergy / maxEnergy;
    }

    public void IncreaseHungerBar(float amount)
    {
        if (hungerDepletedCoroutine != null)
        {
            StopCoroutine(hungerDepletedCoroutine);
            eatWarning.gameObject.SetActive(false);
        }
        if (amount < 0) amount = 0;
        else if (amount > maxHunger) amount = maxHunger;
        else if (currentHunger + amount > maxHunger) amount = maxHunger - currentHunger;

        StartCoroutine(IncreaseHungerAnimation(amount));
    }


    IEnumerator IncreaseHungerAnimation(float amount)
    {
        float startHunger = currentHunger;
        float endHunger = amount;
        float timeElapsed = 0;

        while (timeElapsed < _increaseAnimTime)
        {
            timeElapsed += Time.deltaTime;
            currentHunger = Mathf.Lerp(startHunger, endHunger, timeElapsed / _increaseAnimTime);
            hungerBar.fillAmount = currentHunger / maxHunger;
            yield return null;
        }

        currentHunger = amount;
        hungerBar.fillAmount = currentHunger / maxHunger;
    }

    public void ShowEnergyAmount() => energyAmount.gameObject.SetActive(true);

    public void HideEnergyAmount() => energyAmount.gameObject.SetActive(false);

    public void ShowHungerAmount() => hungerAmount.gameObject.SetActive(true);

    public void HideHungerAmount() => hungerAmount.gameObject.SetActive(false);
}
