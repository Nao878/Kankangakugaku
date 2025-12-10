using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private AudioSource bgmAudioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (volumeSlider != null && bgmAudioSource != null)
        {
            volumeSlider.value = bgmAudioSource.volume;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
    }

    void OnVolumeChanged(float value)
    {
        if (bgmAudioSource != null)
        {
            bgmAudioSource.volume = value;
        }
    }
}
