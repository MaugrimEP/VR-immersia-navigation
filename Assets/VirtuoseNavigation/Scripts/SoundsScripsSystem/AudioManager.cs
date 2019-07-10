using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
            

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public void Play(string name)
    {
        Sound s = GetSound(name);
        if (s == null)
        {
            Debug.LogWarning($"Sound {name} not found");
            return;
        }
        if (s.IsPlaying) return;
        s.IsPlaying = true;
        s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = GetSound(name);
        if (s == null)
        {
            Debug.Log($"Sound {name} not found");
            return;
        }
        s.IsPlaying = false;
        s.source.Pause();
    }

    public Sound GetSound(string name)
    {
        return Array.Find(sounds, sound => sound.name == name);
    }
}
