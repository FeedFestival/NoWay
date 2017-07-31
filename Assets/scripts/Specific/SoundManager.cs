using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource AudioSource;

    private static SoundManager _soundManager;
    public static SoundManager Instance
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
        StartCoroutine(AudioFadeScript.FadeOut(AudioSource, 0.02f));

        // start game music
        StartCoroutine(AudioFadeScript.FadeIn(AudioSource, 0.1f));
    }
}