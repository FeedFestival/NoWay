using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxController : MonoBehaviour
{
    public List<Box> Boxes;

    public void Init()
    {
        // TODO: get them from database;

        if (Boxes == null || Boxes.Count == 0)
        {
            Boxes = new List<Box>();
            var gos = GameObject.FindGameObjectsWithTag("Box");

            foreach (GameObject go in gos)
            {
                var box = go.GetComponent<Box>();

                box.Init(tile: Game.Instance.Tiles[box.X, box.Y]);

                Boxes.Add(box);
            }
        }
    }
}
