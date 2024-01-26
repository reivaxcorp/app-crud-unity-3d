using System.IO;
using UnityEditor;
using UnityEngine;

public class FileManager
{
    private IFileSelected fileSelected;
    private string currentImageName;
    private string folderUidName;

    public FileManager(IFileSelected fileSelected)
    {
        this.fileSelected = fileSelected;
    }

    /// <summary>
    /// We're going to use the user Id to create a folder with uid's name then we put all images there.
    /// </summary>
    public void SetFolderUidName()
    {
        if (FirebaseSDK.GetInstance() != null && FirebaseSDK.GetInstance().isFirebaseReady)
        {
            this.folderUidName = FirebaseSDK.GetInstance().auth.CurrentUser.UserId;
        }
    }

    public void SetCurrentImageName(string imageName)
    {
        this.currentImageName = imageName;
    }

    /// <summary>
    /// We're deleting previous image copy of app device.
    /// </summary>
    /// <param name="ImageName"></param>
    public void DeletePreviousCopyImage()
    {
        if (Application.isMobilePlatform)
        {
            if (currentImageName != null && folderUidName != null)
            {
                string filePath = Path.Combine(Application.persistentDataPath, folderUidName, currentImageName);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    Debug.Log("Archivo eliminado con éxito: " + currentImageName);
                }
            }
        }
    }

    public void SaveFileInternalExtorage(AndroidJavaObject uriObject, Texture2D texture, AndroidJavaObject currentActivity)
    {
        if (folderUidName != null && folderUidName.Length > 0)
        {
            // Redimensionar la textura a 512x512
            Texture2D resizedTexture = TextureScaler.ScaleTexture(texture, 512, 512);

            // Convertir la textura redimensionada a bytes
            byte[] bytesImage = resizedTexture.EncodeToPNG(); // Convierte la textura en formato PNG

            // Obtener información sobre la URI para obtener el nombre del archivo
            string[] projection = { "_display_name" };
            AndroidJavaObject cursor = currentActivity.Call<AndroidJavaObject>("getContentResolver")
                .Call<AndroidJavaObject>("query", uriObject, projection, null, null, null);

            if (cursor != null)
            {
                cursor.Call<bool>("moveToFirst");
                int columnIndex = cursor.Call<int>("getColumnIndex", "_display_name");
                string fileNameWithExtension = cursor.Call<string>("getString", columnIndex);
                cursor.Call("close");

                // Separar el nombre del archivo y la extensión
                string fileName = Path.GetFileNameWithoutExtension(fileNameWithExtension);

                // Crear la carpeta con el UID del usuario si no existe
                string userFolderPath = Path.Combine(Application.persistentDataPath, folderUidName);
                Directory.CreateDirectory(userFolderPath);

                string path = Path.Combine(userFolderPath, fileName + ".png"); // Ruta de destino del archivo PNG

                // Escribe los bytes en un archivo PNG
                File.WriteAllBytes(path, bytesImage); // Escribe los bytes en un archivo PNG

                SetCurrentImageName(fileName + ".png");

                // Puedes mostrar un mensaje de éxito o realizar otras acciones después de guardar la imagen
                Debug.Log("Imagen guardada con éxito en el almacenamiento interno de la aplicación");
            }
        }
        else
        {
            Debug.LogWarning("User uid doesn't exist");
        }
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

    private void OpenFileEditor()
    {
#if UNITY_EDITOR 
        string path = EditorUtility.OpenFilePanel("Select Image", "", "png,jpg,jpeg,gif,bmp");
        if (!string.IsNullOrEmpty(path))
        {
            fileSelected.FileSelectedResultEditor(path);
        }
#endif
    }

}
