using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.scripts.utils;
using UnityEngine;

public class Sphere : MonoBehaviour
{
    public Tile CurrentTile;

    public GameObject Sphere3D;

    private Tile _toMoveTo;

    public bool Moving;

    private float _animationTime;
    private BoxController _boxController;

    public void Init(TileObject tileObject = null, Tile tile = null)
    {
        if (tileObject)
        {
            CurrentTile = tileObject.Tile;
            transform.position = Game.Instance.TileObjects[CurrentTile.X, CurrentTile.Y].transform.position;
        }
        else
        {
            CurrentTile = tile;
            if (CurrentTile != null)
                transform.position = CurrentTile.Position;
        }

        transform.localScale = new Vector3(utils.TileSizeX, utils.TileSizeX, 1);

        _animationTime = 0.3f;

        //
        _boxController = Main.Instance.SceneSetup.BoxController;
        CameraFollow.Instance.Init(transform.localScale);
    }

    public IEnumerator Go(Dir dir)
    {
        bool NoMove = false;

        var newPos = GetNewPos(dir, ref NoMove);

        if (NoMove)
            yield break;

        LeanTween.move(gameObject, newPos, _animationTime).setEase(LeanTweenType.linear);

        // this is for when you have no control
        //LeanTween.rotateAround(Sphere3D, Vector3.forward, 360, 2.5f).setLoopClamp();

        var amountToRotate = (360f / 4f);
        switch (dir)
        {
            case Dir.Up:
                LeanTween.rotateAround(Sphere3D, -Vector3.left, amountToRotate, _animationTime);
                break;
            case Dir.Right:
                LeanTween.rotateAround(Sphere3D, -Vector3.up, amountToRotate, _animationTime);
                break;
            case Dir.Down:
                LeanTween.rotateAround(Sphere3D, Vector3.left, amountToRotate, _animationTime);
                break;
            case Dir.Left:
                LeanTween.rotateAround(Sphere3D, Vector3.up, amountToRotate, _animationTime);
                break;
            default:
                throw new ArgumentOutOfRangeException("dir", dir, null);
        }

        Moving = true;

        yield return new WaitForSeconds(_animationTime);

        CurrentTile = _toMoveTo;
        Moving = false;
    }

    private Vector3 GetNewPos(Dir dir, ref bool noMove)
    {
        int x, y;
        switch (dir)
        {
            case Dir.Up:
                x = 0;
                y = -1;
                break;
            case Dir.Right:
                x = +1;
                y = 0;
                break;
            case Dir.Down:
                x = 0;
                y = +1;
                break;
            case Dir.Left:
                x = -1;
                y = 0;
                break;
            default:
                throw new ArgumentOutOfRangeException("dir", dir, null);
        }

        Vector3 newPos = GetTilePos(x, y);
        if (newPos == Vector3.zero)
            noMove = true;

        if (noMove == false)
        {
            noMove = ValidateNewPos(x, y);
        }

        return newPos;
    }

    private Vector3 GetTilePos(int x, int y)
    {
        if (IsWithinGridBounds(x, y) == false)
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

    private bool ValidateNewPos(int x, int y)
    {
        var tile = Game.Instance.Tiles[CurrentTile.X + x, CurrentTile.Y + y];

        if (tile.State == TileState.BoxIn)
        {
            int x2 = 0, y2 = 0;
            if (x == 0)
                y2 = y * 2;
            if (y == 0)
                x2 = x * 2;

            if (IsWithinGridBounds(x2, y2) == false)
                return true;

            var adiacentTile = Game.Instance.Tiles[CurrentTile.X + x2, CurrentTile.Y + y2];

            if (adiacentTile.State != TileState.Clear)
                return true;

            var boxObj = _boxController.Boxes.FirstOrDefault(
                box => 
                box.X == (CurrentTile.X + x)
                && box.Y == (CurrentTile.Y + y)
                );
            if (boxObj != null)
                StartCoroutine(boxObj.Go(GetDirection(x, y), adiacentTile));
        }

        return false;
    }

    private bool IsWithinGridBounds(int x, int y)
    {
        if (CurrentTile.X + x < 0 || CurrentTile.Y + y < 0 || CurrentTile.X + x >= Game.Instance.HorizontalTileCount ||
            CurrentTile.Y + y >= Game.Instance.VerticalTileCount)
            return false;
        return true;
    }

    private Dir GetDirection(int x, int y)
    {
        if (x == 0)
        {
            if (y > 0)
                return Dir.Down;
            return Dir.Up;
        }
        if (x > 0)
            return Dir.Right;
        return Dir.Left;
    }
}
