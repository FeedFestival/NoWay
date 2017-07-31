using System.Collections;
using System.Collections.Generic;
using Assets.scripts.utils;
using UnityEngine;

public class SceneSetup : MonoBehaviour
{
    public RectTransform CanvasSizer;
    public RectTransform TopPivot;
    public RectTransform BottomPivot;
    public RectTransform MidPoint;
    public RectTransform HalfTempPoint;

    public GameObject GameSurface;

    void Start()
    {
        StartCoroutine(Calculate());
    }

    private IEnumerator Calculate()
    {
        yield return new WaitForSeconds(0.2f);

        var width = CanvasSizer.sizeDelta.x;
        var height = CanvasSizer.sizeDelta.y;

        // 1. calculate top point
        var newPosX = utils.GetPercent(width, 16.5f);
        var newPosY = utils.GetPercent(height, 2f);
        TopPivot.localPosition = new Vector3(
            newPosX,
            -newPosY,
            500f
            );

        // 2. calculate bottom point
        newPosX = utils.GetPercent(width, 100 - 18f);
        BottomPivot.localPosition = new Vector3(
            +newPosX,
            newPosY - height,
            500f
        );

        // 3. calculate mid point
        var topPivotWPos = TopPivot.position;
        var bottomPivotWPos = BottomPivot.position;
        Vector2 mPos = new Vector2(
            (topPivotWPos.x + bottomPivotWPos.x) / 2,
            (topPivotWPos.y + bottomPivotWPos.y) / 2
            );

        MidPoint.position = mPos;
        MidPoint.localPosition = new Vector3(MidPoint.localPosition.x, MidPoint.localPosition.y, 500f);

        // 
        HalfTempPoint.position = new Vector3(MidPoint.position.x, TopPivot.position.y, TopPivot.position.z);

        var yLenght = Vector3.Distance(MidPoint.position, HalfTempPoint.position) * 2;
        var tempPos = new Vector3(MidPoint.position.x, BottomPivot.position.y, MidPoint.position.z);
        var xLength = Vector3.Distance(TopPivot.position, HalfTempPoint.position) + Vector3.Distance(tempPos, BottomPivot.position);

        GameSurface.transform.localScale = new Vector3(xLength, yLenght, 1);
        GameSurface.transform.position = MidPoint.transform.position;
    }
}
