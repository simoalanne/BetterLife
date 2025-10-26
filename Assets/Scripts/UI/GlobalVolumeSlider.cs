using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary> Can be used to control the global volume of the game. </summary>
    public class GlobalVolumeSlider : MonoBehaviour
    {
        private Slider _volumeSlider;
        private TMP_Text _volumeText;

        private void Awake()
        {
            _volumeSlider = GetComponent<Slider>();
            _volumeText = GetComponentInChildren<TMP_Text>();
            SetVolume(Preferences.Volume);
            _volumeSlider.onValueChanged.AddListener(SetVolume);
        }
        
        private void SetVolume(float value)
        {
            _volumeText.text = Mathf.RoundToInt(value * 100).ToString();
            _volumeSlider.value = value;
            AudioListener.volume = value;
            Preferences.Volume = value;
        }
    }
}