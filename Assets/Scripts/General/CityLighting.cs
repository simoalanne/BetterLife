using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CityLighting : MonoBehaviour
{
    [Header("Lighting Settings")]
    [Range(0, 24)]
    [SerializeField, Tooltip("The hour when the lights in the city will turn on.")] private int _lightsOnHour = 18;
    [SerializeField] private float _lightTransitionSpeed = 0.5f; // How fast the lights will transition from off to on or on to off.
    private Light2D[] _cityLights; // Array of all the lights in the city that will be affected by the time of day.
    private float[] _cityLightIntensities; // Array of the original intensities of the lights in the city.
    private bool _lightsOn = false; // Whether the lights are currently on or off.
    // Start is called before the first frame update
    void Start()
    {
        _cityLights = GetComponentsInChildren<Light2D>();
        _cityLightIntensities = new float[_cityLights.Length];
        for (int i = 0; i < _cityLights.Length; i++)
        {
            _cityLightIntensities[i] = _cityLights[i].intensity;
        }
    }

    void Update()
    {
        while (GameTimer.Instance.FullHours == _lightsOnHour && !_lightsOn) // Not the most optimal way to do this but it works.
        {
            ToggleCityLights(true);
        }
    }

    void ToggleCityLights(bool turnOn)
    {
        for (int i = 0; i < _cityLights.Length; i++)
        {
            _cityLights[i].intensity = Mathf.Lerp(_cityLights[i].intensity, turnOn ? _cityLightIntensities[i] : 0, _lightTransitionSpeed * Time.deltaTime);
        }
        _lightsOn = turnOn;
    }
}
