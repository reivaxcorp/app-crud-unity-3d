using UnityEngine;


/// <summary>
/// Handling Android Activity Result, when we selected a file
/// </summary>
public class FilePickerAndroidCallback : AndroidJavaProxy
{
    private IMyImage iImage;

    public FilePickerAndroidCallback(IMyImage iImage) : base("android.app.Activity$Runnable")
    {
        this.iImage = iImage;
    }

    // Handling Android ActivityOnResult
    public void run()
    {
        AndroidJavaObject currentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer")
               .GetStatic<AndroidJavaObject>("currentActivity");

        // Get the result from the intent
        AndroidJavaObject resultIntent = currentActivity.Call<AndroidJavaObject>("getIntent");

        // Check if the result is valid and contains data
        if (resultIntent != null && resultIntent.Call<bool>("hasExtra", "data"))
        {
            // Get the URI of the selected file
            AndroidJavaObject uri = resultIntent.Call<AndroidJavaObject>("getData");

            // Convert the URI to a file path string
            string filePath = GetPathFromUri(uri);
            // Handle the selected file
            this.iImage.HandleSelectedFile(filePath);
        }
    }

    // Convert URI to a file path string
    private string GetPathFromUri(AndroidJavaObject uri)
    {
        string[] projection = { "_data" };
        AndroidJavaObject cursor = GetContentResolver().Call<AndroidJavaObject>(
            "query", uri, projection, null, null, null);

        int columnIndex = cursor.Call<int>("getColumnIndexOrThrow", "_data");
        cursor.Call<bool>("moveToFirst");
        string filePath = cursor.Call<string>("getString", columnIndex);
        cursor.Call("close");

        return filePath;
    }

    private AndroidJavaObject GetContentResolver()
    {
        // Get the current context
        AndroidJavaObject currentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer")
            .GetStatic<AndroidJavaObject>("currentActivity");

        // Get the ContentResolver from the context
        return currentActivity.Call<AndroidJavaObject>("getContentResolver");
    }
}
