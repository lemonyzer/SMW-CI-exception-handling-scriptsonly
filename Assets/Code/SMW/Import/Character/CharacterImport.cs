using UnityEngine;
using System.Collections;

using UnityEditor;
using System;

namespace SMW.Import.Character
{

    public enum FilenameFilter
    {
        CharacterName = 2,
        CharacterTeam = 1
    }

    public class CharacterImport : EditorWindow
    {



        public static string GetInfoFromFileName(string fileName, FilenameFilter filter)
        {
            // old convention
            // BlackMage_alpha_rmFF00FF.png
            // charactername_alpha_rmFF00FF.png
            // hazey_Trainer_alpha_rmFF00FF
            // artist_charactername_alpha_rmFF00FF.png

            // new convention
            // BlackMage_red_ARGB32
            // %charactername%_%team%_ARGB32
            // hazey_Trainer_red_ARGB32
            // artist_%charactername%_%team%_ARGB32

            string[] splitted = SplittFileName(fileName);

            if (splitted == null)
            {
                Debug.LogError(fileName + " SpittFileName == null");
            }
            if (splitted.Length == 3 ||
                splitted.Length == 4)
            {
                if (!string.IsNullOrEmpty(splitted[splitted.Length - (int)filter]))
                    return splitted[splitted.Length - (int)filter];
            }

            Debug.LogError(fileName + " konnte Character namen nicht extrahieren");
            return null;
        }

        public static string[] SplittFileName(string fileName)
        {
            string[] result;
            char[] charSeparators = new char[] { '_' };
            //string[] stringSeparators = new string[] {"[stop]"};
            result = fileName.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
            return result;
        }

    }

}
