using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

public class AspectRatioTester : EditorWindow
{
    private static string applicationDataPath = $"{Application.dataPath}/AspectRatioTestScreenshots/";
    private static bool testAllRatios = false;
    private static bool screenshotInProgress = false;
    private static int currentRatioIndex = 0;
    private static string CurrentAspectRatio => aspectRatios[currentRatioIndex];
    private static Vector2 buttonSize = new Vector2(250, 50);

    private static List<string> aspectRatios = new List<string>()
    {
       applicationDataPath + "Free Aspect.png",
       applicationDataPath + "16x9 Aspect.png",
       applicationDataPath + "16x10 Aspect.png",
       applicationDataPath + "Full HD (1920 x 1080).png"
    };

    private void OnEnable()
    {
        EditorApplication.update += OnEditorUpdate;
    }

    [MenuItem("CustomTools/Aspect ratios tester")]
    public static void OpenWindow() 
    {
        Vector2 windowSize = new Vector2(300, buttonSize.y * aspectRatios.Count + buttonSize.y);
        var window = GetWindow<AspectRatioTester>();
        window.minSize = windowSize;
        window.maxSize = windowSize;
        window.Show();
    }

    private void OnGUI()
    {
        DrawTakeScreenShootButtons();
    }

    private static void DrawTakeScreenShootButtons() 
    {     
        var buttonWidth = GUILayout.Width(buttonSize.x);
        var buttonHeigh = GUILayout.Height(buttonSize.y);

        GUILayout.BeginVertical(GUILayout.ExpandHeight(true));

        for (int i = 0; i < aspectRatios.Count; i++)
        {
            string aspectRatioName = Path.GetFileNameWithoutExtension(aspectRatios[i]);

            if (GUILayout.Button($"Test {aspectRatioName} aspect ratio",buttonWidth,buttonHeigh))
            {
                screenshotInProgress =false;
                RemoveScreenshot(aspectRatios[i]);
                SaveScreenshotAtAspectRatio(i,aspectRatios[i]);
            }

        }

        if (GUILayout.Button("Test all aspect ratios",buttonWidth,buttonHeigh))
        {
            foreach (var aspectRatio in aspectRatios) 
            {
               RemoveScreenshot(aspectRatio);
            }

            screenshotInProgress = false;
            currentRatioIndex = 0;
            testAllRatios = true;                    
        }

        GUILayout.EndVertical();
    }

    private static void OnEditorUpdate()
    {   
        if (!testAllRatios) return;

        if (!screenshotInProgress) 
        { 
            SaveScreenshotAtAspectRatio(currentRatioIndex, CurrentAspectRatio);
            return;
        }

        if (File.Exists(CurrentAspectRatio))
        {
            currentRatioIndex += 1;
            screenshotInProgress = false;
            if (currentRatioIndex >= aspectRatios.Count)
            {
                testAllRatios = false;
                currentRatioIndex = 0;
                Refresh();
            }
        }
                      
    }

    private static void RemoveScreenshot(string fileName) 
    {
        if (File.Exists(fileName)) 
        {
           File.Delete(fileName);
        }
    }
    private static void SaveScreenshotAtAspectRatio(int index, string fileName)
    {       
        SetSize(index);
        TakeScreenshoot(fileName);      
    }

    public static void SetSize(int index)
    {
        var gameViewWindowType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
        var gameViewWindow = EditorWindow.GetWindow(gameViewWindowType);
        var sizeSelectionCallback = gameViewWindowType.GetMethod("SizeSelectionCallback",BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        sizeSelectionCallback.Invoke(gameViewWindow, new object[] { index,null });
    }

    public static void TakeScreenshoot(string filename)
    {
        if (!Directory.Exists(applicationDataPath)) 
        {
           Directory.CreateDirectory(applicationDataPath);
        }

        ScreenCapture.CaptureScreenshot(filename);
        screenshotInProgress = true;
    }

    public static void Refresh()
    {
        AssetDatabase.Refresh();
    }

    private void OnDisable()
    {
        EditorApplication.update -= OnEditorUpdate;
    }
}
