using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OptionsMenu : MonoBehaviour {

    public AudioMixer mainMixer;
    public List<AudioSlider> audioSliders = new List<AudioSlider>();

    private void OnDisable()
    {
        SaveVolumes();
    }

    public void SaveVolumes()
    {
        foreach (AudioSlider audioSlider in audioSliders)
        {
            PlayerPrefs.SetFloat(audioSlider.volumeVariable, audioSlider.slider.value);
        }
    }

    public void SetSliderFromPrefs(AudioSlider audioSlider)
    {
        audioSlider.slider.value = PlayerPrefs.GetFloat(audioSlider.volumeVariable, 0f);
        ValueChanged(audioSlider);
    }

    private void Awake()
    {
        foreach(AudioSlider audioSlider in audioSliders)
        {
            SetSliderFromPrefs(audioSlider);
            audioSlider.slider.onValueChanged.AddListener(delegate { ValueChanged(audioSlider); });
        }
    }

    private void ValueChanged(AudioSlider audioSlider)
    {
        audioSlider.percentText.text = (int)(5f * audioSlider.slider.value / 4f + 100f) + "%";
        mainMixer.SetFloat(audioSlider.volumeVariable, audioSlider.slider.value);
    }


}
