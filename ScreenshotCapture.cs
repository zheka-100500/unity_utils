using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using UnityEditor;
[ExecuteAlways]
public class ScreenshotCapture : MonoBehaviour
{

    public bool Capture = false;
    public string FileName = "";
    public string FileDir = Application.dataPath + "/../Screens/";
    private string GetId() => Guid.NewGuid().ToString().Replace("_", "").Replace("-", "").Remove(10);
    private string GetFileName() => $"{GetId()}.png";

    [Range(0f, 1f)]
    public float ColorRange = 0.8f;

#if UNITY_EDITOR
    private void OnEnable() => FileName = GetFileName();

    void Update()
    {
        if(Capture)
        {
            Capture = false;

            if(!UnityEditor.EditorApplication.isPlaying)
            {
                UnityEditor.EditorApplication.EnterPlaymode();
                return;
            }
            

            if(string.IsNullOrEmpty(FileName)) FileName = GetFileName();
            var texture = ScreenCapture.CaptureScreenshotAsTexture();
            var pixels = texture.GetPixels(0, 0, texture.width, texture.height);
            var tempTexture = new Texture2D(texture.width, texture.height);
            tempTexture.SetPixels(0,0, texture.width, texture.height, pixels.Select(pixel => colorEquas(pixel) ? new Color(0, 0, 0, 0) : pixel).ToArray());
            tempTexture.Apply();
            if(!Directory.Exists(FileDir)) Directory.CreateDirectory(FileDir);
            File.WriteAllBytes(Path.Combine(FileDir, FileName), tempTexture.EncodeToPNG());
            FileName = GetFileName();
        }
    }

            private bool colorEquas(Color color) => color.g >= ColorRange;
#endif

}
