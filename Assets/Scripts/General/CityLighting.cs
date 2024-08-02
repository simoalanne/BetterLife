using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CityLighting : MonoBehaviour
{
    [Header("Lighting Settings")]
    [Range(0.01f, 1f)]
    [SerializeField, Tooltip("The global intensity required to turn the lights on.")]
    private float _lightsTurnOnGlobalLightIntensity = 0.15f;

    private Light2D _globalLight;
    private Light2D[] _cityLights; // Array of all the lights in the city that will be affected by the time of day.
    private bool _lightsOn = true; // Whether the lights are currently on or off.

    void Awake()
    {
        // Get all Light2D components in children and filter out the global light
        _globalLight = GetComponent<Light2D>();
        _cityLights = GetComponentsInChildren<Light2D>();
        _cityLights = _cityLights.Where(light => light != _globalLight).ToArray(); // Filter out the global light.
    }

    void Start()
    {
        if (GameTimer.Instance == null)
        {
            Debug.LogError("GameTimer is missing from the scene.");
        }
        else
        {
            Debug.Log("GameTimer was found in Start method. Setting lighting now.");
            SetLighting();
        }
    }

    void Update()
    {
        if (GameTimer.Instance == null)
        {
            Debug.LogError("GameTimer is missing from the scene.");
            return;
        }

        // Fetch the current light intensity from the GameTimer instance
        float currentLightIntensity = GameTimer.Instance.LightIntensity;
        if (currentLightIntensity == 0) return;
        _globalLight.intensity = currentLightIntensity;

        if (currentLightIntensity <= _lightsTurnOnGlobalLightIntensity && !_lightsOn)
        {
            Debug.Log("City lights are turning on");
            _lightsOn = true;
            ToggleCityLights(true); // Turn the lights on.
        }
        else if (currentLightIntensity > _lightsTurnOnGlobalLightIntensity && _lightsOn)
        {
            Debug.Log("City lights are turning off");
            _lightsOn = false;
            ToggleCityLights(false); // Turn the lights off.
        }
    }

    private void ToggleCityLights(bool turnOn)
    {
        foreach (var light in _cityLights)
        {
            light.enabled = turnOn;
        }
    }

    private void SetLighting()
    {
        float currentLightIntensity = GameTimer.Instance.LightIntensity;

        if (currentLightIntensity <= _lightsTurnOnGlobalLightIntensity)
        {
            _lightsOn = true;
            ToggleCityLights(true); // Turn the lights on.
        }
        else
        {
            _lightsOn = false;
            ToggleCityLights(false); // Turn the lights off.
        }
    }
}
