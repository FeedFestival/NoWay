using System;
using System.Collections;
using System.Collections.Generic;
using Assets.scripts.utils;
using UnityEngine;
using UnityEngine.UI;

public class SceneSetup : MonoBehaviour
{
    [HideInInspector]
    public bool UseFixedScreenScaler;

    [HideInInspector]
    public int ExtraPadding;
    [HideInInspector]
    public int SidePadding;
    [HideInInspector]
    public int BlockWidth;
    [HideInInspector]
    public int DesiredTilesNumber;

    [HideInInspector]
    public GameObject MapImage;

    public string MapName;

    public int SphereStartX;
    public int SphereStartY;

    public BoxController BoxController;

    [Header("Setup Options")]

    public bool DebugThis;
    public bool SeeGrid;

    public RectTransform CanvasSizer;
    public RectTransform Sizer;
    public RectTransform TopPivot;
    public RectTransform BottomPivot;
    public RectTransform MidPoint;
    public RectTransform HalfTempPoint;

    public GameObject TempTile;

    // private calculation vars

    private float _widthPercent = 17.5f;
    private float _widthRightPercent = 19f;
    private float _heightPercent = 3f;

    private float _initialZ = 100f;

    //

    private float _width;
    private float _height;
    
    private Vector2 _topPivotWPos;

    private float _starPosX;
    private float _posY;
    private float _posZ;

    //

    public void Init()
    {
        MapImage.SetActive(true);
        TempTile.SetActive(true);
        Sizer.gameObject.SetActive(true);

        GameController.Instance.Sphere.gameObject.SetActive(true);

        StartCoroutine(Calculate());
    }

    private IEnumerator Calculate()
    {
        Game.Instance.Tiles = Main.Instance.DataService.GetMainMenuTiles();

        yield return new WaitForSeconds(0.1f);
        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        Sizer.sizeDelta = new Vector2(
            utils.GetPercent(CanvasSizer.sizeDelta.x, 100f),
            utils.GetPercent(CanvasSizer.sizeDelta.y, 100f)
            );

        if (DebugThis == false)
        {
            TopPivot.GetComponent<Image>().enabled = false;
            BottomPivot.GetComponent<Image>().enabled = false;
            MidPoint.GetComponent<Image>().enabled = false;
            HalfTempPoint.GetComponent<Image>().enabled = false;
        }

        CalculateTempTileSize();

        if (UseFixedScreenScaler)
        {
            utils.TileSizeX = TempTile.transform.localScale.x / Game.Instance.HorizontalTileCount;

            Game.Instance.VerticalTileCount = (int)(TempTile.transform.localScale.y / utils.TileSizeX);
            var remainingSpace = TempTile.transform.localScale.y - (Game.Instance.VerticalTileCount * utils.TileSizeX);

            _starPosX = _topPivotWPos.x + utils.TileSizeX / 2;
            _posY = (_topPivotWPos.y - utils.TileSizeX / 2) - (remainingSpace / 2);

            PlaceTiles();
        }
        else
        {
            utils.TileSizeX = TempTile.transform.localScale.x / DesiredTilesNumber;

            // 0. figure out the extra padding based on the ratio beetween the image tile size and the current calculated tile size

            var extraPaddingRatio = utils.TileSizeX / BlockWidth;
            var extraPadding = ExtraPadding * extraPaddingRatio;

            Debug.Log(extraPadding);

            //  1. scale the mapImage

            float sidePaddingValue;
            if (BlockWidth > SidePadding)
                sidePaddingValue = (float)BlockWidth / (float)SidePadding;
            else
                sidePaddingValue = (float)SidePadding / (float)BlockWidth;

            var padding = utils.TileSizeX / sidePaddingValue;

            var mapImageSizeX = (utils.TileSizeX * Game.Instance.HorizontalTileCount) + (padding * 2) + (extraPadding * 2);
            var mapImageSizeY = (utils.TileSizeX * Game.Instance.VerticalTileCount) + (padding * 2) + (extraPadding * 2);

            MapImage.transform.localScale = SetGlobalScale(MapImage.transform, new Vector3(mapImageSizeX, mapImageSizeY, 1));

            //  2. Position the mapImage
            var halfLenght = (MapImage.transform.localScale.x / 2);
            var halfHeight = (MapImage.transform.localScale.y / 2);

            var origX = HalfTempPoint.position.x;
            HalfTempPoint.position = new Vector3(TopPivot.position.x + halfLenght, TopPivot.position.y, HalfTempPoint.position.z);
            var xDiff = HalfTempPoint.position.x - origX;
            MidPoint.position = new Vector3(MidPoint.position.x + xDiff, TopPivot.position.y - halfHeight, MidPoint.position.z);

            MapImage.transform.position = new Vector3(MidPoint.transform.position.x, MidPoint.transform.position.y, _posZ);

            // 3. Create tiles
            _starPosX = (_topPivotWPos.x + utils.TileSizeX / 2) + padding + extraPadding;
            _posY = (_topPivotWPos.y - utils.TileSizeX / 2) - padding - extraPadding;

            PlaceTiles();
        }

        if (Main.Instance.SaveMemory)
        {
            GameController.Instance.Sphere.Init(tile: Game.Instance.Tiles[SphereStartX, SphereStartY]);
        }
        else
        {
            GameController.Instance.Sphere.Init(tileObject: Game.Instance.TileObjects[SphereStartX, SphereStartY]);
        }

        BoxController.Init();

        //-----------------------------------------------------------------------------------------------------------------------------
        // garbage collect
        //-----------------------------------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------------------------------
        Destroy(TempTile);
        TempTile = null;

        if (DebugThis == false)
        {
            Destroy(TopPivot.gameObject);
            Destroy(BottomPivot.gameObject);
            Destroy(MidPoint.gameObject);
            Destroy(HalfTempPoint.gameObject);

            TopPivot = null;
            BottomPivot = null;
            MidPoint = null;
            HalfTempPoint = null;
        }
    }

