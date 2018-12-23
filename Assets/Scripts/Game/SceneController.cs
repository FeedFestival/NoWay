using System.Collections;
using System.Collections.Generic;
using Assets.scripts.utils;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    public string MapName;

    [HideInInspector]
    public Level CurrentMap;

    [Header("Links")]
    public MapController MapController;

    // Use this for initialization
    public void Init()
    {
        Level map = Main.Instance.DataService.GetLevel(MapName);
        //MapName

        if (map == null)
            map = new Level
            {
                Id = 0,
                Name = MapName,
                isNew = true
            };

        CurrentMap = map;

        MapController.Init(CurrentMap.Id, OnMapCreated);
    }

    private void OnMapCreated()
    {
        Game.Instance.Sphere.Init(tile: Game.Instance.Tiles[4, 4]);
        Main.Instance.BoxesController.Init();
    }
}
