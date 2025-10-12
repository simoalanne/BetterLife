using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    private const string VolumePrefKey = "musicVolume";
    private Slider _volumeSlider;
    private TMP_Text _volumeText;

    private void Awake()
    {
        _volumeSlider = GetComponent<Slider>();
        _volumeText = GetComponentInChildren<TMP_Text>();
        if (!PlayerPrefs.HasKey(VolumePrefKey))
        {
            SetVolume(1);
            return;
        }

        var volume = PlayerPrefs.GetFloat(VolumePrefKey);
        _volumeSlider.value = volume;
        SetVolume(volume);
    }

    public void ChangeVolume() => SetVolume(_volumeSlider.value);


    private void SetVolume(float value)
    {
        _volumeText.text = Mathf.RoundToInt(value * 100).ToString();
        AudioListener.volume = value;
        PlayerPrefs.SetFloat(VolumePrefKey, value);
    }
}
