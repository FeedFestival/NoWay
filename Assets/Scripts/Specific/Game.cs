using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    private static Game _game;
    public static Game Instance
    {
        get { return _game; }
    }

    public int HorizontalTileCount;
    public int VerticalTileCount;

    void Awake()
    {
        _game = this;
    }

    [SerializeField]
    public Tile[,] Tiles;

    public TileObject[,] TileObjects;
}
