using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;
using SMW.Import.Character;

namespace UnityEnhancements
{

    public static class IOTools
    {

        /// <summary>
        /// Usage:
        /// dInfo.GetFilesByExtensions(".jpg",".exe",".gif");
        /// </summary>
        /// <param name="dirInfo"></param>
        /// <param name="extensions"></param>
        /// <returns></returns>
        public static IEnumerable<FileInfo> GetFilesByExtensions(this DirectoryInfo dirInfo, params string[] extensions)
        {
            var allowedExtensions = new HashSet<string>(extensions, StringComparer.OrdinalIgnoreCase);

            return dirInfo.GetFiles()
                          .Where(f => allowedExtensions.Contains(f.Extension));
        }

        /// <summary>
        /// Usage:
        /// dInfo.GetFilesByExtensions(".jpg",".exe",".gif");
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="extensions"></param>
        /// <returns></returns>
        public static IEnumerable<FileInfo> GetFilesByExtensionsInEfficient(this DirectoryInfo dir, params string[] extensions)
        {
            if (extensions == null)
                throw new ArgumentNullException("extensions");
            IEnumerable<FileInfo> files = Enumerable.Empty<FileInfo>();
            foreach (string ext in extensions)
            {
                files = files.Concat(dir.GetFiles(ext));
            }
            return files;
        }

        public static FileInfo[] GetFileList(string absPath, string fileExtension)
        {
            if (!string.IsNullOrEmpty(absPath))
            {
                DirectoryInfo dir = new DirectoryInfo(absPath);
                FileInfo[] info = dir.GetFiles(fileExtension);


                // Einmalige ausgabe auf Console
                foreach (FileInfo f in info)
                {
                    //				Debug.Log("Found " + f.Name);
                    //				Debug.Log("f.DirectoryName=" + f.DirectoryName);
                    //				Debug.Log("f.FullName=" + f.FullName);
                    //				Debug.Log("modified=" + f.FullName.Substring(Application.dataPath.Length - "Assets".Length));
                    // relative pfad angabe
                    string currentSpritePath = f.FullName.Substring(Application.dataPath.Length - "Assets".Length);
                    Debug.Log("currentSpritePath=" + currentSpritePath);

                    //string charName = GetCharNameFromFileName(f.Name);
                    string charName = CharacterImport.GetInfoFromFileName(f.Name, FilenameFilter.CharacterName);
                    if (charName != null)
                    {
                        Debug.Log(charName);
                    }
                    else
                    {
                        Debug.LogError(f.Name + " konnte Character Name nicht extrahieren");
                    }
                }
                return info;
            }
            else
            {
                Debug.LogError("absPath == \"\" or NULL ");
                return null;
            }
        }
    }

}
