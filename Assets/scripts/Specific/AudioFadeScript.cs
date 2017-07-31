using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class AudioFadeScript
{
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
