using System;
using System.Collections;
using DialogueSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Needs : MonoBehaviour
{
    public static Needs Instance { get; private set; }

    [Header("Animation Settings")]
    [SerializeField] private float _increaseAnimTime = 2f;

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
    private bool increasingHunger = false;
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
    }

    void Update()
    {
        UpdateEnergy();
        UpdateHunger();
    }

    void UpdateEnergy()
    {
    }

    void TeleportToPlayerBed()
    {
        Debug.Log("Teleporting to bed");
        Services.PlayerManager.DisableInputs();
        Services.PlayerManager.HasPlayerPassedOut = true;
        Services.PlayerManager.LoadToPlayerBed();
        Services.DialogueHandler.OnDialogueEnd -= TeleportToPlayerBed;
    }

    void UpdateHunger()
    {
    }


    IEnumerator OnHungerDepletedCoroutine()
    {

        yield return null;
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

    public void IncreaseHungerBar(float amountPercentage)
    {
        if (hungerDepletedCoroutine != null)
        {
            StopCoroutine(hungerDepletedCoroutine);
            eatWarning.gameObject.SetActive(false);
        }
        StartCoroutine(IncreaseHungerAnimation(amountPercentage / 100 * maxHunger));
    }


    IEnumerator IncreaseHungerAnimation(float amount)
    {
        Debug.Log("Increasing hunger bar by: " + amount);
        float startHunger = currentHunger;
        increasingHunger = true;
        float endHunger = currentHunger + amount;
        endHunger = Mathf.Clamp(endHunger, 0, maxHunger);
        Debug.Log("After clamping the hunger bar will fill to: " + endHunger);
        float timeElapsed = 0;
        while (timeElapsed < _increaseAnimTime)
        {
            timeElapsed += Time.deltaTime;
            currentHunger = Mathf.Lerp(startHunger, endHunger, timeElapsed / _increaseAnimTime);
            hungerBar.fillAmount = currentHunger / maxHunger;
            yield return null;
        }

        currentHunger = endHunger; // Set to exact amount
        hungerBar.fillAmount = currentHunger / maxHunger;
        increasingHunger = false;
        hungerAmount.text = $"{Mathf.RoundToInt(currentHunger)} / {maxHunger}";
    }

    public void ShowEnergyAmount() => energyAmount.gameObject.SetActive(true);

    public void HideEnergyAmount() => energyAmount.gameObject.SetActive(false);

    public void ShowHungerAmount() => hungerAmount.gameObject.SetActive(true);

    public void HideHungerAmount() => hungerAmount.gameObject.SetActive(false);
}
