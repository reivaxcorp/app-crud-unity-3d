using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Database;
using System.Runtime.InteropServices.ComTypes;

public class MenuAddItem : MonoBehaviour, IFileSelected, IResultCrud
{
    [SerializeField] AndroidPermission androidPermission;
    [SerializeField] Image menuImagePreview;
    [SerializeField] TextMeshProUGUI resultMsj;
    [SerializeField] TMP_InputField inputFieldName;

    public FileManager fileManager
    {
        private set { _fileManager = value; }
        get { return _fileManager; }
    }

    private bool waitForFirebaseSdk;
    private ProgressText progressText;
    private FileManager _fileManager;

    private void Awake()
    {
        progressText = gameObject.AddComponent<ProgressText>();
    }

    private void Start()
    {
        waitForFirebaseSdk = true;
        fileManager = new FileManager(this);
    }

    private void Update()
    {
        if (waitForFirebaseSdk)
        {
            if (FirebaseSDK.GetInstance().isFirebaseReady)
            {
                fileManager.SetFolderUidName();
                waitForFirebaseSdk = false;
            }
        }
    }

    // Acción del botón "Crear item"
    public void OnUploadItem()
    {
        if (IsDataSetted())
        {
            try
            {
                progressText?.StartProgressTextAnimation("Subiendo", resultMsj);
                byte[] fileBytes = _fileManager.GetBytesImageSelected();
                UploadFileRemote uploadFileRemote = new UploadFileRemote(fileBytes, _fileManager.folderUidName, _fileManager.currentImageName, iResult: this);
            }
            catch (Exception excepcion)
            {
                SetResultCrudUi(false, "Error - " + excepcion.Message);
            }
        }
    }

    public void SetResultCrudUi(bool exitoso, string msj)
    {
        progressText?.StopProgressTextAnimation();

        if (resultMsj != null)
        {
            if (exitoso)
            {
                resultMsj.text = msj;
                resultMsj.color = Color.green;
            }
            else
            {
                resultMsj.text = msj;
                resultMsj.color = Color.red;
            }
        }
        else
        {
            Debug.LogWarning("Por favor, coloca resultMsj en el Inspector");
        }
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
            Debug.LogWarning("MenuImagePreview no asignado en el Inspector");
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
            Debug.LogWarning("Plataforma no soportada");
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
            Debug.LogWarning("Por favor, coloca AndroidPermission en el GameManager");
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
                Debug.Log("¡Permiso concedido!");
                fileManager?.OpenFile();
                break;
            case PermissionStatus.Denied:
                Debug.LogWarning("Permiso denegado por el usuario.");
                break;
        }
    }

    // Desuscribirse para evitar pérdida de memoria
    private void DesuscribeEvent()
    {
        if (androidPermission != null)
        {
            // Desuscribir el evento OnPermissionResult
            androidPermission.OnPermissionResult -= HandlePermissionResult;
        }
    }

    private void ClearResultCrud()
    {
        if (resultMsj != null)
        {
            resultMsj.text = string.Empty;
        }
        else
        {
            Debug.LogWarning("ResultMsj no está colocado en el inspector");
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

    private bool IsDataSetted()
    {
        ClearResultCrud();

        if (inputFieldName == null)
        {
            LogWarningAndSetResult("InputFieldName no asignado en el Inspector");
            return false;
        }

        // Sanitizar el nombre de la imagen utilizando la expresión regular
        string sanitizedFileName = StringSanitizer.SanitizeString(inputFieldName.text);
        if (string.IsNullOrEmpty(sanitizedFileName))
        {
            LogWarningAndSetResult("Ingrese el nombre de la imagen");
            return false;
        }

        if (menuImagePreview == null)
        {
            LogWarningAndSetResult("MenuImagePreview no asignado en el Inspector");
            return false;
        }

        if (menuImagePreview.sprite == null)
        {
            LogWarningAndSetResult("Seleccione una imagen");
            return false;
        }

        return true;
    }

    private void LogWarningAndSetResult(string mensajeAdvertencia)
    {
        Debug.LogWarning(mensajeAdvertencia);
        SetResultCrudUi(false, mensajeAdvertencia);
    }

    public void ResultPathReference(string referenciaRuta)
    {
        // string id, string name, string path, string timestamp
        Debug.Log(referenciaRuta);
        String userUid = FirebaseSDK.GetInstance().auth.CurrentUser.UserId;
        DatabaseReference referenciaBaseDatos = FirebaseSDK.GetInstance().db.RootReference;
        string clave = referenciaBaseDatos.Child("users").Child("items").Child(userUid).Push().Key;
        ItemRemote itemRemoto = new ItemRemote();
    }
}
