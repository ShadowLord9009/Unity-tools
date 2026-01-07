using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

[CustomEditor(typeof(TextureImporter),true)]
public class ImageOptimizer : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Optimize")) 
        {

            AssetDatabase.StartAssetEditing();
            List<string> pathsToReimport = new List<string>();
            try
            {

                foreach (var targetObject in targets)
                {
                    string path = AssetDatabase.GetAssetPath(targetObject);
                    TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(path);
                    if (importer == null) continue;
                    int width = 0;
                    int height = 0;
                    importer.GetSourceTextureWidthAndHeight(out width, out height);
                    int maxDimensionSize = Mathf.Max(width, height);
                    if (maxDimensionSize <= 0) continue;

                    TextureImporterSettings importerSettings = new TextureImporterSettings();
                    importer.ReadTextureSettings(importerSettings);
                    int downPow = Mathf.IsPowerOfTwo(maxDimensionSize) ? maxDimensionSize / 2 : Mathf.NextPowerOfTwo(maxDimensionSize) / 2;
                    downPow = Mathf.Max(downPow, 64);

                    if (importerSettings.maxTextureSize != downPow)
                    {
                        importerSettings.maxTextureSize = downPow;
                        importer.SetTextureSettings(importerSettings);
                        pathsToReimport.Add(path);                      
                    }
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
                foreach (var path in pathsToReimport)
                {
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                }
            }
        }
    }
}
