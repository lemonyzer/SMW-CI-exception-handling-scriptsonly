﻿using UnityEngine;
using System.Collections;

using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;

public class ImportSingleCharacterWindow : EditorWindow
{
    static ImportSingleCharacterWindow currWindow;

    [MenuItem("SMW/ScriptableObject/Palette", false, 1)]
    public static Palette CreatePalette()
    {
        Palette newAsset = ScriptableObject.CreateInstance<Palette>();

        string path = "Assets/newPaletteSO.asset";
        AssetDatabase.CreateAsset(newAsset, AssetDatabase.GenerateUniqueAssetPath(path));
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newAsset;

        return newAsset;
    }

    [MenuItem("SMW/ScriptableObject/SmwCharacter", false, 1)]
    public static SmwCharacter CreateSmwCharacter()
    {
        SmwCharacter newAsset = ScriptableObject.CreateInstance<SmwCharacter>();

        string path = "Assets/newSmwCharacterSO.asset";
        AssetDatabase.CreateAsset(newAsset, AssetDatabase.GenerateUniqueAssetPath(path));
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newAsset;

        return newAsset;
    }

    [MenuItem("SMW/Import/Single Character",false,1)]
    public static void Init()
    {
        if (currWindow == null)
        {
            currWindow = (ImportSingleCharacterWindow)EditorWindow.GetWindow(typeof(ImportSingleCharacterWindow));
            currWindow.titleContent.text = "Import Single Character";
        }
        else
        {
            currWindow.Show();
        }
    }

    Palette palette;
    SmwCharacter testCharacter;
    Sprite rawSpritesheet;
    Sprite preparedSpritesheet;
    Sprite preparedTeamSpritesheet;
    Color testColor;

    Teams slectedTeam = Teams.yellow;

    bool fShowCreatedSprites = true;
    bool fStepByStep = true;
    bool fFull = true;

