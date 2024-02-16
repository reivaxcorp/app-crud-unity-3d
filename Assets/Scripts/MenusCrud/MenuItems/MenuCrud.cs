using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuCrud : MonoBehaviour, IFileSelected, IResult
{
    [SerializeField] MenuManagerApp uiApp;
    [SerializeField] MenuDialogConfirm dialogMsj;
    [SerializeField] AndroidPermission androidPermission;
    [SerializeField] ReceiverMessagesFromAndroid receiverMessagesFromAndroid;
    [SerializeField] ManageItems manageItems;
    [SerializeField] Image menuImagePreview;
    [SerializeField] protected TextMeshProUGUI resultMsj;
    [SerializeField] protected TMP_InputField inputFieldName;
    protected ProgressText progressText;
    protected bool isImageChanged;

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

    public void SetResultWriteDocument(bool successful, string title, string body)
    {
        if(successful)
        {
            dialogMsj.ShowDialog(title, body);
            uiApp.HideMenu();
        }
    }

    public void FileSelectedResultEditor(string path)
    {
        byte[] fileData = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData); // Esta línea convierte los datos de la imagen en la textura
        SetImagePreview(texture);
        this.isImageChanged = true;
    }

    public void SetImagePreview(Texture2D texture)
    {
        // Crea un sprite con la textura cargada
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        // Asigna el sprite al componente Image
        menuImagePreview.sprite = sprite;
    }

    public void SetImageName(string imageName)
    {
        inputFieldName.text = imageName;
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

    public void HideMenu()
    {
        uiApp.HideMenu();
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
        androidPermission.OnPermissionResult += HandlePermissionResult;
        androidPermission.RequestStoragePermission();
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
        resultMsj.text = string.Empty;
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
        receiverMessagesFromAndroid.SetCurrentMenu(menu);
        uiApp.SetCurrentMenu(menu);
    }

    private void ResetMenu()
    {
        menuImagePreview.sprite = null;
        ClearResultCrud();
        inputFieldName.text = "";
        SetCurrentMenu(null);
        this.waitForFirebaseSdk = true;
        this.isImageChanged = false;
    }

    private void Awake()
    {
        progressText = gameObject.AddComponent<ProgressText>();
        CheckReferences();
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

    private void CheckReferences()
    {
        if (uiApp == null) Debug.LogWarning("Coloca el script desde el UiApp (gameObject) el script MenuManagerApp en el Inspector");
        if (dialogMsj == null) Debug.LogWarning("Coloca el script DialogMsj desde el DialogMsj gameObject en MenuApp -> Canvas -> DialogMsj en el inspector");
        if (inputFieldName == null) Debug.LogWarning("InputFieldName no asignado en el Inspector");
        if (menuImagePreview == null) Debug.LogWarning("MenuImagePreview no asignado en el Inspector");
        if (receiverMessagesFromAndroid == null) Debug.LogWarning("Por favor coloca el script ReceiverMeesagesFromAndroid desde el Manager (gameObject) en el inspector");
        if (resultMsj == null) Debug.LogWarning("ResultMsj no está colocado en el inspector");
        if (androidPermission == null) Debug.LogWarning("Por favor coloca el script AndroidPermission desde el Manager (gameObject) en el inspector");
    }

}
