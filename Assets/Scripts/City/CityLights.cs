using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace City
{
    public class CityLights : MonoBehaviour
    {
        [SerializeField, Range(0.01f, 1f)]
        private float lightsTurnOnIntensity = 0.15f;

        private Light2D _globalLight;
        private List<Light2D> _cityLights;
        private bool _lightsOn = true;
        private GameTimer _gameTimer;

        private void Start()
        {
            // Current setup assumes that global light is stored in parent object and all other lights are children
            var allLights = GetComponentsInChildren<Light2D>();
            _globalLight = allLights.First();
            _cityLights = allLights.Skip(1).ToList();
            _gameTimer = Services.GameTimer;
        }

        private void Update()
        {
            var currentIntensity = _gameTimer.LightIntensity;
            _globalLight.intensity = currentIntensity;
            ToggleCityLights(currentIntensity <= lightsTurnOnIntensity);
        }

        private void ToggleCityLights(bool on)
        {
            if (_lightsOn == on) return;
            _lightsOn = on;
            _cityLights.ForEach(l => l.enabled = on);
        }
    }
}
