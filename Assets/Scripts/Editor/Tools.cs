using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    [MenuItem("ShaderTutorials/Create LutTexture")]
    public static void CreateLutTexture()
    {
        using (new AssetEditingScope())
        {
            var texturePath = "Assets/BaseLut2D.png";
            if (File.Exists(texturePath))
            {
                File.Delete(texturePath);
            }

            var texture = new Texture2D(LUT_TEXTURE_UNITDEPTH * LUT_TEXTURE_UNITDEPTH, LUT_TEXTURE_UNITDEPTH, TextureFormat.RGB24, false);

            void SetPixel(int r, int g, int b)
            {
                var x = b / LUT_TEXTURE_RESOLUTION * LUT_TEXTURE_UNITDEPTH + r / LUT_TEXTURE_RESOLUTION;
                var y = g / LUT_TEXTURE_RESOLUTION;
                //r -= LUT_TEXTURE_RESOLUTION / 2;
                //g -= LUT_TEXTURE_RESOLUTION / 2;
                //b -= LUT_TEXTURE_RESOLUTION / 2;
                texture.SetPixel(x, y, new Color(r / 255.0f, g / 255.0f, b / 255.0f));
            }

            for (var b = 0; b < 255; b += LUT_TEXTURE_RESOLUTION)
            {
                for (var g = 0; g < 255; g += LUT_TEXTURE_RESOLUTION)
                {
                    for (var r = 0; r < 255; r += LUT_TEXTURE_RESOLUTION)
                    {
                        SetPixel(r, g, b);
                    }
                }
            }

            texture.Apply();
            File.WriteAllBytes(texturePath, texture.EncodeToPNG());
        }
        Debug.Log("CreateLutTexture Complete");
    }

    [MenuItem("ShaderTutorials/LutTexture to 3D")]
    public static void LutTextureTo3D()
    {
        using (new AssetEditingScope())
        {
            var sourcePath = "Assets/BaseLut2D.png";
            var destinationPath = "Assets/BaseLut.asset";

            var source = new Texture2D(LUT_TEXTURE_UNITDEPTH * LUT_TEXTURE_UNITDEPTH, LUT_TEXTURE_UNITDEPTH, TextureFormat.RGB24, false);
            source.LoadImage(File.ReadAllBytes(sourcePath));

            var destination = new Texture3D(LUT_TEXTURE_UNITDEPTH, LUT_TEXTURE_UNITDEPTH, LUT_TEXTURE_UNITDEPTH, TextureFormat.RGB24, false);
            AssetDatabase.CreateAsset(destination, destinationPath);

            for (var b = 0; b < LUT_TEXTURE_UNITDEPTH; ++b)
            {
                for (var g = 0; g < LUT_TEXTURE_UNITDEPTH; ++g)
                {
                    for (var r = 0; r < LUT_TEXTURE_UNITDEPTH; ++r)
                    {
                        var src = source.GetPixel(b * LUT_TEXTURE_UNITDEPTH + r, g);
                        destination.SetPixel(r, g, b, src);
                    }
                }
            }

            destination.Apply();
            EditorUtility.SetDirty(destination);
        }
        Debug.Log("LutTextureTo3D Complete");
    }

    private const int LUT_TEXTURE_RESOLUTION = 8;
    private const int LUT_TEXTURE_UNITDEPTH = 256 / LUT_TEXTURE_RESOLUTION;
}
