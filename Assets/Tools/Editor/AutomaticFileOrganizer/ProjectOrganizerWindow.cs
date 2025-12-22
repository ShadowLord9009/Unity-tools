using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ProjectOrganizerWindow : EditorWindow
{
    private const string DefaultConfigAssetPath = "Assets/Tools/AssetOrganizerConfig.asset";

    private int _tabIndex;
    private readonly string[] _tabs = { "Organizer", "Asset Type Mappings" };

    private Vector2 _scroll;
    private AssetOrganizerConfig _config;

    [MenuItem("CustomTools/Project Organizer (Clean)")]
    public static void ShowWindow()
    {
        var window = GetWindow<ProjectOrganizerWindow>();
        window.titleContent = new GUIContent("Project Organizer");
        window.minSize = new Vector2(700, 350);
        window.Show();
    }

    private void OnEnable()
    {
        LoadOrCreateConfig();
        EnsureDefaultsIfEmpty();
    }

    private void OnGUI()
    {
        if (_config == null)
        {
            EditorGUILayout.HelpBox("Config is missing. Click Reload.", MessageType.Warning);
            if (GUILayout.Button("Reload"))
                LoadOrCreateConfig();
            return;
        }

        DrawTopBar();

        _scroll = EditorGUILayout.BeginScrollView(_scroll);
        EditorGUILayout.Space(6);

        if (_tabIndex == 0) DrawOrganizerTab();
        else DrawMappingsTab();

        EditorGUILayout.EndScrollView();
    }

    private void DrawTopBar()
    {
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            _tabIndex = GUILayout.Toolbar(_tabIndex, _tabs);

            EditorGUILayout.Space(6);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Save Config", GUILayout.Width(120)))
                {
                    SaveConfig();
                }

                if (GUILayout.Button("Organize Assets", GUILayout.Width(140)))
                {
                    OrganizeAssets();
                }

                GUILayout.FlexibleSpace();

                _config = (AssetOrganizerConfig)EditorGUILayout.ObjectField(
                    "Config",
                    _config,
                    typeof(AssetOrganizerConfig),
                    false,
                    GUILayout.MaxWidth(420)
                );
            }
        }
    }

    private void DrawOrganizerTab()
    {
        EditorGUILayout.LabelField("Organizer", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "Set target folders for each asset type. Then click 'Organize Assets'.\n" +
            "This moves assets on disk using AssetDatabase.MoveAsset (Unity-safe).",
            MessageType.Info
        );

        for (int i = 0; i < _config.assetTypes.Count; i++)
        {
            var t = _config.assetTypes[i];
            if (t == null) continue;

            using (new EditorGUILayout.VerticalScope("box"))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField($"Type: {SafeName(t.name)}", EditorStyles.boldLabel);

                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Remove", GUILayout.Width(80)))
                    {
                        _config.assetTypes.RemoveAt(i);
                        GUIUtility.ExitGUI();
                    }
                }

                t.targetFolder = EditorGUILayout.TextField("Target Folder", NormalizeAssetFolder(t.targetFolder));

                using (new EditorGUILayout.HorizontalScope())
                {
                    var folderObj = AssetDatabase.LoadAssetAtPath<DefaultAsset>(t.targetFolder);
                    var picked = (DefaultAsset)EditorGUILayout.ObjectField("Pick Folder", folderObj, typeof(DefaultAsset), false);

                    if (picked != null)
                    {
                        var path = AssetDatabase.GetAssetPath(picked);
                        if (AssetDatabase.IsValidFolder(path))
                            t.targetFolder = NormalizeAssetFolder(path);
                    }
                }

                EditorGUILayout.LabelField("Extensions", EditorStyles.miniBoldLabel);
                EditorGUILayout.LabelField(string.Join(", ", t.extensions.Select(NormalizeExtension)), EditorStyles.miniLabel);
            }
        }

        EditorGUILayout.Space(10);

        if (GUILayout.Button("Add New Asset Type", GUILayout.Height(28)))
        {
            _config.assetTypes.Add(new AssetTypeData
            {
                name = "NewType",
                extensions = new System.Collections.Generic.List<string> { ".ext" },
                targetFolder = "Assets/NewType"
            });
        }
    }

    private void DrawMappingsTab()
    {
        EditorGUILayout.LabelField("Asset Type Mappings", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "Define which file extensions belong to each type.\n" +
            "Keep extensions like .png, .prefab, .anim (with dot).",
            MessageType.Info
        );

        for (int i = 0; i < _config.assetTypes.Count; i++)
        {
            var t = _config.assetTypes[i];
            if (t == null) continue;

            using (new EditorGUILayout.VerticalScope("box"))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    t.name = EditorGUILayout.TextField("Name", t.name);

                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Remove", GUILayout.Width(80)))
                    {
                        _config.assetTypes.RemoveAt(i);
                        GUIUtility.ExitGUI();
                    }
                }

                EditorGUILayout.Space(4);

                t.targetFolder = EditorGUILayout.TextField("Target Folder", NormalizeAssetFolder(t.targetFolder));

                EditorGUILayout.Space(6);
                EditorGUILayout.LabelField("Extensions", EditorStyles.miniBoldLabel);

                if (t.extensions == null) t.extensions = new System.Collections.Generic.List<string>();

                for (int j = 0; j < t.extensions.Count; j++)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        t.extensions[j] = NormalizeExtension(EditorGUILayout.TextField(t.extensions[j]));

                        if (GUILayout.Button("X", GUILayout.Width(24)))
                        {
                            t.extensions.RemoveAt(j);
                            GUIUtility.ExitGUI();
                        }
                    }
                }

                if (GUILayout.Button("Add Extension", GUILayout.Width(120)))
                {
                    t.extensions.Add(".ext");
                }
            }
        }

        EditorGUILayout.Space(10);

        if (GUILayout.Button("Add New Asset Type", GUILayout.Height(28)))
        {
            _config.assetTypes.Add(new AssetTypeData
            {
                name = "NewType",
                extensions = new System.Collections.Generic.List<string> { ".ext" },
                targetFolder = "Assets/NewType"
            });
        }
    }

    private void OrganizeAssets()
    {
        if (_config.assetTypes.Count == 0)
        {
            Debug.LogWarning("No asset types configured.");
            return;
        }

        foreach (var t in _config.assetTypes)
        {
            t.name = SafeName(t.name);
            t.targetFolder = NormalizeAssetFolder(t.targetFolder);

            if (t.extensions == null) t.extensions = new System.Collections.Generic.List<string>();
            for (int i = 0; i < t.extensions.Count; i++)
                t.extensions[i] = NormalizeExtension(t.extensions[i]);
        }

        foreach (var t in _config.assetTypes)
        {
            EnsureFolderExists(t.targetFolder);
        }

        var assetsDiskPath = Application.dataPath; 
        var files = Directory.EnumerateFiles(assetsDiskPath, "*.*", SearchOption.TopDirectoryOnly)
            .Where(p => !p.EndsWith(".meta", StringComparison.OrdinalIgnoreCase));

        int moved = 0;
        int skipped = 0;

        AssetDatabase.StartAssetEditing();
        try
        {
            foreach (var fullPath in files)
            {
                var ext = NormalizeExtension(Path.GetExtension(fullPath));
                if (string.IsNullOrEmpty(ext)) { skipped++; continue; }

                var match = _config.assetTypes.FirstOrDefault(t => t.extensions.Contains(ext));
                if (match == null) { skipped++; continue; }

                var assetPath = FullPathToAssetPath(fullPath);
                if (string.IsNullOrEmpty(assetPath)) { skipped++; continue; }

                if (assetPath.StartsWith(match.targetFolder + "/", StringComparison.OrdinalIgnoreCase))
                {
                    skipped++;
                    continue;
                }

                var fileName = Path.GetFileName(assetPath);
                var destPath = $"{match.targetFolder}/{fileName}";
                destPath = AssetDatabase.GenerateUniqueAssetPath(destPath);

                var err = AssetDatabase.MoveAsset(assetPath, destPath);
                if (string.IsNullOrEmpty(err))
                    moved++;
                else
                    Debug.LogWarning($"Move failed: {assetPath} -> {destPath}\n{err}");
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
            AssetDatabase.Refresh();
        }

        Debug.Log($"Organize complete. Moved: {moved}, Skipped: {skipped}");
    }

    private void SaveConfig()
    {
        EditorUtility.SetDirty(_config);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Config saved.");
    }

    private void LoadOrCreateConfig()
    {
        _config = AssetDatabase.LoadAssetAtPath<AssetOrganizerConfig>(DefaultConfigAssetPath);

        if (_config == null)
        {
            var dir = Path.GetDirectoryName(DefaultConfigAssetPath)?.Replace("\\", "/");
            if (!string.IsNullOrEmpty(dir))
                EnsureFolderExists(dir);

            _config = CreateInstance<AssetOrganizerConfig>();
            AssetDatabase.CreateAsset(_config, DefaultConfigAssetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    private void EnsureDefaultsIfEmpty()
    {
        if (_config.assetTypes != null && _config.assetTypes.Count > 0) return;

        _config.assetTypes = new System.Collections.Generic.List<AssetTypeData>
        {
            new AssetTypeData { name = "Prefabs", extensions = new System.Collections.Generic.List<string>{ ".prefab" }, targetFolder = "Assets/Prefabs" },
            new AssetTypeData { name = "Animations", extensions = new System.Collections.Generic.List<string>{ ".anim" }, targetFolder = "Assets/Animations" },
            new AssetTypeData { name = "Images", extensions = new System.Collections.Generic.List<string>{ ".png", ".jpg", ".jpeg" }, targetFolder = "Assets/Images" },
        };

        SaveConfig();
    }

    private static string SafeName(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return "Unnamed";
        return s.Trim();
    }

    private static string NormalizeExtension(string ext)
    {
        if (string.IsNullOrWhiteSpace(ext)) return "";
        ext = ext.Trim();
        if (!ext.StartsWith(".")) ext = "." + ext;
        return ext.ToLowerInvariant();
    }

    private static string NormalizeAssetFolder(string folder)
    {
        if (string.IsNullOrWhiteSpace(folder)) return "Assets";
        folder = folder.Trim().Replace("\\", "/");
        if (!folder.StartsWith("Assets", StringComparison.OrdinalIgnoreCase))
            folder = "Assets/" + folder.TrimStart('/');
        return folder.TrimEnd('/');
    }

    private static void EnsureFolderExists(string assetFolderPath)
    {
        assetFolderPath = NormalizeAssetFolder(assetFolderPath);

        if (AssetDatabase.IsValidFolder(assetFolderPath))
            return;

        var parts = assetFolderPath.Split('/');
        string current = parts[0]; 
        for (int i = 1; i < parts.Length; i++)
        {
            var next = current + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(current, parts[i]);
            current = next;
        }
    }

    private static string FullPathToAssetPath(string fullPath)
    {
        fullPath = fullPath.Replace("\\", "/");
        var assetsRoot = Application.dataPath.Replace("\\", "/"); 
        if (!fullPath.StartsWith(assetsRoot, StringComparison.OrdinalIgnoreCase))
            return null;

        var rel = "Assets" + fullPath.Substring(assetsRoot.Length);
        return rel.Replace("\\", "/");
    }
}