#define DEBUGGING

using UnityEngine;
using UnityEditor;
using System.IO;

namespace UnityEnhancements
{
    public static class ScriptableObjectUtility
    {
        /// <summary>
        //	This makes it easy to create, name and place unique new ScriptableObject asset files.
        /// </summary>
		public static T CreateAsset<T>( string projectRelativeAssetPath = "Assets", string assetName = "", bool focusInProjectWindow = false ) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();

            // FIXED
            //// muss ebenfalls, wie in AssetTools.CreateFolder() abgefragt werden, da CreateFolder() den Pfad ggf. manipuliert ABER nicht zurückgibt 
            //if (!projectRelativeAssetPath.StartsWith("Assets/"))
            //{
            //    if (!projectRelativeAssetPath.Equals("Assets"))
            //        projectRelativeAssetPath = "Assets/" + projectRelativeAssetPath;
            //}

#if DEBUGGING
            Debug.Log("assetPath = " + projectRelativeAssetPath);
#endif
            string createdFolderPath = "";
            if (!AssetTools.TryCreateFolderWithAssetDatabase(projectRelativeAssetPath, out createdFolderPath))
            {
                if (createdFolderPath.Equals(""))
                {
                    projectRelativeAssetPath = "Assets";
                    Debug.LogError("CreateFolder " + projectRelativeAssetPath + " failed, save @ Assets root!");
                }
            }
            else
                projectRelativeAssetPath = createdFolderPath;

            if (assetName.Equals(""))
            {
                assetName = "New " + typeof(T).ToString() + ".asset";
            }
            else if (!assetName.EndsWith(".asset"))
            {
                assetName += ".asset";
            }

#if DEBUGGING
            Debug.Log("assetName = " + assetName);
#endif

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(projectRelativeAssetPath + "/" + assetName);

            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

			if (focusInProjectWindow)
			{
	            EditorUtility.FocusProjectWindow();
	            Selection.activeObject = asset;
			}

            return asset;
        }
    }
}
 