using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : MonoBehaviour
{
    public Tile CurrentTile;

    private Tile _toMoveTo;

    private float _animationTime;
    public bool Moving;

    public void Init(float tileSizeX, TileObject tileObject = null, Tile tile = null)
    {
        if (tileObject)
        {
            CurrentTile = tileObject.Tile;
            transform.position = Game.Instance.TileObjects[0, 0].transform.position;
        }
        else
        {
            CurrentTile = tile;
            transform.position = Game.Instance.Tiles[0, 0].Position;
        }

        transform.localScale = new Vector3(tileSizeX, tileSizeX, 1);
        
        _animationTime = 0.3f;

        CameraFollow.Instance.Init(transform.localScale);
    }

    public IEnumerator Go(Dir dir)
    {
        bool NoMove = false;

        var newPos = GetNewPos(dir, ref NoMove);

        // TODO: HERE WE DO VALIDATIONS !!!

        if (NoMove)
            yield break;
        
        //Debug.Log(CurrentTile.Position + "- " + newPos);
        LeanTween.move(gameObject, newPos, _animationTime).setEase(LeanTweenType.linear);

        Moving = true;
        
        yield return new WaitForSeconds(_animationTime);
        
        CurrentTile = _toMoveTo;
        Moving = false;
    }

    private Vector3 GetNewPos(Dir dir, ref bool noMove)
    {
        Vector3 newPos;
        switch (dir)
        {
            case Dir.Up:
                newPos = GetTilePos(0, -1);
                if (newPos == Vector3.zero)
                    noMove = true;
                break;
            case Dir.Right:
                newPos = GetTilePos(+1, 0);
                if (newPos == Vector3.zero)
                    noMove = true;
                break;
            case Dir.Down:
                newPos = GetTilePos(0, +1);
                if (newPos == Vector3.zero)
                    noMove = true;
                break;
            case Dir.Left:
                newPos = GetTilePos(-1, 0);
                if (newPos == Vector3.zero)
                    noMove = true;
                break;
            default:
                throw new ArgumentOutOfRangeException("dir", dir, null);
        }
        return newPos;
    }

    private Vector3 GetTilePos(int x, int y)
    {
        if (CurrentTile.X + x < 0 || CurrentTile.Y + y < 0 
            || CurrentTile.X + x >= Game.Instance.HorizontalTileCount || CurrentTile.Y + y >= Game.Instance.VerticalTileCount)
            return Vector3.zero;

        if (Main.Instance.SaveMemory)
        {
            var tile = Game.Instance.Tiles[CurrentTile.X + x, CurrentTile.Y + y];
            if (tile != null)
            {
                _toMoveTo = tile;
                return tile.Position;
            }
            return Vector3.zero;
        }
        else
        {
            var tile = Game.Instance.TileObjects[CurrentTile.X + x, CurrentTile.Y + y];
            if (tile != null)
            {
                _toMoveTo = tile.Tile;
                return tile.transform.position;
            }
            return Vector3.zero;
        }
    }
}
