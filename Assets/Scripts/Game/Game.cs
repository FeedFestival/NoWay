using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{

    private static Game _game;
    public static Game Instance
    {
        get { return _game; }
    }

    void Awake()
    {
        _game = this;
    }

    /*
         * ---------------------------------------------------------------------
         * * ---------------------------------------------------------------------
         * * ---------------------------------------------------------------------
         */

    public bool DebugThis;
    public bool Vibrate;
    private bool _isDebugingMap;

    public Text DebugText;

    public Sphere Sphere;

    public List<Dir> Directions;

    [SerializeField]
    public Tile[,] Tiles;

    public TileObject[,] TileObjects;

    public void Init()
    {
        Directions = new List<Dir>();

        if (DebugThis == false)
        {
            // Garbage Collect
            if (DebugText)
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

        // Debug Options
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            _isDebugingMap = !_isDebugingMap;
            Main.Instance.SceneController.MapController.Debug_ShowTiles();
        }

        if (_isDebugingMap)
        {
            if (Input.GetKeyUp(KeyCode.Alpha1))
            {
                Main.Instance.SceneController.MapController.Debug_SaveMapSettings(Main.Instance.SceneController.CurrentMap);
            }
        }

        // send event continuously
        if (Directions.Count > 0 && Sphere.Moving == false)
            StartCoroutine(
                Sphere.Go(Directions[0], OnMovementStart, OnMovementComplete, OnNoMove)
                );
    }

    public void OnMovementStart()
    {
        if (Sphere.SphereReveries == null)
            return;

        if (Sphere.SphereReveries.IsAtRest)
            StartCoroutine(Sphere.SphereReveries.Ignition(Directions[0]));
    }

    public void OnMovementComplete()
    {
        if (Directions.Count > 0)
        {
            GoLean();
        }
        else
        {
            // return to orginal position.

            if (Sphere.SphereReveries == null)
                return;

            if (Sphere.SphereReveries.IsLeaned)
                Sphere.SphereReveries.AtRest();
        }
    }

    public void OnNoMove()
    {
        // TODO:
        // try to move but return.
    }

    private void GoLean()
    {
        if (Sphere.SphereReveries == null)
            return;

        //Debug.Log(Directions[0] + " != " + Sphere.SphereReveries.LeanDir);
        if (Directions[0] != Sphere.SphereReveries.LeanDir && Sphere.SphereReveries.IsLeaned)
        {
            // switch to a new direction.
            StartCoroutine(Sphere.SphereReveries.Ignition(Directions[0]));
        }
        // else keep the current direction
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
        SoundController.Instance.Play();

        if (Vibrate)
            Vibration.Vibrate(milliseconds);
    }
}

public enum Dir
{
    None, Up, Right, Down, Left
}