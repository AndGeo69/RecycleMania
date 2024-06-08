using System;
using System.Collections.Generic;
using UnityEngine;

public static class SimpleSoundPlayer
{
    private static AudioSource _source;

    private static bool isPlaying = false;

    public static AudioSource PlaySound(string soundName, float volume = 1f) {
        return PlaySound(soundName, false, volume);
    }

    public static AudioSource PlaySound(string soundName, bool randomize, float volume = 1f)
    {
        AudioClip soundClip = Resources.Load<AudioClip>("Sounds/" + soundName);
        if (soundClip == null)
        {
            Debug.LogError("Sound clip not found: " + soundName);
            return null;
        }

        if (_source == null)
        {
            _source = new GameObject("SoundPlayer").AddComponent<AudioSource>();
            _source.playOnAwake = false;
            _source.loop = false;
            UnityEngine.Object.DontDestroyOnLoad(_source.gameObject);
        }

        // _source.clip = soundClip;
        _source.volume = volume;

        _source.pitch = UnityEngine.Random.Range(0.8f, 1.5f);
        // _source.volume = UnityEngine.Random.Range(0.5f, 1f); 


        if (!isPlaying) {
            _source.PlayOneShot(soundClip);
            isPlaying = true;
        }

        isPlaying = false;
        return _source;
    }

    public static AudioSource PlayRandomSound(string[] soundNames, float volume = 1f)
    {
        if (soundNames == null || soundNames.Length == 0)
        {
            Debug.LogError("No sound names provided to play.");
            return null;
        }

        int randomIndex = UnityEngine.Random.Range(0, soundNames.Length);
        return PlaySound(soundNames[randomIndex], volume);
    }

    public static AudioSource PlayRandomSound(List<String> soundNames)
    {
        return PlayRandomSound(soundNames.ConvertAll(i => i.ToString()).ToArray());
    }

    public static void PlayWarningSound() {
        PlaySound("warning", 0.7f);
    }
}