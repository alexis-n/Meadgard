using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;

public class AudioManager : MonoBehaviour
{

    public static AudioManager instance;

    public AudioMixer audioMixer;
    public AudioBank bank;
    public AudioSource musicSource, ambientSource, sfxSource;
    public AudioSource[] bardSources;
    public bool[] playBards;
    public float musicTransition = 1f, ambientTransition = 1f,
        ambientVolume = 0.5f, musicVolume = 0.5f, sfxVolume = 0.5f;

    private void Awake()
    {
        instance = this;
    }

    public void PlayPrepMusic()
    {
        musicSource.DOFade(0, musicTransition).OnComplete(() => PlayPrepMusicHelper());
        for (int i = 0; i < bardSources.Length; i++)
        {
            bardSources[i].DOFade(0, musicTransition);
        }
    }
    public void PlayPrepMusicHelper()
    {
        musicSource.clip = bank.prepMusic;
        musicSource.volume = 0;
        musicSource.Play();
        musicSource.DOFade(musicVolume, musicTransition);
    }

    public void PlayServiceMusic()
    {
        musicSource.DOFade(0, musicTransition).OnComplete(() => PlayServiceMusicHelper());
    }
    public void PlayServiceMusicHelper()
    {
        musicSource.clip = bank.serviceMusic;
        musicSource.volume = 0;
        musicSource.Play();
        musicSource.DOFade(musicVolume, musicTransition);

        for (int i = 0; i < bardSources.Length; i++)
        {
            if (playBards[i])
            {
                bardSources[i].Play();
                bardSources[i].DOFade(musicVolume, musicTransition);
            }
        }
    }

    public void PlayAmbient(AudioClip clip)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(ambientSource.DOFade(0, ambientTransition)
            .OnComplete(() => ambientSource.clip = clip))
            .AppendCallback(() => ambientSource.Play())
            .Append(ambientSource.DOFade(ambientVolume, ambientTransition));
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.clip = clip;
        sfxSource.volume = sfxVolume;
        sfxSource.Play();
    }

    public void StopAmbient()
    {
        ambientSource.DOFade(0, ambientTransition);
    }

    public void StopMusic()
    {
        musicSource.DOFade(0, musicTransition);
        foreach (var item in bardSources)
        {
            item.DOFade(0, musicTransition);
        }
    }
}
