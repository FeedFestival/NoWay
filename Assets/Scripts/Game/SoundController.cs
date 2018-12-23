using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{

    public AudioSource AudioSource;

    private static SoundController _soundManager;
    public static SoundController Instance
    {
        get { return _soundManager; }
    }

    void Awake()
    {
        _soundManager = this;

        AudioSource = GetComponent<AudioSource>();
    }

    public void Play()
    {
        // stop menu music
        StartCoroutine(FadeOut(AudioSource, 0.02f));

        // start game music
        StartCoroutine(FadeIn(AudioSource, 0.1f));
    }

    public const float maxVol = 0.15f;

    public static IEnumerator FadeOut(AudioSource audioSource, float fadeTime)
    {
        audioSource.volume = maxVol;
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeTime;

            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }

    public static IEnumerator FadeIn(AudioSource audioSource, float fadeTime)
    {
        float startVolume = 0.05f;

        audioSource.volume = 0;
        audioSource.Play();

        while (audioSource.volume < maxVol)
        {
            audioSource.volume += startVolume * Time.deltaTime / fadeTime;

            yield return null;
        }

        audioSource.volume = maxVol;
    }
}
