using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;


public class AudioPanel : MenuPanel
{
    [SerializeField] Slider sfxVolumeSlider;
    [SerializeField] Slider ambientVolumeSlider;
    [SerializeField] Slider uiVolumeSlider;

    public override void Initialize()
    {
        base.Initialize();

        sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChange);
        ambientVolumeSlider.onValueChanged.AddListener(OnAmbientVolumeChange);
        uiVolumeSlider.onValueChanged.AddListener(OnUIVolumeChange);

        LoadSaved();
    }
    public override void FillFunctions()
    {
        base.FillFunctions();

        if(UIMenu.instance != null)
            GetButton("Back").onClick.AddListener(new UnityAction(() => UIMenu.instance.OpenMenuPanel("Settings")));
        else if(UIGame.instance != null)
            GetButton("Back").onClick.AddListener(new UnityAction(() => UIGame.instance.OpenMenuPanel("Settings0")));

        GetButton("Back").onClick.AddListener(new UnityAction(() => Save()));

    }
    public void LoadSaved()
    {
        var settings = DataManager.instance.GetSavedData().settings;

        sfxVolumeSlider.value = settings.sfxVolume;
        ambientVolumeSlider.value = settings.ambinetVolume;
        uiVolumeSlider.value = settings.uiVolume;
    }
    public void Save()
    {
        var settings = DataManager.instance.GetSavedData().settings;

        settings.sfxVolume = sfxVolumeSlider.value;
        settings.ambinetVolume = ambientVolumeSlider.value;
        settings.uiVolume = uiVolumeSlider.value;

        DataManager.instance.Modify(settings);
    }



    //Volume Sliders
    void OnAmbientVolumeChange(float newVolume)
    {
        SoundManager.instance.SetAmbient(newVolume);
    }
    void OnSFXVolumeChange(float newVolume)
    {
        SoundManager.instance.SetSFX(newVolume);
    }
    void OnUIVolumeChange(float newVolume)
    {
        SoundManager.instance.SetUiVolume(newVolume);
    }
}
