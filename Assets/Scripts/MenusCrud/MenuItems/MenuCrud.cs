using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class MenuCrud : MonoBehaviour, IResultDialog
{

    [SerializeField] MenuManagerApp uiApp;
    [SerializeField] GameObject ads;
    [SerializeField] MenuDialogConfirm dialogMsj;
    [SerializeField] AndroidPermission androidPermission;
    [SerializeField] ReceiverMessagesFromAndroid receiverMessagesFromAndroid;
    [SerializeField] ManageItems manageItems;
    [SerializeField] protected Image menuImagePreview;
    [SerializeField] protected TextMeshProUGUI resultMsj;
    [SerializeField] protected TMP_InputField inputFieldName;
    protected ProgressText progressText;
    protected bool isImageChanged;
    protected string imageNameGenerated;

    public FileManager fileManager
    {
        private set { _fileManager = value; }
        get { return _fileManager; }
    }

    private bool waitForFirebaseSdk;
    private FileManager _fileManager;


    public void OpenDialog(string title, string body)
    {
        dialogMsj.ShowDialog(title, body, this);
    }
 
    public void SetImagePreview(Texture2D texture)
    {
        // Crea un sprite con la textura cargada
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        // Asigna el sprite al componente Image
        menuImagePreview.sprite = sprite;
    }

    public void SetImageChange(bool isImageChanged)
    {
        this.isImageChanged = isImageChanged;
    }

    public void SetImageName(string imageName)
    {
        inputFieldName.text = imageName;
    }

    public void SetImageNameGenerate(string imageName)
    {
        this.imageNameGenerated = imageName;
    }

    public void HideMenu()
    {
        fileManager.DeletePreviousCopyImage();
        uiApp.HideMenu();
    }

    public void ShowInterstitialAd()
    {
        if (ads != null)
        {
            InterstitialAd interstitialAd = ads.GetComponent<InterstitialAd>();
            if (interstitialAd != null)
            {
                interstitialAd.ShowAd();
            }
            else
            {
                Debug.LogWarning("InterstitialAd no está en el UiAppp GameObject del inspector");
            }
        }
        else
        {
            Debug.LogWarning("Por favor, coloca el Ads en el MenuAddItem, en su inspector");
        }
    }

    public virtual void ConfirmButtonDialogPressed(bool isDialogConfirm)
    {
        if (isDialogConfirm) // cerro con el botón "Aceptar"
        {
            uiApp.HideMenu();
        }
        else
        {
            ClearMenu(); // cerro con el botón "X". 
        }
    }

    public void ResetMenu()
    {
        ClearInputs();
        WaitForFirebase(true);
        SetImageChange(false);
        ClearResultCrud();
        SetCurrentMenu(null);
    }

    public void ClearResultCrud()
    {
        resultMsj.text = string.Empty;
    }

    private void OpenImageAndroid()
    {
        androidPermission.OnPermissionResult += HandlePermissionResult;
        androidPermission.RequestStoragePermission();
    }  

    private void HandlePermissionResult(PermissionStatus status)
    {
        switch (status)
        {
            case PermissionStatus.Granted:
                Debug.Log("¡Permiso concedido!");
                fileManager.CreateIntentFileAndroid();
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
     
    private void OnEnable()
    {
        ResetMenu();
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

    private void SetCurrentMenu(MenuCrud menu)
    {
        receiverMessagesFromAndroid.SetCurrentMenu(menu);
        uiApp.SetCurrentMenu(menu);
    }
 
    private void ClearMenu()
    {
        ClearInputs();
        SetImageChange(false);
        ClearResultCrud();
    }

    private void ClearInputs()
    {
        menuImagePreview.sprite = null;
        inputFieldName.text = "";
    }

    private void WaitForFirebase(bool isWaitFirebase)
    {
        waitForFirebaseSdk = isWaitFirebase;
    }

    private void Awake()
    {
        progressText = gameObject.AddComponent<ProgressText>();
        CheckReferences();
    }

    private void Start()
    {
        WaitForFirebase(true);
        fileManager = new FileManager(FirebaseSDK.GetInstance().auth.CurrentUser.UserId);
    }

    private void Update()
    {
        if (waitForFirebaseSdk)
        {
            if (FirebaseSDK.GetInstance().isFirebaseReady &&
                FirebaseSDK.GetInstance().auth.CurrentUser != null)
            {
                fileManager.SetFolderUidName(FirebaseSDK.GetInstance().auth.CurrentUser.UserId);
                WaitForFirebase(false);
            }
        }
    }

    public void OpenFile()
    {
        if (Application.isMobilePlatform)
        {
            OpenImageAndroid();
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

    private void OpenFileEditor()
    {
#if UNITY_EDITOR 
        string path = EditorUtility.OpenFilePanel("Select Image", "", "png,jpg,jpeg,gif,bmp");
        if (!string.IsNullOrEmpty(path))
        {

            string fileName = Path.GetFileNameWithoutExtension(path);
            fileManager.SetCurrentImageName(fileName);

            byte[] fileData = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData); // Esta línea convierte los datos de la imagen en la textura
            SetImagePreview(texture);

            SetImageChange(true);
            fileManager.DeletePreviousCopyImage(); // borramos la imagén anterior seleccionada
            fileManager.SetCurrentImageName(fileName);
            fileManager.SaveFileInternalExtorage(texture, fileName); // salvamos una copia la imagén que selecciono
        }
#endif
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
