﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using System;
using System.IO;

public class SpriteImport {

    public static void ImportCharacter(Sprite originalSprite, Palette palette, bool fCreatePreview)
    {
        if (originalSprite == null)
        {
            Debug.LogError("original Sprite not set!");
            return;
        }

        if (palette == null)
        {
            Debug.LogError("Palette not set!");
            return;
        }

        // Import in Unity (manually)
        string spriteAssetPath = AssetDatabase.GetAssetPath(originalSprite);

        // Set Import Settings, and Slice
        SetRawCharacterSpriteSheetTextureImporter(originalSprite, true, false, false);

        // Add Alpha Channel
        Color32 targetColor = new Color32(255, 0, 255, 255);
        //Color32 w_ReplacementColor = new Color32(255, 0, 255, 0);
        Texture2D textureWithAlpha = AddAlphaChannel(originalSprite.texture, targetColor);

        // Generate Unique Asset Path for new Texture
        //string newAssetPath = Path.GetDirectoryName(spriteAssetPath) + "/" + Path.GetFileNameWithoutExtension(spriteAssetPath) + "_ARGB32_" + Path.GetExtension(spriteAssetPath);
        string newAssetPath = Path.GetDirectoryName(spriteAssetPath) + "/" + Path.GetFileNameWithoutExtension(spriteAssetPath) + "_ARGB32_" + ".png";
        string newUniqueAssetPath = AssetDatabase.GenerateUniqueAssetPath(newAssetPath);
       
        // Save Transparent Spritesheet as PNG
        Sprite spriteWithAlpha = SaveTextureAsSprite(textureWithAlpha, newUniqueAssetPath);
        // Set Import Settings, and Slice
        SetRawCharacterSpriteSheetTextureImporter(spriteWithAlpha, true, true, false);

        Texture2D tempTexture = new Texture2D(textureWithAlpha.width, textureWithAlpha.height, TextureFormat.ARGB32, false);
        for (int i=0; i<(int)Teams.count; i++)
        {
            // Generate Unique Asset Path for new Texture
            string teamAssetPath = Path.GetDirectoryName(spriteAssetPath) + "/" + Path.GetFileNameWithoutExtension(spriteAssetPath) + "_ARGB32_" + (Teams)i + Path.GetExtension(spriteAssetPath);
            string uniqueTeamAssetPath = AssetDatabase.GenerateUniqueAssetPath(teamAssetPath);

            //TeamColor.ChangeColors(TeamColor.referenceColors[i], textureWithAlpha);
            //TeamColor.ChangeColors(TeamColor.referenceColorsVerzweigt[i], textureWithAlpha);
            // Full Palette
            tempTexture.SetPixels (textureWithAlpha.GetPixels());
            palette.ChangeColors(i, tempTexture);

            // Save as PNG
            Sprite teamSpritesheet = SaveTextureAsSprite(tempTexture, uniqueTeamAssetPath);

            // Apply Import Settings, and Slice
            SetRawCharacterSpriteSheetTextureImporter(teamSpritesheet, false, true, true);

            if (fCreatePreview)
            {
                GameObject preview = new GameObject("Preview " + i);
                preview.transform.position = new Vector2(-2, 0) + Vector2.right * i;
                SpriteRenderer renderer = preview.AddComponent<SpriteRenderer>();
                renderer.sprite = teamSpritesheet;
            }
        }
        // Clean Memory
        UnityEngine.Object.DestroyImmediate(tempTexture);
        UnityEngine.Object.DestroyImmediate(textureWithAlpha);
    }

