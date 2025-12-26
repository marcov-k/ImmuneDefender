using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using System.Collections;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] List<string> tracks = new();
    [SerializeField] List<AudioClip> clips = new();
    readonly Dictionary<string, AudioClip> notesDict = new();
    readonly Dictionary<string, float> dynDict = new() { { "ppp", 1.0f }, { "pp", 2.5f }, { "p", 4.0f }, { "mp", 5.5f }, { "mf", 7.0f },
        { "f", 8.5f }, { "ff", 10.0f }, { "fff", 11.5f } };
    readonly List<AudioSource> audioSources = new();
    const string trackRegexString = @"(?:\[\((?<note>(?:[A-Z]#?b?[0-9][A-Z])?),(?<time>[0-9\.]*),(?<dyn>[a-z]*)\)\](?<rep>[0-9]*))+?";
    Regex trackRegex;
    readonly List<List<Note>> trackNotes = new();
    public float musicSpeed = 90.0f;
    public float speedIncrease = 0.1f;
    public float maxMusicSpeed = 200.0f;
    public float fadeDuration = 0.25f;
    public float volumeMult = 1.0f;
    float musicVolume = 1.0f;

    void Start()
    {
        trackRegex = new(trackRegexString);
        PrepareDicts();
        AddAudioSources();
        PrepareTracks();
        StartPlayers();
    }

    void Update()
    {
        musicVolume = SettingsData.masterVolume * SettingsData.musicVolume * volumeMult;
    }

    void PrepareDicts()
    {
        foreach (var clip in clips)
        {
            notesDict.Add(clip.name, clip);
        }
    }

    void AddAudioSources()
    {
        for (int i = 0; i < tracks.Count; i++)
        {
            audioSources.Add(gameObject.AddComponent<AudioSource>());
            audioSources[i].playOnAwake = false;
            audioSources[i].Stop();
        }
    }

    void PrepareTracks()
    {
        for (int i = 0; i < tracks.Count; i++)
        {
            trackNotes.Add(new());
            var notes = trackRegex.Matches(tracks[i]);
            foreach (Match note in notes)
            {
                var noteName = note.Groups["note"].Value;
                var timeS = note.Groups["time"].Value;
                var dynS = note.Groups["dyn"].Value;
                var repS = note.Groups["rep"].Value;

                AudioClip clip = null;
                if (notesDict.TryGetValue(noteName, out var noteClip))
                {
                    clip = noteClip;
                }

                var time = 1.0f;
                if (timeS != string.Empty)
                {
                    time = (float)Convert.ToDouble(timeS);
                }

                var dyn = dynDict["mp"];
                if (dynDict.TryGetValue(dynS, out var noteDyn))
                {
                    dyn = noteDyn;
                }

                int rep = 1;
                if (repS != string.Empty)
                {
                    rep = Convert.ToInt32(repS);
                }

                Note noteObj = new() { clip = clip, time = time, dyn = dyn };
                for (int j = 0; j < rep; j++)
                {
                    trackNotes[i].Add(noteObj);
                }
            }
        }
    }

    void StartPlayers()
    {
        StartCoroutine(SpeedIncrease());
        for (int i = 0; i < trackNotes.Count; i++)
        {
            StartCoroutine(TrackPlayer(trackNotes[i], audioSources[i]));
        }
    }

    IEnumerator TrackPlayer(List<Note> track, AudioSource source)
    {
        float timeScale;
        float noteTime;
        while (true)
        {
            source.Stop();
            foreach (var note in track)
            {
                timeScale = 60.0f / musicSpeed;
                source.clip = note.clip;
                source.volume = note.dyn * musicVolume;
                noteTime = note.time * timeScale;
                source.Play();
                yield return new WaitForSecondsRealtime(noteTime * (1.0f - fadeDuration));
                float time = 0;
                float fadeTime = noteTime * fadeDuration;
                float startVol = source.volume;
                while (time < fadeTime)
                {
                    source.volume = Mathf.Lerp(startVol, 0.0f, time / fadeTime);
                    time += Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }
                source.Stop();
            }
        }
    }

    IEnumerator SpeedIncrease()
    {
        WaitForSeconds delay = new(1.0f);
        while (musicSpeed < maxMusicSpeed)
        {
            musicSpeed = Mathf.Min(musicSpeed + speedIncrease, maxMusicSpeed);
            yield return delay;
        }
    }

    private struct Note
    {
        public AudioClip clip;
        public float time;
        public float dyn;
    }
}
