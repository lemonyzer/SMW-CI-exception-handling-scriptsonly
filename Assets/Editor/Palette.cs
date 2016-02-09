using UnityEngine;
using System.Collections;

using System;
using UnityEditor;

[System.Serializable]
public class Palette : ScriptableObject
{
    //[SerializeField]
    //public Texture2D paletteTexture;
    //[SerializeField]
    //public int numcolors;
    //[SerializeField]
    //public Color32[][] colorcodes;

    [SerializeField]
    public Texture2D paletteTexture;

    [SerializeField]
    public Color[] rawReferenceColorPalette;

    [SerializeField]
    public Color[] teamColorPalette;

    /// <summary>
    /// Obsolete
    /// </summary>
    [SerializeField]
    public Color[] colorPalette;

    [SerializeField]
    public int numberOfTeams = 4;
    [SerializeField]
    public int singleColorPaletteWidth = 25;
    [SerializeField]
    public int singleColorPaletteHeight = 9;

    public void InitPalette ()
    {
        if (paletteTexture != null)
        {
            SetRawPaletteTextureImporter(paletteTexture);
            LoadTextureToPalette();
        }
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

    public Color[] ReadTeamColors()
    {
        Color[] colors = new Color[paletteTexture.width * numberOfTeams];

        int i = 0;
        for (int y = paletteTexture.height - 2; y >= 0; y=y-9)          // ab zweiter Zeile, springe 9 Zeien nach unten zu nächster Teamfarbe
        {
            for (int x = 0; x < paletteTexture.width; x++)
            {
                colors[i++] = paletteTexture.GetPixel(x, y);
            }
        }

        return colors;
    }

    public void LoadTextureToPalette ()
    {
        if (paletteTexture == null) {
            Debug.LogError(this.ToString() + "missing palette texture!");
            return;
        }

        rawReferenceColorPalette = ReadReferenceColors ();
        teamColorPalette = ReadTeamColors ();

        

        colorPalette = new Color[paletteTexture.width * paletteTexture.height];

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

        // Method C
        int i = 0;
        for (int y = paletteTexture.height -1; y >= 0; y--)
        {
            for (int x = 0; x < paletteTexture.width; x++)
            {
                colorPalette[i++] = paletteTexture.GetPixel(x, y);
            }
        }
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

    public static int mTeamColorCount = 4;
    public static int mTeamColorVariationsCount = 25;
    public static int mPaletteTeamColorOffset = 25;   // erste Reihe enthält andere Farbinformationen

    public Color[] GetTeamColorPalette(int teamId)
    {
        if (colorPalette == null || colorPalette.Length != (25 * 9 * 4 + 1 * 25))
        {
            return null;
        }

        if (teamColorPalette == null || teamColorPalette.Length != (25 * 4))
        {
            return null;
        }

        Color[] teamColor = new Color[mTeamColorVariationsCount];

        int index = teamId * mTeamColorVariationsCount;
        for (int i = 0; i < mTeamColorVariationsCount; i++)
        {
            teamColor[i] = teamColorPalette[index + i];
        }

        return teamColor;
    }

    public void ChangeColors(int teamId, Texture2D texture)
    {

        Color[] newColors = GetTeamColorPalette(teamId);

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
        Debug.Log("Anzahl an geänderten Pixel = " + fPixelChanged);
        texture.Apply();
    }

    public Color32 GetReferenceColorVariation(int colorVariationNumber)
    {
        return rawReferenceColorPalette[colorVariationNumber];
    }

    public Color32 GetTeamColorVariation(int teamId, int colorVariationNumber)
    {
        Color32 teamColorVariation;

        int index = teamId * mTeamColorVariationsCount;

        teamColorVariation = teamColorPalette[index + colorVariationNumber];

        return teamColorVariation;
    }

    public void LoadPalette ()
    {

        //numcolors = (short)palette->w;
        //numcolors = paletteTexture.width;

        // Init Color Array
        //for (int k=0; k<3; k++)
        //{
        //    colorcodes[k] = new uint8[numcolors];

        //    for (int i = 0; i < 4; i++)
        //        for (int j = 0; j < NUM_SCHEMES; j++)
        //            colorschemes[i][j][k] = new Uint8[numcolors];
        //}

        //int counter = 0;

        //Uint8* pixels = (Uint8*)palette->pixels;
        //short iRedIndex = palette->format->Rshift >> 3;
        //short iGreenIndex = palette->format->Gshift >> 3;
        //short iBlueIndex = palette->format->Bshift >> 3;

        //for (int k = 0; k < numcolors; k++)
        //{
        //    colorcodes[iRedIndex][k] = pixels[counter++];
        //    colorcodes[iGreenIndex][k] = pixels[counter++];
        //    colorcodes[iBlueIndex][k] = pixels[counter++];
        //}

        //counter += palette->pitch - palette->w * 3;

        //for (int i = 0; i < 4; i++)
        //{
        //    for (int j = 0; j < NUM_SCHEMES; j++)
        //    {
        //        for (int m = 0; m < numcolors; m++)
        //        {
        //            colorschemes[i][j][iRedIndex][m] = pixels[counter++];
        //            colorschemes[i][j][iGreenIndex][m] = pixels[counter++];
        //            colorschemes[i][j][iBlueIndex][m] = pixels[counter++];
        //        }

        //        counter += palette->pitch - palette->w * 3;
        //    }
        //}
    }


}
