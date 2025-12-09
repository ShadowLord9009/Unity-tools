using UnityEngine;
using UnityEditor;

public class ProjectOrganizerWindow : EditorWindow
{
    private int selectTabIndex = 0;
    private string[] tabs = { "Organizer","Asset Type Mappings" };

    [MenuItem("CustomTools/ProjectOrganizer")]
	public static void ShowWindow() 
    {
        EditorWindow window = GetWindow<ProjectOrganizerWindow>();
        GUIContent guiContent = new GUIContent();
        guiContent.text = "Project organizer";
        window.titleContent = guiContent;
        window.Show();
    }

    private void OnGUI()
    {
        DrawToolbarTabs();
    }

    private void DrawToolbarTabs()
    {
        GUILayout.BeginHorizontal();
        selectTabIndex = GUILayout.Toolbar(selectTabIndex,tabs);
        GUILayout.EndHorizontal();
    }
}
