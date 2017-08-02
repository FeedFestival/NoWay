using System.Collections;
using System.Collections.Generic;
using Assets.scripts.utils;
using UnityEngine;

public class SceneSetup : MonoBehaviour
{
    public bool UseFixedScreenScaler;

    public int HorizontalTileCount;
    public int VerticalTileCount;

    public RectTransform CanvasSizer;
    public RectTransform TopPivot;
    public RectTransform BottomPivot;
    public RectTransform MidPoint;
    public RectTransform HalfTempPoint;

    public GameObject GameSurface;

    //

    private float _widthPercent = 17.5f;
    private float _widthRightPercent = 19f;
    private float _heightPercent = 3f;

    private float _initialZ = 100f;



    void Start()
    {
        StartCoroutine(Calculate());
    }

    private IEnumerator Calculate()
    {
        yield return new WaitForSeconds(0.1f);
        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        var width = CanvasSizer.sizeDelta.x;
        var height = CanvasSizer.sizeDelta.y;

        if (UseFixedScreenScaler)
        {
            // 1. calculate top point
            var newPosX = utils.GetPercent(width, _widthPercent);
            var newPosY = utils.GetPercent(height, _heightPercent);
            TopPivot.localPosition = new Vector3(
                newPosX,
                -newPosY,
                _initialZ
            );

            // 2. calculate bottom point
            newPosX = utils.GetPercent(width, 100 - _widthRightPercent);
            BottomPivot.localPosition = new Vector3(
                +newPosX,
                newPosY - height,
                _initialZ
            );

            // 3. calculate mid point
            var topPivotWPos = TopPivot.position;
            var bottomPivotWPos = BottomPivot.position;
            Vector2 mPos = new Vector2(
                (topPivotWPos.x + bottomPivotWPos.x) / 2,
                (topPivotWPos.y + bottomPivotWPos.y) / 2
            );

            MidPoint.position = mPos;
            MidPoint.localPosition = new Vector3(MidPoint.localPosition.x, MidPoint.localPosition.y, _initialZ);

            // 
            HalfTempPoint.position = new Vector3(MidPoint.position.x, TopPivot.position.y, TopPivot.position.z);

            var yLenght = Vector3.Distance(MidPoint.position, HalfTempPoint.position) * 2;
            var tempPos = new Vector3(MidPoint.position.x, BottomPivot.position.y, MidPoint.position.z);
            var xLength = Vector3.Distance(TopPivot.position, HalfTempPoint.position) +
                          Vector3.Distance(tempPos, BottomPivot.position);

            //var gamelevel = GameSurface.transform.parent;
            //GameSurface.transform.parent = null;

            GameSurface.transform.localScale = SetGlobalScale(GameSurface.transform, new Vector3(xLength, yLenght, 1));
            GameSurface.transform.position = MidPoint.transform.position;

            //GameSurface.transform.parent = gamelevel;

            // Create the board.
            var tileSizeX = GameSurface.transform.localScale.x / HorizontalTileCount;

            var z = GameSurface.transform.position.z;

            var verticalCount = (int)(GameSurface.transform.localScale.y / tileSizeX);
            var remainingSpace = GameSurface.transform.localScale.y - (verticalCount * tileSizeX);
            
            var starPosX = topPivotWPos.x + tileSizeX / 2;
            var startPosY = (topPivotWPos.y - tileSizeX / 2) - (remainingSpace / 2);
            
            float posY = startPosY;
            
            for (int y = 0; y < verticalCount; y++)
            {
                float posX = starPosX;

                for (int x = 0; x < HorizontalTileCount; x++)
                {
                    GameObject tile = Instantiate(Resources.Load("Prefabs/tile", typeof(GameObject))) as GameObject;
                    if (tile != null)
                    {
                        if (x != 0)
                        {
                            posX = posX + tileSizeX;
                        }

                        tile.transform.position = new Vector3(posX, posY, z);
                        tile.transform.localScale = new Vector3(tileSizeX, tileSizeX, 1);
                    }
                }
                posY = posY - tileSizeX;
            }

            // garbage collect
            Destroy(GameSurface);
            GameSurface = null;
        }
    }

    public static Vector3 SetGlobalScale(Transform t, Vector3 globalScale)
    {
        t.localScale = Vector3.one;
        t.localScale = new Vector3(globalScale.x / t.lossyScale.x, globalScale.y / t.lossyScale.y, globalScale.z / t.lossyScale.z);

        return t.localScale;
    }
}