    public static void ImportCharacterEfficient(Sprite originalSprite)
    {

        // Import in Unity (manually)
        string spriteAssetPath = AssetDatabase.GetAssetPath(originalSprite);

        // TODO only set advanced: readable texture 
        // Set Import Settings, and Slice
        SetRawCharacterSpriteSheetTextureImporter(originalSprite, true, false, false);

        // Add Alpha Channel
        Color32 targetColor = new Color32(255, 0, 255, 255);
        //Color32 w_ReplacementColor = new Color32(255, 0, 255, 0);
        Texture2D textureWithAlpha = AddAlphaChannel(originalSprite.texture, targetColor);

        // Generate Unique Asset Path for new Texture
        //string newAssetPath = Path.GetDirectoryName(spriteAssetPath) + "/" + Path.GetFileNameWithoutExtension(spriteAssetPath) + "_ARGB32_" + Path.GetExtension(spriteAssetPath);
        string newAssetPath = Path.GetDirectoryName(spriteAssetPath) + "/" + Path.GetFileNameWithoutExtension(spriteAssetPath) + "_ARGB32_" + ".png";
        string newUniqueAssetPath = AssetDatabase.GenerateUniqueAssetPath(newAssetPath);

        // Save as PNG
        Sprite spriteWithAlpha = SaveTextureAsSprite(textureWithAlpha, newUniqueAssetPath);
        // Clean Memory
        UnityEngine.Object.DestroyImmediate(textureWithAlpha);
        // Set Import Settings, and Slice
        SetRawCharacterSpriteSheetTextureImporter(spriteWithAlpha, true, true, false);


        for (int i = 0; i < (int)Teams.count; i++)
        {

            // Generate Unique Asset Path for new Texture
            string teamAssetPath = Path.GetDirectoryName(spriteAssetPath) + "/" + Path.GetFileNameWithoutExtension(spriteAssetPath) + "_ARGB32_" + (Teams)i + Path.GetExtension(spriteAssetPath);
            string uniqueTeamAssetPath = AssetDatabase.GenerateUniqueAssetPath(teamAssetPath);

            if (AssetDatabase.CopyAsset(newUniqueAssetPath, uniqueTeamAssetPath))
            {
                // Manually refresh the Database to inform of a change (CopyAsset)
                AssetDatabase.Refresh();

                // change color of asset copy
                Sprite teamSprite = AssetDatabase.LoadAssetAtPath(uniqueTeamAssetPath, typeof(Sprite)) as Sprite;
                TeamColor.ChangeColors(TeamColor.referenceColorsVerzweigt[i], teamSprite.texture);

                //teamSprite.texture.Apply(false, true);
                // doesnt save permanently

                // http://forum.unity3d.com/threads/best-easiest-way-to-change-color-of-certain-pixels-in-a-single-sprite.223030/

                //Texture2D teamSpriteTexture = AssetDatabase.LoadAssetAtPath(uniqueTeamAssetPath, typeof(Texture2D)) as Texture2D;
                //TeamColor.ChangeColors(TeamColor.referenceColorsVerzweigt[i], teamSpriteTexture);

                //// speichere texture änderungen
                //teamSpriteTexture.Apply();

                // http://answers.unity3d.com/questions/731557/texture2d-setpixelsapply-changes-not-permanent.html
                // http://answers.unity3d.com/questions/332581/setpixelsapply-doesnt-keep-changes-after-restart.html

                // speichere texture änderungen
                //AssetDatabase.AddObjectToAsset(teamSpriteTexture, uniqueTeamAssetPath);
                //AssetDatabase.SaveAssets();

                // speichere texture änderungen
                //EditorUtility.SetDirty(teamSpriteTexture);
                //AssetDatabase.SaveAssets();

                // spichere texture änderungen
                //Begin Asset importing. This lets you group several asset imports together into one larger import.
                //AssetDatabase.StartAssetEditing();
                //AssetDatabase.ImportAsset(uniqueTeamAssetPath);
                //AssetDatabase.StopAssetEditing();
            }
            else
                Debug.LogError(originalSprite.name + " Team: " + (Teams)i);
            
        }
    }

    public static Sprite CreateGenericCharacterSpriteSheet(Sprite originalSprite)
    {
        // Import in Unity (manually)
        string spriteAssetPath = AssetDatabase.GetAssetPath(originalSprite);

        // Add Alpha Channel
        Color32 targetColor = new Color32(255, 0, 255, 255);
        //Color32 w_ReplacementColor = new Color32(255, 0, 255, 0);
        Texture2D textureWithAlpha = AddAlphaChannel(originalSprite.texture, targetColor);

        // Generate Unique Asset Path for new Texture
        //string newAssetPath = Path.GetDirectoryName(spriteAssetPath) + "/" + Path.GetFileNameWithoutExtension(spriteAssetPath) + "_ARGB32_" + Path.GetExtension(spriteAssetPath);
        string newAssetPath = Path.GetDirectoryName(spriteAssetPath) + "/" + Path.GetFileNameWithoutExtension(spriteAssetPath) + "_ARGB32_" + ".png";
        string newUniqueAssetPath = AssetDatabase.GenerateUniqueAssetPath(newAssetPath);

        // Save Transparent Spritesheet as PNG
        Sprite spriteWithAlpha = SaveTextureAsSprite(textureWithAlpha, newUniqueAssetPath);
        // Clean Memory
        UnityEngine.Object.DestroyImmediate(textureWithAlpha);
        // Set Import Settings, and Slice
        SetRawCharacterSpriteSheetTextureImporter(spriteWithAlpha, true, true, false);

        return spriteWithAlpha;
    }

