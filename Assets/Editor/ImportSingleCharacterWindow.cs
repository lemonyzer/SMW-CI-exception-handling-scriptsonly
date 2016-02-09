using UnityEngine;
using System.Collections;

using UnityEditor;

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
    Sprite testSprite;
    Color testColor;

    public  Color32 redDark = new Color32(128, 0, 0, 255);
    public  Color32 redMiddel = new Color32(192, 0, 0, 255);
    public  Color32 redLight = new Color32(255, 0, 0, 255);
    public  Color32 redLight32 = new Color32((byte)255, (byte)0, (byte)0, (byte)255);

    public  Color32 greenDark = new Color32(90, 146, 0, 255);
    public  Color32 greenMiddel = new Color32(128, 178, 0, 255);
    public  Color32 greenLight = new Color32(165, 219, 0, 255);

    public  Color32 yellowDark = new Color32(214, 101, 0, 255);
    public  Color32 yellowMiddel = new Color32(247, 158, 0, 255);
    public  Color32 yellowLight = new Color32(255, 211, 16, 255);

    public  Color32 blueDark = new Color32(41, 73, 132, 255);
    public  Color32 blueMiddel = new Color32(57, 113, 173, 255);
    public  Color32 blueLight = new Color32(74, 154, 214, 255);

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
                GUILayout.Label("SMW Single Character Importer", EditorStyles.boldLabel);
                GUILayout.BeginHorizontal();
                {
                    GUILayout.BeginVertical();
                    //redDark = EditorGUILayout.ColorField(redDark);
                    //redMiddel = EditorGUILayout.ColorField(redMiddel);
                    //redLight = EditorGUILayout.ColorField(redLight);
                    //greenDark = EditorGUILayout.ColorField(greenDark);
                    //greenMiddel = EditorGUILayout.ColorField(greenMiddel);
                    //greenLight = EditorGUILayout.ColorField(greenLight);
                    //yellowDark = EditorGUILayout.ColorField(yellowDark);
                    //yellowMiddel = EditorGUILayout.ColorField(yellowMiddel);
                    //yellowLight = EditorGUILayout.ColorField(yellowLight);
                    //blueDark = EditorGUILayout.ColorField(blueDark);
                    //blueMiddel = EditorGUILayout.ColorField(blueMiddel);
                    //blueLight = EditorGUILayout.ColorField(blueLight);

                    palette = EditorGUILayout.ObjectField("Palette", palette, typeof(Palette), false) as Palette;


                    testSprite = EditorGUILayout.ObjectField("RAW Character Spritesheet", testSprite, typeof(Sprite), false) as Sprite;
                    if (testSprite != null)
                        GUI.enabled = true;
                    else
                        GUI.enabled = false;

                    //OnGUI_SO_Test();

                    fStepByStep = EditorGUILayout.Foldout(fStepByStep, "Step-By-Step Import");
                    if (fStepByStep)
                    {

                        if (GUILayout.Button("SetImportSettings"))
                        {
                            SpriteImport.SetRawCharacterSpriteSheetTextureImporter(testSprite, true, false, false);
                        }
                        if (GUILayout.Button("CreatePNG"))
                        {
                            testSprite = SpriteImport.CreateGenericCharacterSpriteSheet(testSprite);
                        }

                        slectedTeam = (Teams)EditorGUILayout.EnumPopup("Team", slectedTeam);
                        if (slectedTeam == Teams.count)
                            slectedTeam = Teams.yellow;

                        if (GUILayout.Button("ChangeColor old"))
                        {
                            TeamColor.ChangeColors(TeamColor.referenceColorsVerzweigt[(int)slectedTeam], testSprite.texture);
                        }

                        if (GUILayout.Button("ChangeColor full"))
                        {
                            palette.ChangeColors((int)slectedTeam, testSprite.texture);
                        }

                        if (GUILayout.Button("Save"))
                        {
                            testSprite.texture.Apply();
                            EditorUtility.SetDirty(testSprite);
                            EditorUtility.SetDirty(testSprite.texture);

                            AssetDatabase.StartAssetEditing();
                            EditorUtility.SetDirty(testSprite);
                            EditorUtility.SetDirty(testSprite.texture);
                            AssetDatabase.StopAssetEditing();

                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                        }

                        if (GUILayout.Button("ReImport"))
                        {
                            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(testSprite));
                        }
                    }

                    fFull = EditorGUILayout.Foldout(fFull, "Full Import");
                    if (fFull)
                    {
                        if (GUILayout.Button("Import"))
                        {
                            SpriteImport.ImportCharacter(testSprite, palette, fShowCreatedSprites);
                        }
                        
                        fShowCreatedSprites = GUILayout.Toggle(fShowCreatedSprites, "Show created Sprites");
                    }

                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
                GUILayout.Label("Current Tileset", EditorStyles.boldLabel);
                GUILayout.Space(10);
                GUILayout.Label("Tileset Tile Preview", EditorStyles.boldLabel);

                if (GUILayout.Button("Show Tile (NewSprite)", GUILayout.ExpandWidth(false)))
                {
                }
                if (GUILayout.Button("Show Tile (AssetSlicedSprite)", GUILayout.ExpandWidth(false)))
                {
                }
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
        //		Repaint();
    }

    void OnGUI_SO_Test ()
    {
        testCharacter = EditorGUILayout.ObjectField("Character", testCharacter, typeof(SmwCharacter), false) as SmwCharacter;

        fStepByStep = EditorGUILayout.Foldout(fStepByStep, "Step-By-Step Import");
        if (fStepByStep)
        {

            if (GUILayout.Button("SetImportSettings"))
            {
                SpriteImport.SetRawCharacterSpriteSheetTextureImporter(testCharacter.sprite, true, false, false);
            }
            if (GUILayout.Button("CreatePNG"))
            {
                testSprite = SpriteImport.CreateGenericCharacterSpriteSheet(testCharacter.sprite);
            }

            slectedTeam = (Teams)EditorGUILayout.EnumPopup("Team", slectedTeam);

            if (GUILayout.Button("ChangeColor"))
            {
                TeamColor.ChangeColors(TeamColor.referenceColorsVerzweigt[(int)slectedTeam], testCharacter.sprite.texture);
            }

            if (GUILayout.Button("ReImport"))
            {
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(testCharacter.sprite));
            }
        }
    }
}
