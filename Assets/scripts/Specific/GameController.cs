using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public enum Dir
    {
        Up, Right, Down, Left
    }

    public Text DebugText;
    public List<Dir> Directions;

    public bool DebugThis;

    // Use this for initialization
    void Start()
    {
        Directions = new List<Dir>();
    }

    // Update is called once per frame
    void Update()
    {
        if (DebugThis && Directions.Count != 0)
        {
            DebugText.text = string.Empty;
            for (int i = Directions.Count - 1; i >= 0; i--)
            {
                DebugText.text = Directions[i] + "[" + i + "] > " + DebugText.text;
            }

            // TODO: !!!!!
            // allways go with Directions[0] as where to go !

            // DebugText.text = Directions[Directions.Count - 1].ToString();
        }

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
    }

    public void GoUp(bool press)
    {
        if (DebugThis)
            Debug.Log("up - " + press);

        if (press)
        {
            FeelIt();

            Directions.Add(Dir.Up);
        }
        else
        {
            Directions.RemoveAll(dir => dir == Dir.Up);
        }
    }

    public void GoRight(bool press)
    {
        if (DebugThis)
            Debug.Log("right - " + press);

        if (press)
        {
            FeelIt();

            Directions.Add(Dir.Right);
        }
        else
        {
            Directions.RemoveAll(dir => dir == Dir.Right);
        }
    }

    public void GoDown(bool press)
    {
        if (DebugThis)
            Debug.Log("down - " + press);

        if (press)
        {
            FeelIt();

            Directions.Add(Dir.Down);
        }
        else
        {
            Directions.RemoveAll(dir => dir == Dir.Down);
        }
    }

    public void GoLeft(bool press)
    {
        if (DebugThis)
            Debug.Log("left - " + press);

        if (press)
        {
            FeelIt();

            Directions.Add(Dir.Left);
        }
        else
        {
            Directions.RemoveAll(dir => dir == Dir.Left);
        }
    }

    void FeelIt(long milliseconds = 25)
    {
        SoundManager.Instance().PlayLetterSound();

        Vibration.Vibrate(milliseconds);
    }
}
