using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public GameObject MapContainer;
    public GameObject TempTile;
    public GameObject TilePrefab;

    private GameObject[,] DebugTiles;

    public int HorizontalTileCount;
    public int VerticalTileCount;

    private float _starPosX;
    private float _starPosZ;
    private float _posY;
    private bool isShowingTiles = false;

    public delegate void OnMapCreated();
    private OnMapCreated OnMapCreatedCallback;

    public void Init(int mapId, OnMapCreated onMapCreated)
    {
        _starPosX = TempTile.transform.position.x;
        _starPosZ = TempTile.transform.position.z;
        _posY = TempTile.transform.position.y + 0.5f;

        StartCoroutine(Calculate(mapId));
        OnMapCreatedCallback = onMapCreated;
    }

    private IEnumerator Calculate(int mapId)
    {
        Game.Instance.Tiles = Main.Instance.DataService.GetMainMenuTiles(mapId);

        yield return new WaitForSeconds(0.1f);

        PlaceTiles();

        OnMapCreatedCallback();

        // garbage collect
        Destroy(TempTile);
        TempTile = null;
    }

    private void PlaceTiles()
    {
        if (Game.Instance.Tiles == null)
        {
            Main.Instance.SceneController.CurrentMap.hasNoSavedTiles = true;
            Game.Instance.Tiles = new Tile[VerticalTileCount, HorizontalTileCount];
        }

        for (int y = 0; y < HorizontalTileCount; y++)
        {
            for (int x = 0; x < VerticalTileCount; x++)
            {
                if (Game.Instance.Tiles[x, y] == null)
                    Game.Instance.Tiles[x, y] = new Tile();

                if (Main.Instance.SceneController.CurrentMap.hasNoSavedTiles)
                {
                    Game.Instance.Tiles[x, y].MakeTile(
                        x: x,
                        y: y,
                        position: new Vector3(_starPosX + x, _posY, _starPosZ - y)
                        );
                }
                else
                {
                    Game.Instance.Tiles[x, y].Position = new Vector3(_starPosX + x, _posY, _starPosZ - y);
                }

                if (Game.Instance.Tiles[x, y].State == TileState.BoxIn) Game.Instance.Tiles[x, y].State = TileState.Clear;
            }
        }
    }

    public void Debug_ShowTiles()
    {
        isShowingTiles = !isShowingTiles;
        if (isShowingTiles)
        {
            if (DebugTiles == null)
                DebugTiles = new GameObject[VerticalTileCount, HorizontalTileCount];

            for (int y = 0; y < HorizontalTileCount; y++)
            {
                for (int x = 0; x < VerticalTileCount; x++)
                {
                    if (Game.Instance.Tiles[x, y] != null)
                    {
                        if (DebugTiles[x, y] == null)
                        {
                            DebugTiles[x, y] = Instantiate(TilePrefab);

                            DebugTiles[x, y].transform.position = new Vector3(Game.Instance.Tiles[x, y].Position.x, 0.0f, Game.Instance.Tiles[x, y].Position.z);
                            DebugTiles[x, y].transform.SetParent(MapContainer.transform);
                            DebugTiles[x, y].name = "[" + x + " , " + y + "]";
                            var debugTile = DebugTiles[x, y].GetComponent<TileDebug>();
                            debugTile.Init(Game.Instance.Tiles[x, y]);

                            Game.Instance.Tiles[x, y].TileDebug = debugTile;
                        }
                        else
                        {
                            DebugTiles[x, y].SetActive(true);
                            DebugTiles[x, y].GetComponent<TileDebug>().Init(Game.Instance.Tiles[x, y]);
                        }
                    }
                }
            }
        }
        else
        {
            for (int y = 0; y < HorizontalTileCount; y++)
            {
                for (int x = 0; x < VerticalTileCount; x++)
                {
                    if (Game.Instance.Tiles[x, y] != null)
                    {
                        if (DebugTiles[x, y] != null)
                        {
                            DebugTiles[x, y].SetActive(false);
                        }
                    }
                }
            }
        }
    }

    internal void Debug_SaveMapSettings(Level map)
    {
        Debug.Log("Starting to save Map Tiles...");

        if (map.isNew)
        {
            Debug.Log("1. Save new map.");
            map.Id = Main.Instance.DataService.CreateLevel(map);
        }

        var tiles = new List<Tile>();
        for (int y = 0; y < HorizontalTileCount; y++)
        {
            for (int x = 0; x < VerticalTileCount; x++)
            {
                if (Game.Instance.Tiles[x, y] != null)
                {
                    Game.Instance.Tiles[x, y].MapId = map.Id;
                    tiles.Add(Game.Instance.Tiles[x, y]);
                }
            }
        }
        if (map.isNew || map.hasNoSavedTiles)
        {
            Debug.Log("2. Save tiles.");
            Main.Instance.DataService.SaveTiles(tiles);
        }
        else {
            Debug.Log("2. Update tiles.");
            Main.Instance.DataService.UpdateTiles(tiles);
        }
        Debug.Log("3. Finished saving.");
    }
}
