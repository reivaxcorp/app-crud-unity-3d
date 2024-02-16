using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class FileManager
{
    private IFileSelected fileSelected;
    private const int SIZE_WIDTH = 500;
    private const int SIZE_HEIGHT = 500;

    public string currentImageName
    {
        private set { _currentImageName = value; }
        get { return _currentImageName; }
    }
    public string folderUidName
    {
        private set { _folderUidName = value; }
        get { return _folderUidName; }
    }
    public string filePath
    {
        private set { _filePath = value; }
        get { return _filePath; }
    }


    private string _currentImageName;
    private string _folderUidName;
    private string _filePath;

    public FileManager(IFileSelected fileSelected)
    {
        this.fileSelected = fileSelected;
    }

    public void OpenFile()
    {
        if (Application.isMobilePlatform)
        {
            CreateIntentFileAndroid();
        }
        else if (Application.isEditor)
        {
            OpenFileEditor();
        }
        else
        {
            Debug.LogWarning("Plataforma no soportada");
        }
    }

    public byte[] GetBytesImageSelected()
    {
        if (Application.isMobilePlatform)
        {
            if (_currentImageName != null && _folderUidName != null)
            {
                string filePath = Path.Combine(Application.persistentDataPath, _folderUidName, _currentImageName);

                if (File.Exists(filePath))
                {
                    // Leer todos los bytes del archivo en filePath
                    byte[] imageBytes = File.ReadAllBytes(filePath);

                    // Ahora 'imageBytes' contiene los bytes de la imagen que puedes usar para subir a Firebase Storage.

                    Debug.Log("Archivo leído con éxito: " + _currentImageName);

                    return imageBytes;
                }
            }
        }
        else
        {
            if (_filePath != null)
            {
                return File.ReadAllBytes(_filePath);
            }
        }
        throw new Exception("Ruta de archivo no encontrada");
    }

    public string GetCurrentFilePath()
    {
        if (_currentImageName != null && _currentImageName.Length > 0 && _folderUidName != null)
        {
            return filePath;
        }
        else
        {
            if(_currentImageName == null || _currentImageName.Length == 0)
            {
                throw new Exception("Archivo de imagen invalido");
            } else if(_folderUidName == null) 
            {
                throw new Exception("Error en la carpeta");
            } else
            {
                throw new Exception("Dato invalido");
            }
        }
    }


    /// <summary>
    /// el nombre de la carpeta de nuestra cuenta, será nuestro Uid de usuario, donde colocaremos las imagenes
    /// de los items de esa cuenta.
    /// </summary>
    public void SetFolderUidName()
    {
        if (FirebaseSDK.GetInstance() != null && FirebaseSDK.GetInstance().isFirebaseReady)
        {
            this._folderUidName = FirebaseSDK.GetInstance().auth.CurrentUser.UserId;
        }
    }

    public void SetCurrentImageName(string imageName)
    {
        this._currentImageName = imageName;
    }

    /// <summary>
    /// Borramos la imagén anterior del dispositivo.
    /// </summary>
    /// <param name="ImageName"></param>
    public void DeletePreviousCopyImage()
    {
        if (Application.isMobilePlatform)
        {
            if (_currentImageName != null && _folderUidName != null)
            {
                string filePath = Path.Combine(Application.persistentDataPath, _folderUidName, _currentImageName);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    Debug.Log("Archivo eliminado con éxito: " + _currentImageName);
                }
            }
        }
    }

    /// <summary>
    /// Redimensionamos las texturas de los items
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="fileName"></param>
    public void SaveFileInternalExtorage(Texture2D texture, string fileName)
    {
        if (_folderUidName != null && _folderUidName.Length > 0)
        {
            // Redimensionar la textura a 512x512 px (si es necesario)
             Texture2D resizedTexture = TextureScaler.ScaleTexture(texture, SIZE_WIDTH, SIZE_HEIGHT);

            // Crear la carpeta con el UID del usuario si no existe
            string userFolderPath = Path.Combine(Application.persistentDataPath, _folderUidName);
            Directory.CreateDirectory(userFolderPath);

            string path = Path.Combine(userFolderPath, fileName); // Ruta de destino del archivo
            SetPathFile(path);

            // Escribir los bytes de la textura en un archivo PNG
            byte[] bytesImage = resizedTexture.EncodeToPNG();
            File.WriteAllBytes(path, bytesImage);

            SetCurrentImageName(fileName);

            // Puedes mostrar un mensaje de éxito o realizar otras acciones después de guardar la imagen
            Debug.Log("Imagen guardada con éxito en el almacenamiento interno de la aplicación");
        }
        else
        {
            Debug.LogWarning("User uid doesn't exist");
        }
    }

    private void CreateIntentFileAndroid()
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

    private void SetPathFile(string path)
    {
        this._filePath = path;
    }

    private void OpenFileEditor()
    {
#if UNITY_EDITOR 
        string path = EditorUtility.OpenFilePanel("Select Image", "", "png,jpg,jpeg,gif,bmp");
        if (!string.IsNullOrEmpty(path))
        {
            SetPathFile(path);
            string fileName = Path.GetFileName(path);
            SetCurrentImageName(fileName);
            fileSelected.FileSelectedResultEditor(path);
        }
#endif
    }

}