    public static Sprite SaveTextureAsSprite(Texture2D tex, string uniqueAssetPath)
    {
        //Wite Texture to File
        byte[] bytes = tex.EncodeToPNG();
        System.IO.File.WriteAllBytes(uniqueAssetPath, bytes);        // Editor Folder!
        bytes = null;

        // Importiere eben geschriebene Datei als Asset
        AssetDatabase.ImportAsset(uniqueAssetPath);
       
        // Get imported Asset
        Sprite textureAsset = AssetDatabase.LoadAssetAtPath(uniqueAssetPath, typeof(Sprite)) as Sprite;

        return textureAsset;
    }

    public static Texture2D AddAlphaChannel(Texture2D originalTexture, Color32 targetColor)
    {
        // Generate Texture with Alpha Channel
        Texture2D textureWithAlpha = new Texture2D(originalTexture.width, originalTexture.height, TextureFormat.ARGB32, false);

        // Alphachannel schreiben (TargetColor wird zu Alpha)
        ChangeTargetColorToAlpha(originalTexture, textureWithAlpha, targetColor); 

        return textureWithAlpha;
    }

    public static void ChangeTargetColorToAlpha(Texture2D originalTexture, Texture2D modifiedTexture, Color32 targetColor)
    {
        int targetColorFoundCount = 0;
        for (int y = 0; y < originalTexture.height; y++)
        {
            for (int x = 0; x < originalTexture.width; x++)
            {
                Color32 currentOriginalPixelColor = originalTexture.GetPixel(x, y);
                byte alphaValue = 255;
                if (currentOriginalPixelColor.Equals(targetColor))
                {
                    targetColorFoundCount++;
                    alphaValue = 0;
                }

                Color32 modifiedTexturePixelColor = new Color32(currentOriginalPixelColor.r,
                                                                currentOriginalPixelColor.g,
                                                                currentOriginalPixelColor.b,
                                                                alphaValue);

                modifiedTexture.SetPixel(x, y, modifiedTexturePixelColor);
            }
        }
        if (targetColorFoundCount > 0)
        {
            modifiedTexture.Apply();
        }
        Debug.Log("TargetColor: " + targetColor.ToString() + " found " + targetColorFoundCount + " times");
    }

    public static Sprite SetupSpriteTextureImporter(Sprite sprite, float pixelPerUnit, bool freadable, bool alphaIsTransparency)
    {
        // Asset
        string spriteAssetPath = AssetDatabase.GetAssetPath(sprite);
        TextureImporter texImporter = (TextureImporter)TextureImporter.GetAtPath(spriteAssetPath);
        texImporter.textureType = TextureImporterType.Advanced;

        // Importsettings
        TextureImporterSettings texImportSettings = new TextureImporterSettings();
        texImportSettings.spriteMode = (int)SpriteImportMode.Single;
        texImportSettings.spritePixelsPerUnit = pixelPerUnit;
        texImportSettings.wrapMode = TextureWrapMode.Clamp;
        texImportSettings.filterMode = FilterMode.Point;
        texImportSettings.mipmapEnabled = false;
        texImportSettings.maxTextureSize = 1024;
        texImportSettings.textureFormat = TextureImporterFormat.ARGB32;
        texImportSettings.readable = freadable;
        texImportSettings.alphaIsTransparency = alphaIsTransparency;

        //Apply Texture Import Settings
        texImporter.SetTextureSettings(texImportSettings);

        //Save changes
        AssetDatabase.ImportAsset(spriteAssetPath, ImportAssetOptions.ForceUpdate);
        AssetDatabase.Refresh();

        //Load modified Sprite 
        sprite = AssetDatabase.LoadAssetAtPath(spriteAssetPath, typeof(Sprite)) as Sprite;
        return sprite;
    }

