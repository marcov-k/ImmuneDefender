using UnityEngine;
using UnityEngine.UI;


public class Settings : MonoBehaviour
{
    [SerializeField] GameObject settingsCont;
    [SerializeField] Slider[] volumeSliders; // in the order: master, music, effects

    void Start()
    {
        InitializeSliders();
        settingsCont.SetActive(false);
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
    }

    public void HideSettings()
    {
        settingsCont.SetActive(false);
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
}
