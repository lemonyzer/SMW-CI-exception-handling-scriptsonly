using UnityEngine;
using System.Collections;

using UnityEditor;
using System;
using SMW;

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
        //Handles for Scene View
        //Handles.color = targetObject.myColor;
        //Handles.CubeCap (0, targetObject.transform.position, targetObject.transform.rotation, targetObject.handleSize);
        //Handles.SphereCap(0, targetObject.transform.position, targetObject.transform.rotation, targetObject.handleSize);
        //Handles.Label(targetObject.transform.position + new Vector3(0f, targetObject.handleSize, 0f), targetObject.name);

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
    private bool fViewTeamPaletteCached;
    private bool fViewTeamPaletteSlow;

    void GUIElemts()
    {
        GUILayout.BeginVertical();
        {
            if (GUILayout.Button("Init Palette"))
            {
                targetObject.InitPalette();
            }
            fViewTeamPaletteCached = EditorGUILayout.Foldout(fViewTeamPaletteCached, "View Team Palette from cached List");
            if (fViewTeamPaletteCached)
            {
                GUI_ViewTeamPaletteCached();
            }
            fViewTeamPaletteSlow = EditorGUILayout.Foldout(fViewTeamPaletteSlow, "View Team Palette from Array");
            if (fViewTeamPaletteSlow)
            {
                GUI_ViewTeamPaletteSlow();
            }

        }
        GUILayout.EndVertical();

    }

    private void GUI_ViewTeamPaletteCached()
    {

        GUILayout.BeginHorizontal();
        {
            // Header
            GUILayout.Label("ref"); // ref Color Header
            for (int i=0; i<(int)Teams.count; i++)
            {
                GUILayout.Label("" +(Teams)i);  // team Color Header
            }
        }
        GUILayout.EndHorizontal();

        teamSliderPosition = EditorGUILayout.BeginScrollView(teamSliderPosition);
        {
            GUILayout.BeginHorizontal();
            {
                // Reference Color Palette
                GUIReferencePalette();

                int teamNr = 0;
                //fTeamRed = EditorGUILayout.Foldout(fTeamRed, "");
                if (fTeamRed)
                {
                    GUITeamPaletteCached(teamNr++);
                }

                //fTeamGreen = EditorGUILayout.Foldout(fTeamGreen, "");
                if (fTeamGreen)
                {
                    GUITeamPaletteCached(teamNr++);
                }

                //fTeamYellow = EditorGUILayout.Foldout(fTeamYellow, "");
                if (fTeamYellow)
                {
                    GUITeamPaletteCached(teamNr++);
                }

                //fTeamBlue = EditorGUILayout.Foldout(fTeamBlue, "");
                if (fTeamBlue)
                {
                    GUITeamPaletteCached(teamNr++);
                }
            }
            GUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }

    private void GUI_ViewTeamPaletteSlow()
    {
        GUILayout.BeginHorizontal();
        {
            for (int i = 0; i < (int)Teams.count; i++)
            {
                GUILayout.Label("" + (Teams)i);
            }
        }
        GUILayout.EndHorizontal();

        teamSliderPosition = EditorGUILayout.BeginScrollView(teamSliderPosition);
        {
            GUILayout.BeginHorizontal();
            {
                int teamNr = 0;
                //fTeamRed = EditorGUILayout.Foldout(fTeamRed, "Rot");
                if (fTeamRed)
                {
                    GUITeamPaletteSlow(teamNr++);
                }

                //fTeamGreen = EditorGUILayout.Foldout(fTeamGreen, "Grün");
                if (fTeamGreen)
                {
                    GUITeamPaletteSlow(teamNr++);
                }

                //fTeamYellow = EditorGUILayout.Foldout(fTeamYellow, "Gelb");
                if (fTeamYellow)
                {
                    GUITeamPaletteSlow(teamNr++);
                }

                //fTeamBlue = EditorGUILayout.Foldout(fTeamBlue, "Blau");
                if (fTeamBlue)
                {
                    GUITeamPaletteSlow(teamNr++);
                }
            }
            GUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }

    void GUIReferencePalette ()
    {
        GUILayout.BeginVertical();

        Color[] colorPalette = targetObject.RawReferenceColorPalette;

        if (colorPalette == null)
        {
            GUILayout.Label(targetObject.ToString() + " RawReferenceColorPalettet == NULL");
        }
        else
        {
            for (int i = 0; i < colorPalette.Length; i++)
            {
                EditorGUILayout.ColorField(colorPalette[i]);
            }
        }
        GUILayout.EndVertical();
    }

    void GUITeamPaletteSlow (int teamId)
    {
        GUILayout.BeginVertical();

        Color[] teamColorPalette = targetObject.GetTeamColorPaletteSlow (teamId);

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

    void GUITeamPaletteCached (int teamId)
    {
        GUILayout.BeginVertical();

        Color[] teamColorPalette = targetObject.GetTeamColorPaletteFromList(teamId);

        if (teamColorPalette == null)
        {
            GUILayout.Label((Teams)teamId + " == NULL");
        }
        else
        {
            for (int i = 0; i < teamColorPalette.Length; i++)
            {
                EditorGUILayout.ColorField(teamColorPalette[i]);
            }
        }
        GUILayout.EndVertical();
    }
}
