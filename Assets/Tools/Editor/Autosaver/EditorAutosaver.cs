using System;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class EditorAutosaver : EditorWindow
{
    private static EditorWindow window;
    private const string menuOption = "File/Autosave";
    private const string choiceKey = "choice";
    private static int choice = 0;
    private const string ONE_SECOND = "1 sec";
    private const string THIRTY_SECOND = "30 sec";
    private const string ONE_MINUTE = "1 min";
    private const string FIVE_MINUTES = "5 min";
    private string[] choices = new string[] {ONE_SECOND,THIRTY_SECOND,ONE_MINUTE,FIVE_MINUTES };
    private static float saveTime = 1;
    private static float nextSave = 0;
    public static bool IsEnable 
    {
        get { return EditorPrefs.GetBool(menuOption, false); }
        set 
        {
            EditorPrefs.SetBool(menuOption, value);          
        }
    }

    [MenuItem(menuOption,false,175)]
    public static void ToggleAutosave() 
    {
        IsEnable = !IsEnable;
        
        if(IsEnable) 
        {
           ShowWindow();
        }

        else 
        {
           CloseWindow();
        }
    }

    [MenuItem(menuOption, true)]
    public static bool ToggleActionValidate() 
    {
       Menu.SetChecked(menuOption, IsEnable);  
       return true;            
    }

    private static void CloseWindow()
    {
        if (window == null) return;

        window.Close();
    }

    private static void ShowWindow()
    {
        window = GetWindow(typeof(EditorAutosaver));
        GUIContent guiContent = new GUIContent();
        guiContent.text = "Autosave settings";
        window.titleContent = guiContent;
        window.Show();
    }

    static EditorAutosaver()
    {
        choice = EditorPrefs.GetInt(choiceKey, 0);
        EditorApplication.update += Update;
    }

    private static void Update() 
    {
        AutosaveLogic();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Interval: ");
        EditorGUILayout.Space();
        EditorGUI.BeginChangeCheck();
        choice = EditorGUILayout.Popup("", choice,choices); 
        if(EditorGUI.EndChangeCheck())
        {
            switch(choices[choice]) 
            {
                case ONE_SECOND:
                    saveTime = 1; 
                    break;

                case THIRTY_SECOND: 
                    saveTime = 30;
                    break;

                case ONE_MINUTE:
                    saveTime = 60;
                    break;

                case FIVE_MINUTES:
                    saveTime = 300;
                    break;          
            }

            EditorPrefs.SetInt(choiceKey, choice);
            ResetCounter();
        }     
    }

    private static void AutosaveLogic() 
    {
        if (!IsEnable) return;

        if (EditorApplication.timeSinceStartup > nextSave)
        {
            var scene = EditorSceneManager.GetActiveScene();
            if (!scene.isDirty || string.IsNullOrEmpty(scene.path)) return;
            bool saveSucess = EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            nextSave = (float)EditorApplication.timeSinceStartup + saveTime;
        }        
    }

    private void ResetCounter() 
    {
        nextSave = (float)EditorApplication.timeSinceStartup;
    }
}
