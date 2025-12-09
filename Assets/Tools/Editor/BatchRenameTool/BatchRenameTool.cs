using UnityEngine;
using UnityEditor;


public class BatchRenameTool : EditorWindow
{
    private string batchName = string.Empty;
    private string batchNumber = string.Empty;
    private bool showOptions = true;   

    [MenuItem("CustomTools/Batch rename tools")]
    public static void BatchRename() 
    {
       EditorWindow window = GetWindow<BatchRenameTool>();
       window.maxSize = new Vector2(500, 150);
       window.minSize = window.maxSize;

       GUIContent guiContent = new GUIContent();
       guiContent.text = "Batch rename tool";

       window.titleContent = guiContent;
       window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Step 1: Select objects in the hierarchy",EditorStyles.boldLabel);
        EditorGUILayout.Space(2);
        
        GUIStyle foldout = new GUIStyle(EditorStyles.foldout);
        foldout.fontStyle = FontStyle.Bold;
        showOptions = EditorGUILayout.Foldout(showOptions, "Step 2: Enter rename info", foldout);
        if (showOptions)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("\tEnter name for batch", EditorStyles.boldLabel);
            batchName = EditorGUILayout.TextField(batchName);
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("\tEnter starting number", EditorStyles.boldLabel);
            batchNumber = EditorGUILayout.TextField(batchNumber, EditorStyles.numberField);
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.LabelField("Step 3: Click the rename button", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();

        if (GUILayout.Button("Rename")) 
        {
            int num =(int.TryParse(batchNumber, out int number))? number:0;

            foreach (GameObject selectedObject in Selection.objects)
            {
                selectedObject.name = $"{num}_{batchName}";
                num++;
            }
        }
        EditorGUILayout.Space();
        EditorGUILayout.EndHorizontal();
        Repaint();  
    }
}
