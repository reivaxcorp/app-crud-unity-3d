using UnityEditor;
using UnityEngine;

public class FileManager : IMyImage
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

       // Llamar a tu actividad de Android
        AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityPlayer = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");

        // Crear el intent para obtener contenido
        AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent");
        intent.Call<AndroidJavaObject>("setAction", "android.intent.action.GET_CONTENT");
        intent.Call<AndroidJavaObject>("setType", "image/*");  // Filtra por archivos de imagen

        // Inicia la actividad personalizada con startActivityForResult
        int requestCode = 123; // Puedes cambiar este código a tu preferencia
        unityPlayer.Call("startActivityForResult", intent, requestCode, null);

    }

    public void ReceiveData(string fileUri)
    {
        // Lógica para manejar la URI del archivo en Unity
        Debug.Log("Received file URI in Unity: " + fileUri);
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
