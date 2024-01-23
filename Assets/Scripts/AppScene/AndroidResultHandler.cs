using UnityEngine;

/// <summary>
/// Handling Android Activity Result, when we selected a file
/// </summary>
public class AndroidResultHandler : AndroidJavaProxy
{

    public AndroidResultHandler() : base("com.reivaxcorp.unityappcrud.CrudUnityPlayerActivity") {}

    public void onActivityResult(int requestCode, int resultCode, AndroidJavaObject resultIntent)
    {

        Debug.Log("dddddddddddddddfffffffffffffffffffddddddddddddddddddddd");
        // Verifica que el resultado sea exitoso
        if (resultCode == -1 && resultIntent != null)
        {
            // Procesa la imagen aquí
            string imagePath = GetImagePathFromUri(resultIntent);
        }
    }

    private string GetImagePathFromUri(AndroidJavaObject uri)
    {
        string[] projection = { "_data" };
        AndroidJavaObject cursor = GetContentResolver().Call<AndroidJavaObject>(
            "query", uri, projection, null, null, null);

        if (cursor != null)
        {
            cursor.Call<bool>("moveToFirst");
            int columnIndex = cursor.Call<int>("getColumnIndex", "_data");
            string imagePath = cursor.Call<string>("getString", columnIndex);
            cursor.Call("close");
            return imagePath;
        }

        return null;
    }

    private AndroidJavaObject GetContentResolver()
    {
        AndroidJavaObject unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        return currentActivity.Call<AndroidJavaObject>("getContentResolver");
    }
}
