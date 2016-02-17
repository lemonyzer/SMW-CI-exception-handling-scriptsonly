#define DEBUGGING
#undef DEBUGGING

using UnityEngine;
using System.Collections;

using System;
using UnityEditor;
using System.Collections.Generic;

[System.Serializable]
public class TeamPalette
{
    [SerializeField]
    Color[] colors;

    public Color[] Colors { get {return colors;} set {colors = value;} }
}


[System.Serializable]
public class Palette : ScriptableObject
{
    [SerializeField]
    Texture2D paletteTexture;

    [SerializeField]
    Color[] rawReferenceColorPalette;

    [SerializeField]
    Color[] teamColorPaletteCombined;

    [SerializeField]
    List<TeamPalette> teamColorPaletteList;

    [SerializeField]
    int mSingleColorPaletteHeight = 9;

    [SerializeField]
    int mNumberOfTeamColor = 4;
    [SerializeField]
    int mTeamColorVariationsCount = 25;
    [SerializeField]
    int mPaletteTeamColorOffset = 2;   // erste Reihe enthält generische Farbinformationen


    public void InitPalette ()
    {
        if (paletteTexture != null)
        {
            SetRawPaletteTextureImporter(paletteTexture);
            LoadTextureToPalette();
            Save();
        }
    }

