using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Written by Oleg "tars" Scheglov
// https://github.com/tarsss
// For game Panzer Sim

[System.Serializable]
public enum Sound
{
    button,
    word_button,
    buy,
    error,
    win,
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Range(0, 100)]
    public float generalVolume = 80;
    public SoundAudioClip[] soundAudioClips;

    public List<AudioSource> sourcePool = new List<AudioSource>();

    private void Awake()
    {
        Instance = this;
    }

    public void PlaySound(Sound sound)
    {
        SoundAudioClip soundAudioClip = GetSoundAudioClip(sound);

        if (soundAudioClip.CanPlay())
        {
            AudioSource pooledSource = null;
            bool sourceIsInPool = false;

            foreach (AudioSource clip in sourcePool)
            {
                if (clip == null) continue;

                if (clip.clip == soundAudioClip.audioClip[0])
                {
                    sourceIsInPool = true;
                    pooledSource = clip;
                    break;
                }
            }

            if (sourceIsInPool)
            {
                pooledSource.Play();
            }
            else
            {
                GameObject source = new GameObject("Audio Source");
                DontDestroyOnLoad(source);
                AudioSource audioSource = source.AddComponent<AudioSource>();
                float volume = generalVolume / 100 * soundAudioClip.volume / 100;

                audioSource.clip = soundAudioClip.audioClip[Random.Range(0, soundAudioClip.audioClip.Length)];
                audioSource.volume = volume;

                audioSource.Play();
                soundAudioClip.lastTimePlayed = Time.time;

                if (!sourcePool.Contains(audioSource))
                {
                    sourcePool.Add(audioSource);
                }
            }
        }
    }

    public void PlaySound(int soundIndex)
    {
        SoundAudioClip soundAudioClip = soundAudioClips[soundIndex];
        {
            if (soundAudioClip.CanPlay())
            {
                AudioSource pooledSource = null;
                bool sourceIsInPool = false;

                foreach (AudioSource clip in sourcePool)
                {
                    if (clip == null) continue;

                    if (clip.clip == soundAudioClip.audioClip[0])
                    {
                        sourceIsInPool = true;
                        pooledSource = clip;
                        break;
                    }
                }

                if (sourceIsInPool)
                {
                    pooledSource.Play();
                }
                else
                {
                    GameObject source = new GameObject("Audio Source");
                    DontDestroyOnLoad(source);
                    AudioSource audioSource = source.AddComponent<AudioSource>();
                    float volume = generalVolume / 100 * soundAudioClip.volume / 100;

                    audioSource.clip = soundAudioClip.audioClip[Random.Range(0, soundAudioClip.audioClip.Length)];
                    audioSource.volume = volume;

                    audioSource.PlayOneShot(soundAudioClip.audioClip[Random.Range(0, soundAudioClip.audioClip.Length)], volume);

                    soundAudioClip.lastTimePlayed = Time.time;

                    if (!sourcePool.Contains(audioSource))
                    {
                        sourcePool.Add(audioSource);
                    }
                }
            }
        }
    }

    /*
    public void StopSound(Sound sound)
    {
        AudioSource[] sources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource source in sources)
        {
            SoundAudioClip soundAudioClip = GetSoundAudioClip(sound);
            if (source.clip == soundAudioClip.audioClip[0])
            {
                soundAudioClip.lastTimePlayed = Time.time - soundAudioClip.audioClip[0].length;
                Destroy(source.gameObject);
            }
        }
    }
    */

    private SoundAudioClip GetSoundAudioClip(Sound sound)
    {
        foreach (SoundAudioClip clip in soundAudioClips)
        {
            if (clip.sound == sound)
            {
                return clip;
            }
        }

        Debug.LogError("Sound not found.");
        return null;
    }
}

[System.Serializable]
public class SoundAudioClip
{
    public Sound sound;
    public AudioClip[] audioClip;
    [Range(0, 100)]
    public float volume = 100;

    public bool preventOverlapping;
    [HideInInspector]
    public float lastTimePlayed;

    public bool CanPlay()
    {
        if (preventOverlapping)
        {
            if (Time.time > lastTimePlayed + audioClip[0].length || lastTimePlayed == 0)
            {
                return true;
            }

            else return false;
        }

        else return true;
    }
}
