using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public bool DebugThis;

    public Text DebugText;

    public Sphere Sphere;
    
    public List<Dir> Directions;
    
    private static GameController _gameController;
    public static GameController Instance
    {
        get { return _gameController; }
    }

    void Awake()
    {
        _gameController = this;
    }
    
    void Start()
    {
        Directions = new List<Dir>();

        if (DebugThis == false)
        {
            // Garbage Collect
            Destroy(DebugText.gameObject);
            DebugText = null;
        }
    }
    
    void Update()
    {
        //  Up
        if (Input.GetKeyDown(KeyCode.W))
        {
            GoUp(true);
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            GoUp(false);
        }

        //  Right
        if (Input.GetKeyDown(KeyCode.D))
        {
            GoRight(true);
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            GoRight(false);
        }

        //  Down
        if (Input.GetKeyDown(KeyCode.S))
        {
            GoDown(true);
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            GoDown(false);
        }

        //  Left
        if (Input.GetKeyDown(KeyCode.A))
        {
            GoLeft(true);
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            GoLeft(false);
        }

        // send event continuously
        if (Directions.Count > 0 && Sphere.Moving == false)
            StartCoroutine(Sphere.Go(Directions[0]));
    }

    private void AddDirection(Dir dir)
    {
        Directions.Add(dir);

        OnDirectionChange();
    }

    private void RemoveDirection(Dir dir)
    {
        Directions.RemoveAll(d => d == dir);
        
        OnDirectionChange();
    }

    private void OnDirectionChange()
    {
        if (DebugThis)
            ShowDirections();
    }

    private void ShowDirections()
    {
        DebugText.text = string.Empty;
        for (int i = Directions.Count - 1; i >= 0; i--)
        {
            DebugText.text = Directions[i] + "[" + i + "] > " + DebugText.text;
        }
    }

    public void GoUp(bool press)
    {
        if (DebugThis)
            Debug.Log("up - " + press);

        if (press)
        {
            FeelIt();

            AddDirection(Dir.Up);
        }
        else
        {
            RemoveDirection(Dir.Up);
        }
    }

    public void GoRight(bool press)
    {
        if (DebugThis)
            Debug.Log("right - " + press);

        if (press)
        {
            FeelIt();

            AddDirection(Dir.Right);
        }
        else
        {
            RemoveDirection(Dir.Right);
        }
    }

    public void GoDown(bool press)
    {
        if (DebugThis)
            Debug.Log("down - " + press);

        if (press)
        {
            FeelIt();

            AddDirection(Dir.Down);
        }
        else
        {
            RemoveDirection(Dir.Down);
        }
    }

    public void GoLeft(bool press)
    {
        if (DebugThis)
            Debug.Log("left - " + press);

        if (press)
        {
            FeelIt();

            AddDirection(Dir.Left);
        }
        else
        {
            RemoveDirection(Dir.Left);
        }
    }
    
    void FeelIt(long milliseconds = 15)
    {
        SoundManager.Instance.Play();

        Vibration.Vibrate(milliseconds);
    }
}

public enum Dir
{
    Up, Right, Down, Left
}