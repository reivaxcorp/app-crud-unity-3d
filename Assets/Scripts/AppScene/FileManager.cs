using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class FileManager :  IMyImage
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
            OpenFileAndroid();
        }
        else if (Application.isEditor)
        {
            OpenFileEditor();
        }
        else
        {
            Debug.LogWarning("Platform not supported");
        }
    }

    private void OpenFileAndroid()
    {

        AndroidJavaObject unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");


        AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent");
        intent.Call<AndroidJavaObject>("setAction", "android.intent.action.GET_CONTENT");
        intent.Call<AndroidJavaObject>("setType", "image/*");  // Filtra por archivos de imagen

        // Utilizando AndroidJavaRunnable para ejecutar en el hilo principal
        activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
        {
            // currentActivity.Call("startActivityForResult", intent, 0, resultHandler);
            activity.Call("startActivityForResult", intent, 0, new AndroidResultHandler());
            Debug.Log("startActivityForResult dasfafadsf");
        }));
    }

        private void OpenFileEditor()
    {
#if UNITY_EDITOR 
        string path = EditorUtility.OpenFilePanel("Select Image", "", "png,jpg,jpeg,gif,bmp");
        if (!string.IsNullOrEmpty(path))
        {
            iImage.HandleSelectedFile(path);
        }
#endif
    }

}
