using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
    public float totalIncreaseTime = 60.0f;
    [Range(0.0f, 1.0f)] public float volumeMult = 1.0f;
    [SerializeField] AudioSource[] sources = new AudioSource[2];
    [SerializeField] List<AudioClip> speedClips = new();

    void Start()
    {
        UpdateVolume();
        StartCoroutine(SpeedIncrease());
    }

    void Update()
    {
        UpdateVolume();
    }

    void UpdateVolume()
    {
        sources[0].volume = SettingsData.masterVolume * SettingsData.musicVolume * volumeMult;
        sources[1].volume = SettingsData.masterVolume * SettingsData.musicVolume * volumeMult;
    }

    IEnumerator SpeedIncrease()
    {
        sources[0].clip = speedClips[0];
        sources[0].Play();
        if (speedClips.Count > 1)
        {
            int currentSource = 0;
            int nextSource = 1;
            float delayTime = totalIncreaseTime / (speedClips.Count - 1.0f);
            var delay = new WaitForSeconds(delayTime);
            float speedIncrease = 1.0f / (speedClips.Count - 1.0f);
            float speedMult = 1.0f + speedIncrease;
            float playingTime = 0.0f + delayTime;
            sources[nextSource].clip = speedClips[1];
            sources[nextSource].time = playingTime * (1.0f / speedMult);
            for (int i = 1; i < speedClips.Count; i++)
            {
                yield return delay;

                (nextSource, currentSource) = (currentSource, nextSource);
                sources[currentSource].Play();

                playingTime += delayTime;
                speedMult += speedIncrease;

                if (i < speedClips.Count - 1)
                {
                    sources[nextSource].clip = speedClips[i + 1];
                    sources[nextSource].time = playingTime * (1.0f / speedMult);
                }
            }
        }
    }
}
