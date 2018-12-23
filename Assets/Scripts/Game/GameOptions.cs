using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOptions : MonoBehaviour
{
    private static GameOptions _gameOptions;
    public static GameOptions Instance
    {
        get { return _gameOptions; }
    }

    void Awake()
    {
        _gameOptions = this;
    }

    public Color32 White;
    public Color32 AvailableColor;
    public Color32 NotAvailableColor;
    public Color32 DeadZoneColor;
    public Color32 SpecialColor;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
