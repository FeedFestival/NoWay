using System.Collections;
using System.Collections.Generic;
using Assets.scripts.utils;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SceneSetup))]
public class SceneSetupEditor : Editor
{
    private SceneSetup SceneSetup;

    override public void OnInspectorGUI()
    {
        SceneSetup = target as SceneSetup;

        if (SceneSetup == null)
            return;

        SceneSetup.UseFixedScreenScaler = GUILayout.Toggle(SceneSetup.UseFixedScreenScaler, "UseFixedScreenScaler");

        //SceneSetup.HorizontalTileCount = EditorGUILayout.IntField("HorizontalTileCount:", SceneSetup.HorizontalTileCount);

        if (SceneSetup.UseFixedScreenScaler == false)
            SceneSetup.ExtraPadding = EditorGUILayout.IntField("ExtraPadding:", SceneSetup.ExtraPadding);

        if (SceneSetup.UseFixedScreenScaler == false)
            SceneSetup.SidePadding = EditorGUILayout.IntField("SidePadding:", SceneSetup.SidePadding);

        if (SceneSetup.UseFixedScreenScaler == false)
            SceneSetup.BlockWidth = EditorGUILayout.IntField("BlockWidth:", SceneSetup.BlockWidth);

        if (SceneSetup.UseFixedScreenScaler == false)
            SceneSetup.DesiredTilesNumber = EditorGUILayout.IntField("DesiredTilesNumber:", SceneSetup.DesiredTilesNumber);

        if (SceneSetup.UseFixedScreenScaler == false)
            SceneSetup.MapImage = EditorGUILayout.ObjectField("MapImage: ", SceneSetup.MapImage, typeof(GameObject), true) as GameObject;

        DrawDefaultInspector();

        if (utils.TileSizeX > 0)
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField(
                string.Format(@"

TileSizeX : {0}

            ", utils.TileSizeX
                    ));

            EditorGUILayout.EndVertical();
        }
    }
}
