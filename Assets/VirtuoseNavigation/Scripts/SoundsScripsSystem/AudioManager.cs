using UnityEngine.Audio;
using UnityEngine;
using System;
using System.Collections;

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

    public void PlayNoReset(string name)
    {
        Sound s = GetSound(name);
        s.PlayNoReset();
    }

    public void Stop(string name)
    {
        Sound s = GetSound(name);
        s.Stop();
    }

    public void Play(string name)
    {
        Sound s = GetSound(name);
        s.Play();
    }

    public Sound GetSound(string name)
    {
        return Array.Find(sounds, sound => sound.name == name);
    }

    public void PlayFadeNoReset(string name, float riseLength)
    {
        Sound s = GetSound(name);
        if (s.IsPlaying) return;
        s.IsPlaying = true;
        StopCoroutine("StopFaceCo");
        StartCoroutine("PlayFaceCo", (s, riseLength));
    }

    public void StopFade(string name, float riseLength)
    {
        Sound s = GetSound(name);
        if (!s.IsPlaying) return;
        s.IsPlaying = false;
        StopCoroutine("PlayFaceCo");
        StartCoroutine("StopFaceCo", (s, riseLength));
    }
    private IEnumerator StopFaceCo((Sound s, float dropLength) p)
    {
        Sound s = p.s;
        float dropLength = p.dropLength;

        float initialVolume = s.source.volume;
        float remainingTime = dropLength;
        while (remainingTime >= 0)
        {
            remainingTime -= VRTools.GetDeltaTime();
            s.source.volume = Mathf.Lerp(0f, initialVolume, remainingTime / dropLength);
            yield return null;
        }

        s.source.Stop();
    }

    private IEnumerator PlayFaceCo((Sound s, float riseLength) p)
    {
        Sound s = p.s;
        float riseLength = p.riseLength;
        s.source.Play();


        float initialVolume = s.source.volume;
        float timePassed = 0f;
        while (timePassed <= riseLength)
        {
            timePassed += VRTools.GetDeltaTime();
            s.source.volume = Mathf.Lerp(initialVolume, 1f, timePassed / riseLength);
            yield return null;
        }
    }

}
