using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MenuAddItem : MonoBehaviour, IFileSelected
{
    [SerializeField] AndroidPermission androidPermission;
    [SerializeField] UnityReceiverFile unityReceiverFile;

    private FileManager fileManager;

    private void Start()
    {
        fileManager = new FileManager(this);
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

    public void FileSelectedResultEditor(string path)
    {
 
        if(unityReceiverFile != null)
        {
            unityReceiverFile.LoadTextureFromFile(path);
        }
        else
        {
            Debug.LogWarning("Please put UnityReceiverFile on inspector!");
        }
    }
}
