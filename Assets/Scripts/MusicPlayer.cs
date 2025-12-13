using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.Rendering;
using System;
using System.Collections;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] List<string> tracks = new();
    [SerializeField] SerializedDictionary<string, AudioClip> notesDict = new();
    [SerializeField] SerializedDictionary<string, float> dynDict = new();
    readonly List<AudioSource> audioSources = new();
    const string trackRegexString = @"(?:\[\((?<note>(?:[A-Z]#?b?[0-9][A-Z])?),(?<time>[0-9\.]*),(?<dyn>[a-z]*)\)\](?<rep>[0-9]*))+?";
    Regex trackRegex;
    readonly List<List<Note>> trackNotes = new();
    public float musicSpeed = 90.0f;
    public float musicVolume = 1.0f;

    void Start()
    {
        trackRegex = new(trackRegexString);
        AddAudioSources();
        PrepareTracks();
        StartPlayers();
    }

    void AddAudioSources()
    {
        for (int i = 0; i < tracks.Count; i++)
        {
            if (audioSources.Count < i + 1)
            {
                audioSources.Add(gameObject.AddComponent<AudioSource>());
                audioSources[i].playOnAwake = false;
                audioSources[i].Stop();
            }
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

                var dyn = 5.0f;
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
                yield return new WaitForSecondsRealtime(noteTime);
                source.Stop();
            }
        }
    }

    private struct Note
    {
        public AudioClip clip;
        public float time;
        public float dyn;
    }
}
