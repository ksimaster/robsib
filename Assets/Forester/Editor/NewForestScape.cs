using UnityEditor;
using UnityEngine;

namespace Forester
{
    public class NewForestScape : MonoBehaviour
    {

        [MenuItem("Tools/Forester/Create New ForestScape")]
        static void CreateForestScapeObj()
        {
            string assetPath = null;

            if (assetPath == null)
            {
                string[] directorys = System.IO.Directory.GetDirectories("Assets/");
                foreach (string dir in directorys)
                {
                    if (dir.Contains("Forester"))
                    {
                        assetPath = dir;
                    }
                }
                //If not found then check subdirectorys in case it is a plugins folder
                if (assetPath == null)
                {
                    for (int i = 0; i < directorys.Length; i++)
                    {
                        string[] subDirectorys = System.IO.Directory.GetDirectories(directorys[i]);
                        foreach (string dir in subDirectorys)
                        {
                            if (dir.Contains("Forester"))
                            {
                                assetPath = dir;
                            }
                        }
                    }
                }
            }
            Object newForest = Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(assetPath + "/InternalResources/Prefabs/ForestScape.prefab"));
            newForest.name = "ForestScape";
            //Undo.RegisterCreatedObjectUndo(newForest, "New Forest");
        }
    }
}
