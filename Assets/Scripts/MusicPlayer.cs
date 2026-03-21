using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
    [Range(0.0f, 2.0f)] public float musicSpeed = 1.0f;
    public float speedIncrease = 0.01f;
    [Range(0.0f, 2.0f)] public float maxMusicSpeed = 2.0f;
    [Range(0.0f, 1.0f)] public float volumeMult = 1.0f;
    AudioSource source;
    [SerializeField] AudioMixer mixer;

    void Start()
    {
        source = GetComponent<AudioSource>();
        UpdateVolume();
        StartCoroutine(SpeedIncrease());
    }

    void Update()
    {
        UpdateVolume();
    }

    void UpdateVolume()
    {
        source.volume = SettingsData.masterVolume * SettingsData.musicVolume * volumeMult;
    }

    IEnumerator SpeedIncrease()
    {
        WaitForSeconds delay = new(0.25f);
        while (musicSpeed < maxMusicSpeed)
        {
            yield return delay;
            musicSpeed = Mathf.Min(musicSpeed + speedIncrease, maxMusicSpeed);
            source.pitch = musicSpeed;
            mixer.SetFloat("pitch", 1.0f / musicSpeed);
        }
    }
}
