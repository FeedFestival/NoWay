using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDebug : MonoBehaviour
{
    public TextMesh TextMesh;

    public bool IsDefaultTile;

    [SerializeField]
    private TileState _tileState;
    public TileState TileState
    {
        get { return _tileState; }
        set
        {
            _tileState = value;
            if (_tileReference != null)
            {
                _tileReference.State = _tileState;
                ChangeColor(_tileState);
            }
        }
    }

    public string SpecialData;

    [SerializeField]
    private Speciality _tileSpeciality;
    public Speciality TileSpeciality
    {
        get { return _tileSpeciality; }
        set
        {
            _tileSpeciality = value;
            if (_tileReference != null)
            {
                _tileReference.Speciality = _tileSpeciality;
            }
        }
    }

    private Tile _tileReference;

    public void Init(Tile tile)
    {
        _tileReference = tile;
        _tileState = tile.State;

        TextMesh.text = tile.X + "." + tile.Y;
        ChangeColor(tile.State);

    }

    void OnValidate()
    {
        if (Application.isPlaying == false) return;

        if (_tileReference != null)
            _tileReference.Data = SpecialData;

        TileState = _tileState;
        TileSpeciality = _tileSpeciality;
    }

    private void ChangeColor(TileState tileState)
    {
        var thisMaterial = GetComponent<MeshRenderer>().materials[0];
        Color32 color = GameOptions.Instance.White;

        if (TileSpeciality != Speciality.None)
        {
            color = GameOptions.Instance.SpecialColor;
        }
        else
        {
            switch (tileState)
            {
                case TileState.Clear:
                    color = GameOptions.Instance.AvailableColor;
                    break;
                case TileState.Blocked:
                    color = GameOptions.Instance.NotAvailableColor;
                    break;
                case TileState.DeathZone:
                    color = GameOptions.Instance.DeadZoneColor;
                    break;
                case TileState.BoxIn:
                    break;
                default:
                    break;
            }
        }

        thisMaterial.color = color;
        TextMesh.color = color;
    }
}