    public static Sprite SetRawCharacterSpriteSheetTextureImporter(Sprite sprite, bool fReadAble, bool fAlpha, bool fSlice)
    {
        // Asset
        string spriteAssetPath = AssetDatabase.GetAssetPath(sprite);
        TextureImporter texImporter = (TextureImporter)TextureImporter.GetAtPath(spriteAssetPath);
        texImporter.textureType = TextureImporterType.Advanced;

        // Raw Character Spritesheet Importsettings
        TextureImporterSettings texImportSettings = new TextureImporterSettings();
        texImportSettings.spriteMode = (int)SpriteImportMode.Single;    // will be multiple thourg TextureImporter directly
        texImportSettings.spritePixelsPerUnit = 32f;
        texImportSettings.wrapMode = TextureWrapMode.Clamp;
        texImportSettings.filterMode = FilterMode.Point;
        texImportSettings.mipmapEnabled = false;
        texImportSettings.maxTextureSize = 1024;
        texImportSettings.textureFormat = TextureImporterFormat.ARGB32;
        texImportSettings.readable = fReadAble;
        texImportSettings.alphaIsTransparency = fAlpha;
        //texImportSettings.ApplyTextureType(TextureImporterType.Advanced, false);

        //Apply Texture Import Settings
        texImporter.SetTextureSettings(texImportSettings);

        // Slicing
        if (fSlice)
        {
            //SpriteImportMode.Multiple
            int subSpriteCount = 6;
            SpriteAlignment spriteAlignment = SpriteAlignment.Center;
            Vector2 customAlignment = new Vector2(0.5f, 0.5f);
            int subSpritePixelWidth = 32, subSpritePixelHeight = 32;
            //Generate MetaData
            List<SpriteMetaData> metaData = CalculateSpriteMetaData(texImporter, subSpriteCount, spriteAlignment, customAlignment, subSpritePixelWidth, subSpritePixelHeight);
            //Apply Meta Data and Import Mode
            if (metaData != null)
            {
                texImporter.spriteImportMode = SpriteImportMode.Multiple;
                texImporter.spritesheet = metaData.ToArray();
            }
            else
                Debug.LogError("spritesheet metadata == NULL!!! - slice error");
        }

        //Save changes
        AssetDatabase.ImportAsset(spriteAssetPath, ImportAssetOptions.ForceUpdate);
        AssetDatabase.Refresh();

        //Load modified Sprite 
        sprite = AssetDatabase.LoadAssetAtPath(spriteAssetPath, typeof(Sprite)) as Sprite;
        return sprite;
    }

    public static List<SpriteMetaData> CalculateSpriteMetaData(TextureImporter spriteImporter, int subSpritesCount, SpriteAlignment spriteAlignment, Vector2 customOffset, int pixelSizeWidth, int pixelSizeHeight)
    {
        bool failed = false;
        List<SpriteMetaData> metaDataList = new List<SpriteMetaData>();

        // Calculate SpriteMetaData (sliced SpriteSheet)
        for (int i = 0; i < subSpritesCount; i++)
        {
            try
            {

                SpriteMetaData spriteMetaData = new SpriteMetaData
                {
                    alignment = (int)spriteAlignment,
                    border = new Vector4(),
                    name = System.IO.Path.GetFileNameWithoutExtension(spriteImporter.assetPath) + "_" + i,
                    pivot = GetPivotValue(spriteAlignment, customOffset),
                    rect = new Rect(i * pixelSizeWidth, 0, pixelSizeWidth, pixelSizeHeight)
                };

                metaDataList.Add(spriteMetaData);

            }
            catch (Exception exception)
            {
                failed = true;
                Debug.LogException(exception);
            }
        }

        return metaDataList;
    }

