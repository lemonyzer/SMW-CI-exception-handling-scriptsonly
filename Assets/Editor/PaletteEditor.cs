using UnityEngine;
using System.Collections;

using UnityEditor;

[CustomEditor(typeof(Palette))]
public class PaletteEditor : Editor {

    Palette targetObject;

    void OnEnable()
    {
        targetObject = (Palette)target;
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI ();
        DrawDefaultInspector();

        GUILayout.Space(10);
        GUIElemts();
    }

    void OnSceneGUI()
    {
        // Handles for Scene View
 //       Handles.color = targetObject.myColor;
        //		Handles.CubeCap (0, targetObject.transform.position, targetObject.transform.rotation, targetObject.handleSize);
 //       Handles.SphereCap(0, targetObject.transform.position, targetObject.transform.rotation, targetObject.handleSize);
 //       Handles.Label(targetObject.transform.position + new Vector3(0f, targetObject.handleSize, 0f), targetObject.name);

        // 2D GUI for Scene View
        Handles.BeginGUI();
        GUILayout.BeginArea(new Rect(10f, 10f, 100f, 400f));
        //Handles.Button("Next Map");
        GUIElemts();
        GUILayout.EndArea();
        Handles.EndGUI();
    }

    bool fTeamRed = true;
    bool fTeamGreen = true;
    bool fTeamYellow = true;
    bool fTeamBlue = true;

    Vector2 teamSliderPosition;

    void GUIElemts()
    {
        GUILayout.BeginVertical();
        {
            if (GUILayout.Button("Init Palette"))
            {
                targetObject.InitPalette();
            }

            teamSliderPosition = EditorGUILayout.BeginScrollView(teamSliderPosition);
            {
                GUILayout.BeginHorizontal();
                {
                    int teamNr = 0;
                    fTeamRed = EditorGUILayout.Foldout(fTeamRed, "Rot");
                    if (fTeamRed)
                    {
                        GUITeamPalette(teamNr++);
                    }

                    fTeamGreen = EditorGUILayout.Foldout(fTeamGreen, "Grün");
                    if (fTeamGreen)
                    {
                        GUITeamPalette(teamNr++);
                    }

                    fTeamYellow = EditorGUILayout.Foldout(fTeamYellow, "Gelb");
                    if (fTeamYellow)
                    {
                        GUITeamPalette(teamNr++);
                    }

                    fTeamBlue = EditorGUILayout.Foldout(fTeamBlue, "Blau");
                    if (fTeamBlue)
                    {
                        GUITeamPalette(teamNr++);
                    }
                }
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }
        GUILayout.EndVertical();

    }

    void GUITeamPalette (int teamId)
    {
        GUILayout.BeginVertical();

        Color[] teamColorPalette = targetObject.GetTeamColorPalette (teamId);

        if (teamColorPalette == null)
        {
            GUILayout.Label((Teams)teamId + " == NULL");
        }
        else
        {
            for (int i=0; i< teamColorPalette.Length; i++)
            {
                EditorGUILayout.ColorField(teamColorPalette[i]);
            }
        }
        GUILayout.EndVertical();
    }
}
