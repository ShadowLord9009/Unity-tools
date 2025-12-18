using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Tools/Asset Organizer Config", fileName = "AssetOrganizerConfig")]
public class AssetOrganizerConfig : ScriptableObject
{
     public List<AssetTypeData> assetTypes = new();
}

[Serializable]
public class AssetTypeData
{
    public string name = "NewType";
    public List<string> extensions = new() { ".ext" };
    public string targetFolder = "Assets/";
}