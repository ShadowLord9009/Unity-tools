using System;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class CustomHierarchyOptions 
{
    static CustomHierarchyOptions() 
    {
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGui;
    }

    private static void HierarchyWindowItemOnGui(int instanceID, Rect selectionRect)
    {
        DrawActiveToggleButton(instanceID, selectionRect);
        DrawInfoButton(instanceID, selectionRect,string.Empty);
        DrawZoomInButton(instanceID,selectionRect,"Frame this gameobject");
        DrawPrefabButton(instanceID, selectionRect, "Make prefab");
        DrawDeleteButton(instanceID,selectionRect,"Delete");
    }

    private static Rect DrawRect(float x, float y, float size) 
    {
        return new Rect(x, y, size, size);
    }

    private static void DrawButtonWithToggle(int id, float x, float y, float size) 
    {
        GameObject gameObject = (GameObject)EditorUtility.InstanceIDToObject(id);
        if (gameObject == null) return; 

        Rect rect= DrawRect(x, y, size);
        bool toggle = GUI.Toggle(rect, gameObject.activeSelf,string.Empty);
        gameObject.SetActive(toggle);
    }

    private static void DrawActiveToggleButton(int id, Rect rect) 
    {
        DrawButtonWithToggle(id, rect.x - 20, rect.y + 3, 10);
    }

    private static void DrawButtonWithTexture(float x, float y, float size, string name,
        Action action, GameObject gameObject,string tooltip) 
    { 
         if(gameObject == null) return;
        GUIStyle style = new GUIStyle();
        style.fixedHeight = 0;
        style.fixedWidth = 0;
        style.stretchHeight = true;
        style.stretchWidth = true;

        Rect rect = DrawRect(x, y, size);
        Texture texture = Resources.Load<Texture>(name);
        GUIContent guiContent = new GUIContent();
        guiContent.image = texture;
        guiContent.text = string.Empty;
        guiContent.tooltip = tooltip;
        bool isClicked = GUI.Button(rect, guiContent, style);
        if(isClicked ) 
        {
            action.Invoke();
        }
    }

    private static void DrawInfoButton(int id, Rect rect, string tooltip) 
    {
      GameObject gameObject = (GameObject)EditorUtility.InstanceIDToObject(id);
        
      if (gameObject != null && gameObject.TryGetComponent<Info>(out var info))
      {
            tooltip = info.info;
            DrawButtonWithTexture(rect.x +150,rect.y + 2, 14, "Info", () => { },gameObject,tooltip);
      }      
    }

    private static void DrawZoomInButton(int id, Rect rect, string tooltip) 
    {
      GameObject gameobject = (GameObject)EditorUtility.InstanceIDToObject(id);
      if (gameobject == null) return;

        Action action = () =>
        {
            Selection.activeGameObject = gameobject;
            SceneView.FrameLastActiveSceneView();             
        };
        DrawButtonWithTexture(rect.x + 175, rect.y + 2, 14, "zoomIn", action, gameobject, tooltip);
    }

    private static void DrawPrefabButton(int id, Rect rect, string tooltip) 
    {
        GameObject gameObject = (GameObject)EditorUtility.InstanceIDToObject(id);
        if (gameObject == null) return;

        Action action = () =>
        {
            const string pathToPrefabFolder = "Assets/Prefabs";
            bool isFolderExist = AssetDatabase.IsValidFolder(pathToPrefabFolder);
            if (!isFolderExist)
            {
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            }
            string prefabName = gameObject.name + ".prefab";
            string prefabPath = pathToPrefabFolder + "/" + prefabName;
            AssetDatabase.DeleteAsset(prefabPath);
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(gameObject, prefabPath);
            EditorGUIUtility.PingObject(prefab);
            PrefabUtility.ConvertToPrefabInstance(gameObject, prefab,new ConvertToPrefabInstanceSettings(),InteractionMode.AutomatedAction);
        };
        DrawButtonWithTexture(rect.x + 198, rect.y, 18, "makePrefab", action, gameObject, tooltip);
    }

    private static void DrawDeleteButton(int id, Rect rect, string tooltip) 
    {
        GameObject gameobject = (GameObject)EditorUtility.InstanceIDToObject(id);
        if (gameobject == null) return;

        Action action = () =>
        {
            UnityEngine.Object.DestroyImmediate(gameobject);
        };
        DrawButtonWithTexture(rect.x + 225, rect.y + 2, 14, "delete", action, gameobject, tooltip);
    
    }
}
