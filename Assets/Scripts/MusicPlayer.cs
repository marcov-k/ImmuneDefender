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
    const string trackRegexString = @"(?:\[(?<data>(?:\((?:(?:[A-Z]#?b?[0-9][A-Z])|(?:Rest))?,(?:[0-9\.]*),(?:[a-z]*)\))+)\](?<rep>[0-9]*))+?";
    Regex trackRegex;
    const string noteRegexString = @"(?:\((?<note>(?:(?:[A-Z]#?b?[0-9][A-Z])|(?:Rest))?),(?<time>[0-9\.]*),(?<dyn>[a-z]*)\))+?";
    Regex noteRegex;
    readonly List<List<Note>> trackNotes = new();
    public float musicSpeed = 90.0f;
    public float speedIncrease = 0.1f;
    public float maxMusicSpeed = 200.0f;
    public float fadeDuration = 0.45f;
    public float volumeMult = 1.0f;
    float musicVolume = 1.0f;
    float maxVolume;

    void Start()
    {
        InitValues();
        UpdateVolume();
        PrepareDicts();
        AddAudioSources();
        PrepareTracks();
        StartPlayers();
    }

    void Update()
    {
        UpdateVolume();
    }

    void UpdateVolume()
    {
        musicVolume = SettingsData.masterVolume * SettingsData.musicVolume * volumeMult;
    }

    void InitValues()
    {
        maxVolume = dynDict["fff"] * volumeMult;
        trackRegex = new(trackRegexString);
        noteRegex = new(noteRegexString);
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
            audioSources[i].ignoreListenerPause = true;
            audioSources[i].Stop();
        }
    }

    void PrepareTracks()
    {
        for (int i = 0; i < tracks.Count; i++)
        {
            trackNotes.Add(new());
            var trackBlocks = trackRegex.Matches(tracks[i]);
            foreach (Match trackBlock in trackBlocks)
            {
                string data = trackBlock.Groups["data"].Value;
                string repsString = trackBlock.Groups["rep"].Value;
                int reps = (repsString != string.Empty) ? Convert.ToInt32(repsString) : 1;
                List<Note> repNotes = new();
                var noteBlocks = noteRegex.Matches(data);
                foreach (Match noteBlock in noteBlocks)
                {
                    string noteName = noteBlock.Groups["note"].Value;
                    string timeString = noteBlock.Groups["time"].Value;
                    string dynString = noteBlock.Groups["dyn"].Value;

                    AudioClip noteClip = null;
                    if (noteName != "Rest" && notesDict.TryGetValue(noteName, out var clip))
                    {
                        noteClip = clip;
                    }

                    float time = (timeString != string.Empty) ? (float)Convert.ToDouble(timeString) : 1.0f;

                    float dyn = 5.5f;
                    if (dynDict.TryGetValue(dynString, out var dynVal))
                    {
                        dyn = dynVal;
                    }

                    Note note = new() { clip = noteClip, time = time, dyn = dyn };
                    repNotes.Add(note);
                }
                for (int j = 0; j < reps; j++)
                {
                    trackNotes[i].AddRange(repNotes);
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
        float time;
        float fadeTime;
        float startVol;
        while (true)
        {
            source.Stop();
            foreach (var note in track)
            {
                timeScale = 60.0f / musicSpeed;
                source.clip = note.clip;
                source.volume = note.dyn * musicVolume / maxVolume;
                noteTime = note.time * timeScale;
                source.Play();
                yield return new WaitForSecondsRealtime(noteTime * (1.0f - fadeDuration));
                time = 0;
                fadeTime = noteTime * fadeDuration;
                startVol = source.volume;
                while (time < fadeTime)
                {
                    source.volume = Mathf.Lerp(startVol, 0.0f, time / fadeTime);
                    time += Time.unscaledDeltaTime;
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
            yield return delay;
            musicSpeed = Mathf.Min(musicSpeed + speedIncrease, maxMusicSpeed);
        }
    }

    private struct Note
    {
        public AudioClip clip;
        public float time;
        public float dyn;
    }
}
