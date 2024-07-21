using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Needs : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float _increaseAnimTime = 0.5f;

    [Header("Energy Settings")]
    [SerializeField] private float energy100To0TimeInGameHours = 12f;
    [SerializeField] private float maxEnergy = 500f;
    [SerializeField] private Image energyBar;
    [SerializeField] private TMP_Text energyAmount;
    private readonly bool increasingEnergy = false;
    private float energy100to0Time;
    public Action OnEnergyDepleted;
    private float currentEnergy;

    [Header("Hunger Settings")]
    [SerializeField] private float hunger100To0TimeInGameHours = 12f;
    [SerializeField] private float maxHunger = 500f;
    [SerializeField] private Image hungerBar;
    [SerializeField] private TMP_Text hungerAmount;
    private float hunger100to0Time;
    public Action OnHungerDepleted;
    private float currentHunger;

    void Awake()
    {
        HideEnergyAmount();
        HideHungerAmount();
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

            if (currentEnergy <= 0)
            {
                OnEnergyDepleted?.Invoke();
            }
        }
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
                OnHungerDepleted?.Invoke();
            }


        }
    }

    public void IncreaseEnergy(float amount)
    {
        if (amount < 0) amount = 0;
        else if (amount > maxEnergy) amount = maxEnergy;
        else if (currentEnergy + amount > maxEnergy) amount = maxEnergy - currentEnergy;

        StartCoroutine(IncreaseEnergyAnimation(amount));
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

    public void ShowEnergyAmount() => energyAmount.gameObject.SetActive(true);

    public void HideEnergyAmount() => energyAmount.gameObject.SetActive(false);

    public void ShowHungerAmount() => hungerAmount.gameObject.SetActive(true);

    public void HideHungerAmount() => hungerAmount.gameObject.SetActive(false);
}