    private void CalculateTempTileSize()
    {
        _width = CanvasSizer.sizeDelta.x;
        _height = CanvasSizer.sizeDelta.y;

        // 1. calculate top point
        var newPosX = utils.GetPercent(_width, _widthPercent);
        var newPosY = utils.GetPercent(_height, _heightPercent);
        TopPivot.localPosition = new Vector3(
            newPosX,
            -newPosY,
            _initialZ
        );

        // 2. calculate bottom point
        newPosX = utils.GetPercent(_width, 100 - _widthRightPercent);
        BottomPivot.localPosition = new Vector3(
            +newPosX,
            newPosY - _height,
            _initialZ
        );

        // 3. calculate mid point
        _topPivotWPos = TopPivot.position;
        var bottomPivotWPos = BottomPivot.position;
        Vector2 mPos = new Vector2(
            (_topPivotWPos.x + bottomPivotWPos.x) / 2,
            (_topPivotWPos.y + bottomPivotWPos.y) / 2
        );

        MidPoint.position = mPos;
        MidPoint.localPosition = new Vector3(MidPoint.localPosition.x, MidPoint.localPosition.y, _initialZ);

        // 
        HalfTempPoint.position = new Vector3(MidPoint.position.x, TopPivot.position.y, TopPivot.position.z);

        var yLenght = Vector3.Distance(MidPoint.position, HalfTempPoint.position) * 2;
        var tempPos = new Vector3(MidPoint.position.x, BottomPivot.position.y, MidPoint.position.z);
        var xLength = Vector3.Distance(TopPivot.position, HalfTempPoint.position) +
                      Vector3.Distance(tempPos, BottomPivot.position);

        // Create the board.
        TempTile.transform.localScale = SetGlobalScale(TempTile.transform, new Vector3(xLength, yLenght, 1));
        TempTile.transform.position = MidPoint.transform.position;

        _posZ = TempTile.transform.position.z;
    }

    private void PlaceTiles()
    {
        if (Game.Instance.Tiles == null)
            Game.Instance.Tiles = new Tile[Game.Instance.VerticalTileCount, Game.Instance.HorizontalTileCount];

        if (Main.Instance.SaveMemory == false)
            Game.Instance.TileObjects = new TileObject[Game.Instance.VerticalTileCount, Game.Instance.HorizontalTileCount];

        for (int y = 0; y < Game.Instance.VerticalTileCount; y++)
        {
            float posX = _starPosX;

            for (int x = 0; x < Game.Instance.HorizontalTileCount; x++)
            {
                if (x != 0)
                {
                    posX = posX + utils.TileSizeX;
                }

                if (Main.Instance.SaveMemory == false)
                {
                    GameObject tile = Instantiate(Resources.Load("Prefabs/tile", typeof(GameObject))) as GameObject;
                    if (tile != null)
                    {
                        tile.transform.position = new Vector3(posX, _posY, _posZ - 10);
                        tile.transform.localScale = new Vector3(utils.TileSizeX, utils.TileSizeX, 1);
                        tile.name = "[" + y + ", " + x + "]";

                        TileObject tileObject = tile.gameObject.GetComponent<TileObject>();
                        if (tileObject == null)
                            tileObject = tile.gameObject.AddComponent<TileObject>();

                        tileObject.Init(Game.Instance.Tiles[x, y], x, y);

                        // light em up !
                        LightEmUp(tileObject);

                        Game.Instance.TileObjects[x, y] = tileObject;
                    }
                }
                else
                {
                    if (Game.Instance.Tiles[x, y] == null)
                        Game.Instance.Tiles[x, y] = new Tile();

                    Game.Instance.Tiles[x, y].MakeTile(
                            x: x,
                            y: y,
                            position: new Vector3(posX, _posY, _posZ - 10),
                            tileState: TileState.Clear
                        );
                }
            }
            _posY = _posY - utils.TileSizeX;
        }
    }

    private void LightEmUp(TileObject tile)
    {
        if (SeeGrid)
        {
            switch (tile.State)
            {
                case TileState.Clear:
                    tile.Sprite.color = new Color(255f, 255f, 255f, 0.1f);
                    break;
                case TileState.Blocked:
                    tile.Sprite.color = Color.red;
                    break;
                case TileState.DeathZone:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        else
        {
            tile.Sprite.enabled = false;
        }
    }

    public static Vector3 SetGlobalScale(Transform t, Vector3 globalScale)
    {
        t.localScale = Vector3.one;
        t.localScale = new Vector3(globalScale.x / t.lossyScale.x, globalScale.y / t.lossyScale.y, globalScale.z / t.lossyScale.z);

        return t.localScale;
    }
}
