using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class FileManager: IMyImage 
{
    private IMyImage iImage;

    public FileManager(IMyImage iImage)
    {
        this.iImage = iImage;   
    }

    public void HandleSelectedFile(string filePath)
    {
        iImage.HandleSelectedFile(filePath);
    }

    public void OpenFile()
    {
        if (Application.isMobilePlatform)
        {
            AndroidJavaObject currentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer")
                .GetStatic<AndroidJavaObject>("currentActivity");

            AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent");
            intent.Call<AndroidJavaObject>("setAction", "android.intent.action.GET_CONTENT");
            intent.Call<AndroidJavaObject>("setType", "image/*");  // Filtra por archivos de imagen

            FilePickerAndroidCallback callback = new FilePickerAndroidCallback(iImage: this);

            currentActivity.Call("startActivityForResult", intent, 0, callback);
        }
        else if (Application.isEditor)
        {
            string path = EditorUtility.OpenFilePanel("Select Image", "", "png,jpg,jpeg,gif,bmp");
            if (!string.IsNullOrEmpty(path))
            {
                iImage.HandleSelectedFile(path);
            }
        }
        else
        {
            Debug.LogWarning("Platform not supported");
        }
    }

}
