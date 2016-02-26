using UnityEngine;
using UnityEditor;
using System.Collections;

namespace UnityEnhancements
{
    public class AssetTools
    {

		public static string GetProjectPath ()
		{
			string projectPath = Application.dataPath.Substring(0, Application.dataPath.Length - "/Assets".Length);
			return projectPath;
		}

		public static string GetAbsolutAssetPath (Object asset)
		{
			string relAssetPath = AssetDatabase.GetAssetPath (asset);
			if (string.IsNullOrEmpty (relAssetPath))
				return null;
			else
				return GetProjectPath() + "/" + relAssetPath;
		}

        public static string GenerateUniqueAssetPath(string projRelativeFilePath)
        {

            // remove Filename + extension from Filepath
            string createdPath = "";
            if (TryCreateFolderWithAssetDatabase(projRelativeFilePath, out createdPath))
            {
                return AssetDatabase.GenerateUniqueAssetPath(createdPath);
            }
            else
                return "";  // error
        }

        /// <summary>
        /// Creates the folder.
        /// CreateAssetsFolderWithAssetDatabase can only create Folder inside the Project/Assets Folder
        /// use CreateFolder if you want to create anywere!
        /// </summary>
        /// <returns><c>true</c>, if folder was created, <c>false</c> otherwise.</returns>
        /// <param name="pathRelativeToProjectPath">Path relative to project path. eg. Assets/Prefabs/Characters/Mario</param>
        public static bool TryCreateFolderWithAssetDatabase(string pathRelativeToProjectPath, out string assetPath)
        {
            if (!pathRelativeToProjectPath.StartsWith("Assets/"))
            {
                if (!pathRelativeToProjectPath.Equals("Assets"))
                    pathRelativeToProjectPath = "Assets/" + pathRelativeToProjectPath;
            }

            // Splitt
            string[] pathSegments = pathRelativeToProjectPath.Split(new char[] { '/' });

            // AssetDatabase workDirectory
            string accumulatedUnityFolder = pathSegments[0];   // should always be Assets

            // Complete Path (C:\...\...\...) System.IO workDirectory
            string accumulatedSystemFolder = Application.dataPath;

            //string projectFolder = "";              // = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length);      // Startwert C:/Users/Aryan/Documents/SuperMarioWars_UnityNetwork/Assets
            //projectFolder = Application.dataPath.Substring(0, Application.dataPath.Length - "/Assets".Length) ;

            //		Debug.Log("Unity : " + accumulatedUnityFolder);
            //		Debug.Log("System: " + accumulatedSystemFolder);

            string lastExistedFolder = accumulatedUnityFolder;  // should always be "Assets" before running the loop

            for (int i=1; i<pathSegments.Length; i++)
            {
                string folder = pathSegments[i];
                accumulatedSystemFolder += "/" + folder;
                accumulatedUnityFolder += "/" + folder;

                string guidFolder = "";
                if (!System.IO.Directory.Exists(accumulatedSystemFolder))
                {
                    //Debug.LogWarning(accumulatedSystemFolder + " Ordner existiert nicht!\n" +
                    //                  accumulatedUnityFolder + " Ordner existiert nicht!");

                    //Debug.Log("parentFolder = " + lastExistedFolder + " (letzter existierender Ordner)");
                    string guidParentFolder = AssetDatabase.AssetPathToGUID(lastExistedFolder);
                    if (guidParentFolder != "")
                    {
                        //Debug.Log("guidParentFolder = " + guidParentFolder);
                        guidFolder = AssetDatabase.CreateFolder(lastExistedFolder, folder);                         // TODO  ------------ WTF ordnerangabe geht!  GUID angabe geht nicht!!!!
                        if (guidFolder != "")
                        {
                            //Debug.Log(accumulatedSystemFolder + " Ordner wurder erfolgreich erstellt! \n" +
                            //           accumulatedUnityFolder + " Ordner wurder erfolgreich erstellt! \n");
                        }
                        else
                        {
                            //Debug.LogError("Ordner " + folder + " konnte in " + lastExistedFolder + " nicht erstellt werden!");
                            //						Debug.LogError (accumulatedSystemFolder + " konnte nicht erstellt werden! \n" +
                            //						                accumulatedUnityFolder + " konnte nicht erstellt werden! \n");
                            assetPath = lastExistedFolder;
                            return false;
                        }
                    }
                    else
                    {
                        // Parent Folder existiert nicht in AssetDatabase
                        //Debug.LogError(accumulatedSystemFolder + " guidParentFolder konnte nicht gefunden werden! \n" +
                        //                accumulatedUnityFolder + " guidParentFolder konnte nicht gefunden werden! \n");
                        assetPath = "";
                        return false;
                    }
                }
                else
                {
                    //				Debug.Log (accumulatedSystemFolder + " existiert! \n" +
                    //				           accumulatedUnityFolder + " existiert!");
                }

                lastExistedFolder = accumulatedUnityFolder;
            }
            assetPath = lastExistedFolder;
            return true;
        }


    }

}

