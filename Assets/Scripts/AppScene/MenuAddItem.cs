using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MenuAddItem : MonoBehaviour, IFileSelected
{
    [SerializeField] AndroidPermission androidPermission;
     [SerializeField] Image menuImagePreview;

    private FileManager _fileManager;
    public FileManager fileManager
    {
        private set { _fileManager = value;  }
        get { return _fileManager; }
    }

    
    private void Start()
    {
        fileManager = new FileManager(this);
        fileManager.SetFolderUidName();
    }

    public void FileSelectedResultEditor(string path)
    {
        byte[] fileData = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData); // Esta línea convierte los datos de la imagen en la textura
        SetImagePreview(texture);
    }

    public void SetImagePreview(Texture2D texture)
    {
        // Crea un sprite con la textura cargada
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        // Asigna el sprite al componente Image
        if (menuImagePreview != null)
        {
            menuImagePreview.sprite = sprite;
        }
        else
        {
            Debug.LogError("Image component not assigned in the Inspector");
        }
    }

    public void OpenImage()
    {
        if (Application.isMobilePlatform)
        {
            OpenImageAndroid();
        }
        else if (Application.isEditor)
        {
            OpenImageEditor();
        }
        else
        {
            Debug.LogWarning("Platform not supported");
        }
    }

    private void OpenImageAndroid()
    {
        if (androidPermission != null)
        {
            androidPermission.OnPermissionResult += HandlePermissionResult;
            androidPermission.RequestStoragePermission();
        }
        else
        {
            Debug.LogWarning("Please put AndroidPermission on GameManager");
        }
    }

    private void OpenImageEditor()
    {
        fileManager?.OpenFile();
    }

    private void HandlePermissionResult(PermissionStatus status)
    {
        switch (status)
        {
            case PermissionStatus.Granted:
                Debug.Log("Permission granted!");
                fileManager?.OpenFile();
                break;
            case PermissionStatus.Denied:
                Debug.LogWarning("Permission denied by user.");
                break;
        }
    }

    // desuscribe to prevent memory leak
    private void DesuscribeEvent()
    {
        if (androidPermission != null)
        {
            // desuscribe event OnAccountCreated
            androidPermission.OnPermissionResult -= HandlePermissionResult;
        }
    }

    private void OnDisable()
    {
        DesuscribeEvent();
    }

    private void OnDestroy()
    {
        DesuscribeEvent();
    }

}
