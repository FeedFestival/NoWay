using System;
using System.Collections;
using System.Collections.Generic;
using Assets.scripts.utils;
using UnityEngine;

public class Box : MonoBehaviour
{
    public Tile CurrentTile;
    
    public int X;
    public int Y;

    public GameObject Box3D;
    
    private float _animationTime;
    public bool Moving;

    public void Init(TileObject tileObject = null, Tile tile = null)
    {
        if (tileObject)
        {
            OnMoveComplete(tileObject.Tile);
            transform.position = Game.Instance.TileObjects[CurrentTile.X, CurrentTile.Y].transform.position;
        }
        else
        {
            OnMoveComplete(tile);
            if (CurrentTile != null)
                transform.position = CurrentTile.Position;
        }

        transform.localScale = new Vector3(utils.TileSizeX, utils.TileSizeX, utils.TileSizeX);

        _animationTime = 0.3f;

        Box3D = gameObject;
    }

    public IEnumerator Go(Dir dir, Tile toMoveTo)
    {
        var newPos = toMoveTo.Position;
        
        LeanTween.move(gameObject, newPos, _animationTime).setEase(LeanTweenType.linear);
        
        Moving = true;

        yield return new WaitForSeconds(_animationTime);

        CurrentTile.State = TileState.Clear;

        OnMoveComplete(toMoveTo);
        
        Moving = false;
    }

    private void OnMoveComplete(Tile tile)
    {
        CurrentTile = tile;
        CurrentTile.State = TileState.BoxIn;
        X = CurrentTile.X;
        Y = CurrentTile.Y;
    }
}
