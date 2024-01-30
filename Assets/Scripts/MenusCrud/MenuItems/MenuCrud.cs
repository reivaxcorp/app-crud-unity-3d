using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuCrud : MonoBehaviour, IFileSelected, IResultUi
{
    [SerializeField] AndroidPermission androidPermission;
    [SerializeField] ReceiverMessagesFromAndroid receiverMessagesFromAndroid;
    [SerializeField] Image menuImagePreview;
    [SerializeField] protected TextMeshProUGUI resultMsj;
    [SerializeField] protected TMP_InputField inputFieldName;
    protected ProgressText progressText;

    public FileManager fileManager
    {
        private set { _fileManager = value; }
        get { return _fileManager; }
    }

    private bool waitForFirebaseSdk;
    private FileManager _fileManager;

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
        string fileName = Path.GetFileName(path);
        SetImageName(fileName);
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

    public void SetImageName(string imageName)
    {

        if (inputFieldName != null)
        {
            inputFieldName.text = imageName;
        }
        else
        {
            LogWarningAndSetResult("InputFieldName no asignado en el Inspector");
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


    public bool IsDataSetted()
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

    private void OnEnable()
    {
        SetCurrentMenu(this);
    }

    private void OnDisable()
    {
        ResetMenu();
        DesuscribeEvent();
    }

    private void OnDestroy()
    {
        DesuscribeEvent();
    }

    private void LogWarningAndSetResult(string mensajeAdvertencia)
    {
        Debug.LogWarning(mensajeAdvertencia);
        SetResultCrudUi(false, mensajeAdvertencia);
    }

    private void SetCurrentMenu(MenuCrud menu)
    {
        if (receiverMessagesFromAndroid != null)
        {
            receiverMessagesFromAndroid.SetCurrentMenu(menu);
        }
        else
        {
            Debug.LogWarning("Por favor coloca El ReceiverMeesagesFromAndroid en el inspector");
        }
    }

    private void ResetMenu()
    {
        SetCurrentMenu(null);
        this.waitForFirebaseSdk = true;
    }

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
}
