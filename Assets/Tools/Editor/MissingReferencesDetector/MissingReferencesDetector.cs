using UnityEngine;
using UnityEditor;

public class MissingReferencesDetector : EditorWindow
{
    [MenuItem("Window/Missing References Detector")]
    public static void OpenWindow() 
    {
       EditorWindow window = EditorWindow.GetWindow<MissingReferencesDetector>();
       window.maxSize = new Vector2(250, 100);
       window.minSize = window.maxSize;

        GUIContent guiContent = new GUIContent();
        guiContent.text = "Find Missing References";
        window.titleContent = guiContent;
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();

        if(GUILayout.Button("Find missing references")) 
        {
            GameObject[] gameObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

            foreach (var gameobject in gameObjects)
            {
                Component[] components = gameobject.GetComponents<Component>();

                foreach (var component in components)
                {
                    SerializedObject serializedObject = new SerializedObject(component);
                    SerializedProperty serializedProperty =  serializedObject.GetIterator();
                    while(serializedProperty.Next(true))
                    {
                      if(serializedProperty.propertyType== SerializedPropertyType.ObjectReference) 
                      {
                         if(serializedProperty.objectReferenceValue == null) 
                         {
                                Debug.Log("<color=red><b>Missing reference:</b></color>" 
                                + serializedProperty.displayName + " on " + gameobject.name, gameobject
                                );
                         }
                      }
                    }
                }
            }

            EditorGUILayout.Space();
            Repaint();
        }
    }
}
