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
    
    private void Start()
    {
        ChangeGraphicQuality();
    }

    public void ChangeGraphicQuality()
    {
        QualitySettings.SetQualityLevel(_graphicDropdown.value);
    }

    public void ChangeMasterVolume()
    {
        _mainAudioMixer.SetFloat("MasterVol", _masterVol.value);
    }
    
    public void ChangeMusicVolume()
    {
        _mainAudioMixer.SetFloat("MusicVol", _musicVol.value);
    }
    
    public void ChangeSFXVolume()
    {
        _mainAudioMixer.SetFloat("SFXVol", _sfxVol.value);
    }
}