    void OnGUI()
    {
        //EditorGUILayout.Space(10);
        GUILayout.BeginHorizontal();
        {
            GUILayout.Space(10);
            GUILayout.BeginVertical();
            {
                GUILayout.Space(10);
                GUILayout.Label("SMW Character Importer", EditorStyles.boldLabel);
                GUILayout.BeginHorizontal();
                {
                    GUILayout.BeginVertical();

                    palette = EditorGUILayout.ObjectField("Palette", palette, typeof(Palette), false) as Palette;
                    if (palette != null)
                        GUI.enabled = true;
                    else
                        GUI.enabled = false;

                    fBatchImport = EditorGUILayout.Foldout(fBatchImport, "Batch Import");
                    if (fBatchImport)
                    {
                        OnGUI_BatchImport();
                    }

                    fSingleImport = EditorGUILayout.Foldout(fSingleImport, "Single Import");
                    if (fSingleImport)
                    {
                        OnGUI_SingleImport();
                    }

                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
        //		Repaint();
    }

    private void OnGUI_SingleImport ()
    {
        rawSpritesheet = EditorGUILayout.ObjectField("RAW Character Spritesheet", rawSpritesheet, typeof(Sprite), false, GUILayout.ExpandWidth(true)) as Sprite;
        if (rawSpritesheet != null)
        {
            Rect controlRect = EditorGUILayout.GetControlRect(true, rawSpritesheet.texture.height, GUILayout.ExpandWidth(false));
            EditorGUI.DrawPreviewTexture(controlRect, rawSpritesheet.texture);
            GUI.enabled = true;
        }
        else
            GUI.enabled = false;



        fStepByStep = EditorGUILayout.Foldout(fStepByStep, "Step-By-Step Import");
        if (fStepByStep)
        {
            OnGUI_SingleStepByStepImport();
        }

        fFull = EditorGUILayout.Foldout(fFull, "Auto Import");
        if (fFull)
        {
            OnGUI_SingleAutoImport();
        }
    }

    private void OnGUI_SingleStepByStepImport()
    {
        if (GUILayout.Button("SetImportSettings"))
        {
            rawSpritesheet = SpriteImport.SetRawCharacterSpriteSheetTextureImporter(rawSpritesheet, true, false, false);
        }
        if (GUILayout.Button("CreatePNG"))
        {
            preparedSpritesheet = SpriteImport.CreateGenericCharacterSpriteSheet(rawSpritesheet);
        }

        preparedSpritesheet = EditorGUILayout.ObjectField("Prepared Character Spritesheet", preparedSpritesheet, typeof(Sprite), false) as Sprite;
        if (preparedSpritesheet != null)
        {
            Rect controlRect = EditorGUILayout.GetControlRect(true, preparedSpritesheet.texture.height, GUILayout.ExpandWidth(false));
            EditorGUI.DrawTextureTransparent(controlRect, preparedSpritesheet.texture);
            GUI.enabled = true;
        }
        else
            GUI.enabled = false;

        slectedTeam = (Teams)EditorGUILayout.EnumPopup("Team", slectedTeam);
        if (slectedTeam == Teams.count)
            slectedTeam = Teams.yellow;

        if (GUILayout.Button("ChangeColor old"))
        {
            TeamColor.ChangeColors(TeamColor.referenceColorsVerzweigt[(int)slectedTeam], preparedSpritesheet.texture);
        }

        if (GUILayout.Button("ChangeColor full (copy&edit)"))
        {
            palette.ChangeColors((int)slectedTeam, preparedSpritesheet.texture);
            // TODO replace with
            // preparedTeamSpritesheet = SpriteImport.CreateTeamSprite(slectedTeam, preparedSpritesheet);
        }

        preparedTeamSpritesheet = EditorGUILayout.ObjectField("Prepared Team Spritesheet", preparedTeamSpritesheet, typeof(Sprite), false) as Sprite;
        if (preparedTeamSpritesheet != null)
        {
            Rect controlRect = EditorGUILayout.GetControlRect(true, preparedTeamSpritesheet.texture.height, GUILayout.ExpandWidth(false));
            EditorGUI.DrawTextureTransparent(controlRect, preparedTeamSpritesheet.texture);
            GUI.enabled = true;
        }
        else
            GUI.enabled = false;


        if (GUILayout.Button("Save"))
        {
            preparedSpritesheet.texture.Apply();
            EditorUtility.SetDirty(preparedSpritesheet);
            EditorUtility.SetDirty(preparedSpritesheet.texture);

            AssetDatabase.StartAssetEditing();
            EditorUtility.SetDirty(preparedSpritesheet);
            EditorUtility.SetDirty(preparedSpritesheet.texture);
            AssetDatabase.StopAssetEditing();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        if (GUILayout.Button("ReImport"))
        {
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(preparedSpritesheet));
        }
    }

    private void OnGUI_SingleAutoImport()
    {
        if (GUILayout.Button("Import"))
        {
            SpriteImport.ImportCharacter(rawSpritesheet, palette, fShowCreatedSprites);
        }

        fShowCreatedSprites = GUILayout.Toggle(fShowCreatedSprites, "Show created Sprites");
    }

    string batch_ImportPath = "";
    string batch_LastWorkingImportPath = "";
    string EP_lastBatchImportFolder = "EP_lastBatchImportFolder";
    //FileInfo[] window_Batch_FileInfo;
    static IEnumerable<FileInfo> window_Batch_FileInfos;
    string[] extensions = new string[] { ".png", ".bmp" };


    void DebugFiles (DirectoryInfo dirInfo, IEnumerable<FileInfo> files)
    {
        Debug.Log("FullName= " + dirInfo.FullName);
        Debug.Log("Name= " + dirInfo.Name);
        foreach (FileInfo item in files)
        {
            Debug.Log(item.Name);
        }
    }

    void OnGUI_BatchImport()
    {
        if (GUILayout.Button("Select Import Folder"))
        {
            // open folder dialog
            batch_ImportPath = EditorUtility.OpenFolderPanel("Select Import Folder with Sprites", batch_LastWorkingImportPath, "");
            if (!string.IsNullOrEmpty(batch_ImportPath))
            {
                batch_LastWorkingImportPath = batch_ImportPath;
                //absolutenPath in EditorPrefs speichern 
                EditorPrefs.SetString(EP_lastBatchImportFolder, batch_LastWorkingImportPath);

                DirectoryInfo dirInfo = new DirectoryInfo(batch_ImportPath);
                window_Batch_FileInfos = BatchImport.GetFilesByExtensions(dirInfo, extensions);
                DebugFiles(dirInfo, window_Batch_FileInfos);
                //window_Batch_FileInfo = BatchImport.GetFileList(batch_ImportPath, ".png");
                GUI.enabled = true;
            }
            else
            {
                //WITCHTIG!!!!!!!!!!
                batch_ImportPath = "";
                //window_Batch_FileInfo = null;
                GUI.enabled = false;
            }
        }
        if (GUILayout.Button ("Start Batch Import"))
        {
            StartBatchImport(new DirectoryInfo(batch_ImportPath), palette);
        }
    }

    Vector3 previewPosition = Vector3.zero;
    private bool fBatchImport;
    private bool fSingleImport;

    void StartBatchImport(DirectoryInfo dirInfo, Palette palette)
    {

        IEnumerable<FileInfo> files = BatchImport.GetFilesByExtensions(dirInfo, extensions);

        foreach (FileInfo f in files)
        {
            // relative pfad angabe
            string currentSpritePath = f.FullName.Substring(Application.dataPath.Length - "Assets".Length);
            Debug.Log("Found " + currentSpritePath);

            Sprite currentSprite = null;
            TextureImporter spriteImporter = null;
            spriteImporter = TextureImporter.GetAtPath(currentSpritePath) as TextureImporter;
            if (spriteImporter == null)
            {
                Debug.LogError(currentSpritePath + " TextureImporter == null ");
                continue;       // skip this character
            }
            else
            {
                currentSprite = AssetDatabase.LoadAssetAtPath(spriteImporter.assetPath, typeof(Sprite)) as Sprite;
                // PerformMetaSlice
                previewPosition.y--;
                SpriteImport.ImportCharacter(currentSprite, palette, true, previewPosition);
            }
        }

    }

    //void OnGUI_SO_Test ()
    //{
    //    testCharacter = EditorGUILayout.ObjectField("Character", testCharacter, typeof(SmwCharacter), false) as SmwCharacter;

    //    fStepByStep = EditorGUILayout.Foldout(fStepByStep, "Step-By-Step Import");
    //    if (fStepByStep)
    //    {

    //        if (GUILayout.Button("SetImportSettings"))
    //        {
    //            SpriteImport.SetRawCharacterSpriteSheetTextureImporter(testCharacter.sprite, true, false, false);
    //        }
    //        if (GUILayout.Button("CreatePNG"))
    //        {
    //            preparedSpritesheet = SpriteImport.CreateGenericCharacterSpriteSheet(testCharacter.sprite);
    //        }

    //        slectedTeam = (Teams)EditorGUILayout.EnumPopup("Team", slectedTeam);

    //        if (GUILayout.Button("ChangeColor"))
    //        {
    //            TeamColor.ChangeColors(TeamColor.referenceColorsVerzweigt[(int)slectedTeam], testCharacter.sprite.texture);
    //        }

    //        if (GUILayout.Button("ReImport"))
    //        {
    //            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(testCharacter.sprite));
    //        }
    //    }
    //}

}
