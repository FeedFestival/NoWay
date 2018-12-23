using System.Collections;
using System.Collections.Generic;
using Assets.scripts.utils;
using UnityEngine;

public class Box : MonoBehaviour {

    public Tile CurrentTile;

    public int X;
    public int Y;

    private BoxReveries _boxReveries;

    private float _animationTime;
    public bool Moving;

    public void Init(TileObject tileObject = null, Tile tile = null)
    {
        if (tileObject)
        {
            OnMoveComplete(tileObject.Tile);
            transform.position = GetBoxPosition(Game.Instance.TileObjects[CurrentTile.X, CurrentTile.Y].transform.position);
        }
        else
        {
            OnMoveComplete(tile);
            if (CurrentTile != null)
                transform.position = CurrentTile.Position;
        }
        
        _animationTime = 0.3f;

        _boxReveries = transform.gameObject.GetComponent<BoxReveries>();
        _boxReveries.Init();
    }

    public IEnumerator Go(Dir dir, Tile toMoveTo)
    {
        var newPos = GetBoxPosition(toMoveTo.Position);

        LeanTween.move(gameObject, newPos, _animationTime).setEase(LeanTweenType.linear);

        if (_boxReveries.IsLean == false)
        {
            _boxReveries.IsLean = true;

            StartCoroutine(_boxReveries.Decline(dir, animT: _animationTime));
        }

        Moving = true;

        yield return new WaitForSeconds(_animationTime);

        CurrentTile.SetState(TileState.Clear);
        
        OnMoveComplete(toMoveTo);

        Moving = false;
    }

    private void OnMoveComplete(Tile tile)
    {
        CurrentTile = tile;
        CurrentTile.SetState(TileState.BoxIn);
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

        if ((offset.x > threshold || offset.z > threshold) || (offset.x < threshold || offset.z < threshold))
        {
            // While the objest is moving...
        }
        else
        {
            _boxReveries.OnRest();
        }
    }

    private Vector3 GetBoxPosition(Vector3 pos)
    {
        return new Vector3(pos.x, pos.y, pos.z);
    }
}