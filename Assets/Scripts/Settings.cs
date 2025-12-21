using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Settings : MonoBehaviour
{
    [SerializeField] GameObject settingsCont;
    [SerializeField] Slider[] volumeSliders; // in the order: master, music, effects
    public static bool open = false;
    InputActions inputs;

    void Awake()
    {
        inputs = new();
    }

    void Start()
    {
        InitializeSliders();
        InitInputs();
        open = false;
        settingsCont.SetActive(false);
    }

    void OnEnable()
    {
        inputs.Enable();
    }

    void OnDisable()
    {
        inputs.Disable();
    }

    void InitInputs()
    {
        inputs.Player.Pause.performed += ctx => OnPause();
    }

    void InitializeSliders()
    {
        volumeSliders[0].value = SettingsData.masterVolume;
        volumeSliders[1].value = SettingsData.musicVolume;
        volumeSliders[2].value = SettingsData.effectsVolume;
    }

    public void ShowSettings()
    {
        settingsCont.SetActive(true);
        open = true;
    }

    public void HideSettings()
    {
        settingsCont.SetActive(false);
        open = false;
    }

    public void VolumeChanged(int index)
    {
        switch (index)
        {
            case 0:
                SettingsData.masterVolume = volumeSliders[index].value;
                break;
            case 1:
                SettingsData.musicVolume = volumeSliders[index].value;
                break;
            case 2:
                SettingsData.effectsVolume = volumeSliders[index].value;
                break;
        }
    }

    public void OnPause()
    {
        if (open) HideSettings();
    }
}
