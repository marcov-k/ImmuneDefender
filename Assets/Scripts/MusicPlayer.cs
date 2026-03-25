using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
    static readonly WaitForSecondsRealtime loopDelay = new(0.25f);
    [Range(0.0f, 1.0f)] public float volumeMult = 1.0f;
    AudioSource audioSource;
    [SerializeField] bool increasesSpeed = false;
    [SerializeField] AudioClip speedClip;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        UpdateVolume();
        if (increasesSpeed) StartCoroutine(SpeedIncrease());
    }

    void Update()
    {
        UpdateVolume();
    }

    void UpdateVolume()
    {
        audioSource.volume = SettingsData.masterVolume * SettingsData.musicVolume * volumeMult;
    }

    IEnumerator SpeedIncrease()
    {
        yield return new WaitForSecondsRealtime(audioSource.clip.length);
        audioSource.Stop();
        yield return loopDelay;
        audioSource.clip = speedClip;
        audioSource.Play();
    }
}
