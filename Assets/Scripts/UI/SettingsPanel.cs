using System;
using UnityEngine;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _graphicDropdown;
    
    [SerializeField] private Slider _masterVol;
    [SerializeField] private Slider _sfxVol;
    [SerializeField] private Slider _musicVol;
    [SerializeField] private AudioMixer _mainAudioMixer;

    private void OnEnable()
    {
        _masterVol.onValueChanged.AddListener(delegate { ChangeAudioVolume(_masterVol, "MasterVol"); });
        _sfxVol.onValueChanged.AddListener(delegate { ChangeAudioVolume(_sfxVol, "SFXVol"); });
        _musicVol.onValueChanged.AddListener(delegate { ChangeAudioVolume(_musicVol, "MusicVol"); });
        
        _graphicDropdown.onValueChanged.AddListener(delegate { ChangeGraphicQuality(_graphicDropdown); });
    }

    private void OnDisable()
    {
        _masterVol.onValueChanged.RemoveListener(delegate { ChangeAudioVolume(_masterVol, "MasterVol"); });
        _sfxVol.onValueChanged.RemoveListener(delegate { ChangeAudioVolume(_sfxVol, "SFXVol"); });
        _musicVol.onValueChanged.RemoveListener(delegate { ChangeAudioVolume(_musicVol, "MusicVol"); });
        
        _graphicDropdown.onValueChanged.RemoveListener(delegate { ChangeGraphicQuality(_graphicDropdown); });
    }

    private void Start()
    {
        ChangeGraphicQuality(_graphicDropdown);
    }

    private void ChangeGraphicQuality(TMP_Dropdown dropdown)
    {
        QualitySettings.SetQualityLevel(dropdown.value);
    }

    private void ChangeAudioVolume(Slider slider, string channel)
    {
        _mainAudioMixer.SetFloat(channel, Mathf.Log10(slider.value) * 20);
    }
}
