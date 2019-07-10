using UnityEngine.Audio;
using UnityEngine;
using System.Collections;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 1;
    [Range(0.1f, 3f)]
    public float pitch = 1;

    public bool loop;

    [HideInInspector]
    public bool IsPlaying = false;

    [HideInInspector]
    public AudioSource source;

    public void Play()
    {
        source.Play();
        IsPlaying = true;
    }

    public void PlayNoReset()
    {
        if (IsPlaying) return;
        source.Play();
        IsPlaying = true;
    }

    public void Stop()
    {
        source.Stop();
        IsPlaying = false;
    }

}
