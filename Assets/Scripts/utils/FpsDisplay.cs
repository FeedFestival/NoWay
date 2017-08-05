﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FpsDisplay : MonoBehaviour
{
    public bool DebugFps;

    public Text Text;

    float deltaTime = 0.0f;

    void Start()
    {
        StartCoroutine(ShowFps());
    }

    void Update()
    {
        if (DebugFps)
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    }

    IEnumerator ShowFps()
    {
        if (DebugFps == false)
            yield break;

        yield return new WaitForSeconds(1f);

        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        Text.text = text;

        StartCoroutine(ShowFps());
    }
}
