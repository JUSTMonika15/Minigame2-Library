using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioMixer audioMixer;
    public AudioSource bgmSource, sfxSource, typingSource;
    private Dictionary<string, AudioClip> sfxClips;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Load sfx resources to dictionary
        AudioClip[] sfxs = Resources.LoadAll<AudioClip>("Audio/SFX");
        sfxClips = new Dictionary<string, AudioClip>();
        foreach (AudioClip sfx in sfxs)
        {
            sfxClips[sfx.name] = sfx;
        }

        // Play bgm
        bgmSource.Play();
    }

    public void PlaySFX(string sfxName)
    {
        sfxSource.PlayOneShot(sfxClips[sfxName]);
    }

    public void PlayTyping()
    {
        typingSource.Play();
    }

    public void StopTyping()
    {
        typingSource.Stop();
    }
}