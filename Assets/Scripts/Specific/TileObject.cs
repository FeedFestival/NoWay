using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileObject : MonoBehaviour
{
    [HideInInspector]
    public Tile Tile;

    [SerializeField]
    private TileState _state;
    public TileState State
    {
        get
        {
            if (Tile == null)
                Tile = new Tile();
            _state = Tile.State;
            return _state;
        }
        set { Tile.State = value; }
    }

    [SerializeField]
    private int _x;
    public int X
    {
        get
        {
            _x = Tile.X;
            return _x;
        }
        set
        {
            _x = value;
            if (Tile == null)
                Tile = new Tile();
            Tile.X = _x;
        }
    }

    [SerializeField]
    private int _y;
    public int Y
    {
        get
        {
            _y = Tile.Y;
            return _y;
        }
        set
        {
            _y = value;
            if (Tile == null)
                Tile = new Tile();
            Tile.Y = _y;
        }
    }
    
    public SpriteRenderer Sprite;
    
    public void Init(Tile tile, int x, int y)
    {
        if (tile == null)
            tile = new Tile();
        Tile = tile;
        X = x;
        Y = y;

        Tile.x = transform.position.x;
        Tile.y = transform.position.y;
        Tile.z = transform.position.z;
    }
}
