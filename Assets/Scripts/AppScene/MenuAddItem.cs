using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MenuAddItem : MonoBehaviour, IMyImage
{
    [SerializeField] AndroidPermission androidPermission;
    [SerializeField] Image imagePreview;
    private FileManager fileManager;
    private string currentImagePath = "";

    private void Start()
    {
        fileManager = new FileManager(iImage: this);
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

    public void HandleSelectedFile(string filePath)
    {
        this.currentImagePath = filePath;

        Texture2D texture = LoadTextureFromFile(filePath);
        if (texture != null)
        {
            SetImagePreview(texture);
        }
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

    private void SetImagePreview(Texture2D texture)
    {
        // Crea un sprite con la textura cargada
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        // Asigna el sprite al componente Image
        if (imagePreview != null)
        {
            imagePreview.sprite = sprite;
        }
        else
        {
            Debug.LogError("Image component not assigned in the Inspector");
        }
    }


    private Texture2D LoadTextureFromFile(string filePath)
    {
        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData); // Esta línea convierte los datos de la imagen en la textura
        return texture;
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
