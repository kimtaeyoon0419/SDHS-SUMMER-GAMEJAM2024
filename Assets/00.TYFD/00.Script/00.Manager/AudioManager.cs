// # System
using System.Collections;
using System.Collections.Generic;

// # Unity
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string clipName;
    public AudioClip audioClip;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private List<Sound> bgms;
    [SerializeField] private List<Sound> sfxs;

    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayMusic("MainThema");
    }

    public void PlayMusic(string name)
    {
        foreach(var sound in bgms)
        {
            if(name == sound.clipName)
            {
                bgmSource.clip = sound.audioClip;
                bgmSource.Play();
                break;
            }
            else
            {
                Debug.Log("음악을 찾을 수 없습니다.");
            }
        }
    }

    public void PlaySfx(string name)
    {
        foreach(var sound in sfxs)
        {
            if(name == sound.clipName)
            {
                sfxSource.PlayOneShot(sound.audioClip);
            }
            else
            {
                Debug.Log("효과음을 찾을 수 없습니다.");
            }
        }
    }

    public void MusicVolume(float volume)
    {
        bgmSource.volume = volume;
    }

    public void SFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}
