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

    public bool IsLean;
    private float _leanAnimationTime;
    private Vector3 _restRotation;
    private float _degreeDecline;

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

        _leanAnimationTime = 0.15f;
        _restRotation = Box3D.transform.eulerAngles;
        _degreeDecline = 20f;
    }

    public IEnumerator Go(Dir dir, Tile toMoveTo)
    {
        var newPos = toMoveTo.Position;
        
        LeanTween.move(gameObject, newPos, _animationTime).setEase(LeanTweenType.linear);

        if (IsLean == false)
        {
            IsLean = true;

            Decline(dir);
        }

        Moving = true;
        
        yield return new WaitForSeconds(_animationTime);

        CurrentTile.State = TileState.Clear;

        OnMoveComplete(toMoveTo);
        
        Moving = false;
    }

    public void Decline(Dir dir, float? animT = null)
    {
        Vector3 vDir;

        if (animT == null)
            animT = _animationTime;

        switch (dir)
        {
            case Dir.Up:
                vDir = new Vector3(_restRotation.x, _restRotation.y, _restRotation.z - _degreeDecline);
                break;
            case Dir.Right:
                vDir = new Vector3(_restRotation.x, _restRotation.y + _degreeDecline, _restRotation.z);
                break;
            case Dir.Down:
                vDir = new Vector3(_restRotation.x, _restRotation.y, _restRotation.z + _degreeDecline);
                break;
            case Dir.Left:
                vDir = new Vector3(_restRotation.x, _restRotation.y - _degreeDecline, _restRotation.z);
                break;
            default:
                throw new ArgumentOutOfRangeException("dir", dir, null);
        }
        
        LeanTween.rotateLocal(Box3D, vDir, _leanAnimationTime);
        

    }

    public void OnRest()
    {
        IsLean = false;

        LeanTween.rotateLocal(Box3D, _restRotation, _leanAnimationTime);
    }

    private void OnMoveComplete(Tile tile)
    {
        CurrentTile = tile;
        CurrentTile.State = TileState.BoxIn;
        X = CurrentTile.X;
        Y = CurrentTile.Y;

        StartCoroutine(CheckAtRest());
    }

    private Vector3 _checkPos;
    float threshold = 0.0f;
    private IEnumerator CheckAtRest()
    {
        _checkPos = transform.position;

        yield return new WaitForSeconds(0.01f);

        Vector3 offset = transform.position - _checkPos;

        if ((offset.x > threshold || offset.y > threshold)
            || (offset.x < threshold || offset.y < threshold))
        {
            
        }
        else
        {
            OnRest();
        }

    }
}
