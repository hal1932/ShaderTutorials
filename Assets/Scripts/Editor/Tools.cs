using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Tools
{
    [MenuItem("ShaderTutorials/Capture GameView")]
    public static void CaptureGameView()
    {
        var fileName = $"{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.png";
        ScreenCapture.CaptureScreenshot(fileName);
        Debug.Log("ScreenShot: " + fileName);
    }

    //[MenuItem("ShaderTutorials/With Scaling")]
    //public static void WithScaling()
    //{
    //    var menuPath = "ShaderTutorials/With Scaling";
    //    Menu.SetChecked(menuPath, !Menu.GetChecked(menuPath));
    //}
}