    //SpriteEditorUtility
    public static Vector2 GetPivotValue(SpriteAlignment alignment, Vector2 customOffset = default (Vector2))
    {
        switch (alignment)
        {
            case SpriteAlignment.Center:
                return new Vector2(0.5f, 0.5f);
            case SpriteAlignment.TopLeft:
                return new Vector2(0.0f, 1f);
            case SpriteAlignment.TopCenter:
                return new Vector2(0.5f, 1f);
            case SpriteAlignment.TopRight:
                return new Vector2(1f, 1f);
            case SpriteAlignment.LeftCenter:
                return new Vector2(0.0f, 0.5f);
            case SpriteAlignment.RightCenter:
                return new Vector2(1f, 0.5f);
            case SpriteAlignment.BottomLeft:
                return new Vector2(0.0f, 0.0f);
            case SpriteAlignment.BottomCenter:
                return new Vector2(0.5f, 0.0f);
            case SpriteAlignment.BottomRight:
                return new Vector2(1f, 0.0f);
            case SpriteAlignment.Custom:
                return customOffset;
            default:
                return Vector2.zero;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="spriteImporter"></param>
    /// <param name="subSpritesCount">=6</param>
    /// <param name="spriteAlignment">=Center</param>
    /// <param name="customOffset">new Vector2 (0.5f,0.5f)</param>
    /// <param name="pixelPerUnit">=32</param>
    /// <param name="pixelSizeWidth">=32</param>
    /// <param name="pixelSizeHeight">=32</param>
    private void PerformMetaSlice(TextureImporter spriteImporter, int subSpritesCount, SpriteAlignment spriteAlignment, Vector2 customOffset, int pixelPerUnit, int pixelSizeWidth, int pixelSizeHeight)
    {
        if (spriteImporter != null)
        {
            Debug.Log("PerformMetaSlice: " + spriteImporter.assetPath);
            //			TextureImporter myImporter = null;
            //			myImporter = TextureImporter.GetAtPath (AssetDatabase.GetAssetPath(sprite)) as TextureImporter ;

            bool failed = false;
            List<SpriteMetaData> metaDataList = new List<SpriteMetaData>();

            //TODO abfragen ob sprite <multiple> ist
            //TODO abfragen ob größe stimmt, überspringen ???

            // falls multiple 

            // Calculate SpriteMetaData (sliced SpriteSheet)
            for (int i = 0; i < subSpritesCount; i++)
            {
                try
                {

                    SpriteMetaData spriteMetaData = new SpriteMetaData
                    {
                        alignment = (int)spriteAlignment,
                        border = new Vector4(),
                        name = System.IO.Path.GetFileNameWithoutExtension(spriteImporter.assetPath) + "_" + i,
                        pivot = GetPivotValue(spriteAlignment, customOffset),
                        rect = new Rect(i * pixelSizeWidth, 0, pixelSizeWidth, pixelSizeHeight)
                    };

                    metaDataList.Add(spriteMetaData);

                }
                catch (Exception exception)
                {
                    failed = true;
                    Debug.LogException(exception);
                }
            }

            if (!failed)
            {
                spriteImporter.spritePixelsPerUnit = pixelPerUnit;                  // setze PixelPerUnit
                spriteImporter.spriteImportMode = SpriteImportMode.Multiple;        // setze Multiple Mode
                spriteImporter.spritesheet = metaDataList.ToArray();                // weiße metaDaten zu

                // speichere ImportSettings
                EditorUtility.SetDirty(spriteImporter);

                try
                {
                    AssetDatabase.StartAssetEditing();
                    AssetDatabase.ImportAsset(spriteImporter.assetPath);
                }
                catch (Exception e)
                {
                    Debug.LogError("wtf " + e.ToString());
                }
                finally
                {
                    AssetDatabase.StopAssetEditing();
                    //					myImporter.SaveAndReimport();
                    //Close();
                }
            }
            else
            {
                Debug.LogError(spriteImporter.assetPath + " failed");
                SpriteAssetInfo(spriteImporter);
            }
        }
        else
        {
            Debug.LogError(" sprite == null");
        }
    }

    void SpriteAssetInfo(TextureImporter importer)
    {
        // Spriteinfo ausgeben
        Debug.Log("Path = " + importer.assetPath);
        Debug.Log("Import Mode = " + importer.spriteImportMode.ToString());
        Debug.Log("Pixel Per Unit = " + importer.spritePixelsPerUnit.ToString());
    }
}