    void Save ()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty (this);
        //AssetDatabase.SaveAssets();
#endif
    }

    public Color[] ReadReferenceColors ()
    {
        Color[] colors = new Color[paletteTexture.width];
        int i = 0;
        int y = paletteTexture.height - 1;                              // erste Zeile enthält RAW Colors (Farben des generischen Spritesheets)
        for (int x = 0; x < paletteTexture.width; x++)
        {
            colors[i++] = paletteTexture.GetPixel(x, y);
        }
        return colors;
    }

    public Color[] ReadTeamColorsToArray()
    {
        Color[] colors = new Color[paletteTexture.width * mNumberOfTeamColor];

        int i = 0;
        for (int y = paletteTexture.height - mPaletteTeamColorOffset; y >= 0; y=y-mSingleColorPaletteHeight)          // ab zweiter Zeile, springe 9 Zeien nach unten zu nächster Teamfarbe
        {
            for (int x = 0; x < paletteTexture.width; x++)
            {
                colors[i++] = paletteTexture.GetPixel(x, y);
            }
        }

        return colors;
    }

    public List<TeamPalette> ReadTeamColorsToList()
    {
        List<TeamPalette> teamColorList = new List<TeamPalette>();

        int i = 0;
        for (int y = paletteTexture.height - mPaletteTeamColorOffset; y >= 0; y = y - mSingleColorPaletteHeight)          // ab zweiter Zeile, springe 9 Zeien nach unten zu nächster Teamfarbe
        {
            TeamPalette current = new TeamPalette();
            current.Colors = GetTeamColorPaletteFromTexture(i);
            teamColorList.Add(current);
            i++;
        }

        return teamColorList;
    }

    public void LoadTextureToPalette ()
    {
        if (paletteTexture == null) {
            Debug.LogError(this.ToString() + "missing palette texture!");
            return;
        }

        rawReferenceColorPalette = ReadReferenceColors ();
        teamColorPaletteCombined = ReadTeamColorsToArray ();
        teamColorPaletteList = ReadTeamColorsToList();

        //colorPalette = new Color[paletteTexture.width * paletteTexture.height];

        // starts bottom left...
        // Method A
        //colorPalette = paletteTexture.GetPixels();

        // Method B
        //int i = 0;
        //for (int y=0; y < paletteTexture.height; y++)
        //{
        //    for (int x=0; x < paletteTexture.width; x++)
        //    {
        //        colorPalette[i++] = paletteTexture.GetPixel(x, y);
        //    }
        //}

        // starts top left...
        // Method C
        //int i = 0;
        //for (int y = paletteTexture.height -1; y >= 0; y--)
        //{
        //    for (int x = 0; x < paletteTexture.width; x++)
        //    {
        //        colorPalette[i++] = paletteTexture.GetPixel(x, y);
        //    }
        //}
    }

    public static Texture2D SetRawPaletteTextureImporter(Texture2D texture)
    {
        // Asset
        string textureAssetPath = AssetDatabase.GetAssetPath(texture);
        TextureImporter texImporter = (TextureImporter)TextureImporter.GetAtPath(textureAssetPath);
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
        texImportSettings.readable = true;
        texImportSettings.alphaIsTransparency = false;
        //texImportSettings.ApplyTextureType(TextureImporterType.Advanced, false);

        //Apply Texture Import Settings
        texImporter.SetTextureSettings(texImportSettings);

        //Save changes
        AssetDatabase.ImportAsset(textureAssetPath, ImportAssetOptions.ForceUpdate);
        AssetDatabase.Refresh();

        //Load modified Sprite 
        texture = AssetDatabase.LoadAssetAtPath(textureAssetPath, typeof(Texture2D)) as Texture2D;
        return texture;
    }

    // Caching: Speicherintensiv, ArrayList muss aktualisiert werden sobald Texture aktualisiert wurde
    public Color[] GetTeamColorPaletteFromList(int teamId)
    {
        if (teamId >= 0 && teamId < teamColorPaletteList.Count)
        {
            if (teamColorPaletteList[teamId] != null)
                return teamColorPaletteList[teamId].Colors;
            else
                Debug.LogError("Palette der TeamID: " + teamId + " existiert nicht in Liste!");
        }
        else
            Debug.LogError("TeamID: " + teamId + " ausserhalb des erlaubten bereichs!");
        return null;
    }

    // Rechenintensiv
    // lieber cachen
    Color[] GetTeamColorPaletteFromTexture(int teamId)
    {
        int x = 0;
        int y = paletteTexture.height - mPaletteTeamColorOffset - teamId * mSingleColorPaletteHeight;
        return paletteTexture.GetPixels(x,y,mTeamColorVariationsCount,1);
    }

    // Rechenintensiv
    // lieber cachen
    public Color[] GetTeamColorPaletteSlow(int teamId)
    {
        if (teamColorPaletteCombined == null || teamColorPaletteCombined.Length != (mTeamColorVariationsCount * mNumberOfTeamColor))
        {
            return null;
        }

        Color[] teamColor = new Color[mTeamColorVariationsCount];

        int index = teamId * mTeamColorVariationsCount;
        for (int i = 0; i < mTeamColorVariationsCount; i++)
        {
            teamColor[i] = teamColorPaletteCombined[index + i];
        }

        return teamColor;
    }

    public void ChangeColorsSlow(int teamId, Texture2D texture)
    {
        // dynamical generated
        Color[] newColors = GetTeamColorPaletteSlow(teamId);

        // check if Sprite can be accessed
        //TODO
        if (texture.format != TextureFormat.ARGB32 &&
            texture.format != TextureFormat.RGBA32 &&
            texture.format != TextureFormat.RGBA4444)
        {
            Debug.LogError("texture format cant be accessed!!!");
            return;
        }


        // Anzahl der veränderten Pixel
        int fPixelChanged = 0;

        //		texture.filterMode = FilterMode.Bilinear;
        //		texture.wrapMode = TextureWrapMode.Clamp;

        for (int y = 0; y < texture.height; y++)
        {

            for (int x = 0; x < texture.width; x++)
            {
                Color32 currentColor = texture.GetPixel(x, y);

                Color32 newColor = new Color32();
                bool pixelHasReferenceColor = false;
                // schleife:
                // schaue ob aktueller Pixel einer der folgenden referenz Farben besitzt:

                // vorher ein bild, alle farben möglich (schleife über alle teamfarben nötig)
                // jetzt: ein raw bild unangetastet, immer nur raw reference werte möglich (keine schleife mehr nötig)
                //for (int iColor = 0; iColor < mTeamColorCount; iColor++)
                //{
                Color32 refColor;
                for (int iColorVariation = 0; iColorVariation < mTeamColorVariationsCount; iColorVariation++)
                {
                    refColor = GetReferenceColorVariation(iColorVariation);

                    if (currentColor.Equals(refColor))
                    {
                        //							newColor = TeamColor.referenceColors[fColorId,iColorIntensity];
                        newColor = newColors[iColorVariation];
                        pixelHasReferenceColor = true;
                        break;
                    }
                }
                //if (pixelHasReferenceColor)
                //    break;
                //}

                if (pixelHasReferenceColor)
                {
                    texture.SetPixel(x, y, newColor);
                    fPixelChanged++;
                }

            }
        }
#if DEBUGGING
        Debug.Log("Anzahl an geänderten Pixel = " + fPixelChanged);
#endif
        texture.Apply();
    }

    public void ChangeColorsSlowFix(int teamId, Texture2D texture)
    {
        // dynamical generated
        //Color[] newColors = GetTeamColorPaletteSlow(teamId);  // FIX

        // check if Sprite can be accessed
        //TODO
        if (texture.format != TextureFormat.ARGB32 &&
            texture.format != TextureFormat.RGBA32 &&
            texture.format != TextureFormat.RGBA4444)
        {
            Debug.LogError("texture format cant be accessed!!!");
            return;
        }

        // Anzahl der veränderten Pixel
        int fPixelChanged = 0;

        for (int y = 0; y < texture.height; y++)
        {

            for (int x = 0; x < texture.width; x++)
            {
                Color32 currentColor = texture.GetPixel(x, y);

                Color32 newColor = new Color32();
                bool pixelHasReferenceColor = false;
                // schleife:
                // schaue ob aktueller Pixel einer der folgenden referenz Farben besitzt:
                //for (int iColor = 0; iColor < mTeamColorCount; iColor++)
                // vorher ein bild, alle farben möglich (schleife über alle teamfarben nötig)
                // jetzt: ein raw bild unangetastet, immer nur raw reference werte möglich (keine schleife mehr nötig)

                Color32 refColor;
                for (int iColorVariation = 0; iColorVariation < mTeamColorVariationsCount; iColorVariation++)
                {
                    refColor = GetReferenceColorVariation(iColorVariation);

                    if (currentColor.Equals(refColor))
                    {
                        newColor = GetTeamColorVariationFromArray(teamId, iColorVariation); //FIX
                        pixelHasReferenceColor = true;
                        break;
                    }
                }

                if (pixelHasReferenceColor)
                {
                    texture.SetPixel(x, y, newColor);
                    fPixelChanged++;
                }

            }
        }
#if DEBUGGING
        Debug.Log("Anzahl an geänderten Pixel = " + fPixelChanged);
#endif
        texture.Apply();
    }

    public void ChangeColorsFast(int teamId, Texture2D texture)
    {
        // cached
        Color[] newColors = GetTeamColorPaletteFromList(teamId);

        // check if Sprite can be accessed
        //TODO
        if (texture.format != TextureFormat.ARGB32 &&
            texture.format != TextureFormat.RGBA32 &&
            texture.format != TextureFormat.RGBA4444)
        {
            Debug.LogError(this.ToString() + " " + this.paletteTexture.ToString() + " texture format cant be accessed!!!");
            return;
        }

        // Anzahl der veränderten Pixel
        int fPixelChanged = 0;

        for (int y = 0; y < texture.height; y++)
        {

            for (int x = 0; x < texture.width; x++)
            {
                Color32 currentColor = texture.GetPixel(x, y);
                Color32 newColor = new Color32();
                bool pixelHasReferenceColor = false;
                // schleife:
                // schaue ob aktueller Pixel einer der folgenden referenz Farben besitzt:
                //for (int iColor = 0; iColor < mTeamColorCount; iColor++)
                // vorher ein bild, alle farben möglich (schleife über alle teamfarben nötig)
                // jetzt: ein raw bild unangetastet, immer nur raw reference werte möglich (keine schleife mehr nötig)
                Color32 refColor;
                for (int iColorVariation = 0; iColorVariation < mTeamColorVariationsCount; iColorVariation++)
                {
                    refColor = GetReferenceColorVariation(iColorVariation);

                    if (currentColor.Equals(refColor))
                    {
                        newColor = newColors[iColorVariation];
                        pixelHasReferenceColor = true;
                        break;
                    }
                }

                if (pixelHasReferenceColor)
                {
                    texture.SetPixel(x, y, newColor);
                    fPixelChanged++;
                }

            }
        }
#if DEBUGGING
        Debug.Log("Anzahl an geänderten Pixel = " + fPixelChanged);
#endif
        texture.Apply();
    }

    public Color32 GetReferenceColorVariation(int colorVariationNumber)
    {
        return rawReferenceColorPalette[colorVariationNumber];
    }

    public Color32 GetTeamColorVariationFromArray(int teamId, int colorVariationNumber)
    {
        Color32 teamColorVariation;

        int index = teamId * mTeamColorVariationsCount;

        teamColorVariation = teamColorPaletteCombined[index + colorVariationNumber];

        return teamColorVariation;
    }

    public Color32 GetTeamColorVariationFromList(int teamId, int colorVariationNumber)
    {
        Color[] teamPalette = GetTeamColorPaletteFromList(teamId);
        if (teamPalette != null)
            return teamPalette[colorVariationNumber];
        else
        {
            Debug.LogError(this.ToString() + " teamPalette " + teamId + " == NULL!");
            return Color.magenta;
        }
    }

    public Color[] RawReferenceColorPalette { get { return rawReferenceColorPalette; } private set { rawReferenceColorPalette = value; } }

}
