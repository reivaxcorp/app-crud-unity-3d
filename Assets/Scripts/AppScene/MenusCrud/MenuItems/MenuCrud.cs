/*********************************************************************************
 * Nombre del Archivo:     MenuCrud.cs
 * Descripción:            Clase padre, proporciona los  métodos comunes de las 
 *                         clases hijas MenuAddItem y MenuUpdateItem. Como por ejemplo, 
 *                         comprobar inputs, verificar datos lógicos, y preparar el menu de 
 *                         acuerdo al usuario activo.
 *                         
 * Autor:                  Javier
 * Organización:           ReivaxCorp.
 *
 * Derechos de Autor (c) [2024] ReivaxCorp
 * 
 * Permiso es otorgado, sin cargo, para que cualquier persona obtenga una copia
 * de este software y de los archivos de documentación asociados (el "Software"),
 * para tratar en el Software sin restricción, incluyendo sin limitación los
 * derechos para usar, copiar, modificar, fusionar, publicar, distribuir,
 * sublicenciar, y/o vender copias del Software, y para permitir a las personas a
 * quienes pertenezca el Software, sujeto a las siguientes condiciones:
 *
 * El aviso de derechos de autor anterior y este aviso de permiso se incluirán en
 * todas las copias o partes sustanciales del Software.
 *
 * EL SOFTWARE SE PROPORCIONA "TAL CUAL", SIN GARANTÍA DE NINGÚN TIPO, EXPRESA O
 * IMPLÍCITA, INCLUYENDO PERO NO LIMITADO A LAS GARANTÍAS DE COMERCIABILIDAD,
 * IDONEIDAD PARA UN PROPÓSITO PARTICULAR Y NO INFRACCIÓN. EN NINGÚN CASO LOS
 * AUTORES O TITULARES DE DERECHOS DE AUTOR SERÁN RESPONSABLES DE CUALQUIER
 * RECLAMACIÓN, DAÑO O OTRA RESPONSABILIDAD, YA SEA EN UNA ACCIÓN DE CONTRATO, AGRAVIO
 * O DE OTRO MODO, DERIVADAS DE, FUERA DE O EN CONEXIÓN CON EL SOFTWARE O EL USO U OTROS
 * TRATOS EN EL SOFTWARE.
 *********************************************************************************/

using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MenuCrud : MonoBehaviour
{
    
    [SerializeField] GameObject ads;
    [SerializeField] MenuDialogConfirm dialogMsj;
    [SerializeField] AndroidPermission androidPermission;
    [SerializeField] ReceiverMessagesFromAndroid receiverMessagesFromAndroid;
    [SerializeField] ManageItems manageItems;
    [SerializeField] protected MenuManagerApp uiApp;
    [SerializeField] protected Image menuImagePreview;
    [SerializeField] protected TextMeshProUGUI resultMsj;
    [SerializeField] protected TMP_InputField inputFieldName;
    protected ProgressText progressText;
    protected bool isImageChanged;
    protected bool isDelteItem;

    public FileManager fileManager
    {
        private set { _fileManager = value; }
        get { return _fileManager; }
    }

    private bool waitForFirebaseSdk;
    private FileManager _fileManager;


    public void OpenDialog(string title, string body)
    {
        dialogMsj.ShowDialog(title, body);
    }
 
    public void SetImagePreview(Texture2D texture)
    {
        // Crea un sprite con la textura cargada
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        // Asigna el sprite al componente Image
        menuImagePreview.sprite = sprite;
    }

    /// <summary>
    /// Cuando el usuario elige una imagén 
    /// </summary>
    /// <param name="isImageChanged"></param>
    public void SetImageChange(bool isImageChanged)
    {
        this.isImageChanged = isImageChanged;
    }

    public void SetImageNameInInput(string imageName)
    {
        inputFieldName.text = imageName;
    }

    public void StartAnimationTextMenu(bool isAnimationStart, string msj)
    {
        if(isAnimationStart)
        {
            progressText?.StartProgressTextAnimation(msj, resultMsj);
        }
        else
        {
            progressText?.StopProgressTextAnimation();
        }
    }

    /// <summary>
    /// Verificamos que los datos hayan sido llenado
    /// </summary>
    /// <returns></returns>
    public bool IsDataSetted()
    {
        ClearResultCrud();

        if (inputFieldName == null)
        {
            LogWarningAndSetResult("InputFieldName no asignado en el Inspector");
            return false;
        }

        if (menuImagePreview == null)
        {
            LogWarningAndSetResult("MenuImagePreview no asignado en el Inspector");
            return false;
        }

        // Sanitizar el nombre de la imagen utilizando la expresión regular
        string sanitizedFileName = StringSanitizer.SanitizeString(inputFieldName.text);

        if (string.IsNullOrEmpty(sanitizedFileName))
        {
            LogWarningAndSetResult("Ingrese el nombre de la imagén");
            SetMsjInfoUI("Ingrese el nombre de la imagén");
            return false;
        }

        if (sanitizedFileName.Length > 30)
        {
            LogWarningAndSetResult("Nombre debe ser menor a 30 caracteres");
            SetMsjInfoUI("Nombre debe ser menor a 30 caracteres");
            return false;
        }

        if (menuImagePreview.sprite == null)
        {
            LogWarningAndSetResult("Seleccione una imagén");
            SetMsjInfoUI("Seleccione una imagén");
            return false;
        }

        return true;
    }
    protected void SetMsjInfoUI(string msj)
    {
        if (resultMsj != null)
        {
            resultMsj.text = msj;
            resultMsj.color = Color.cyan;
        }
        else
        {
            Debug.LogWarning("Por favor, coloca resultMsj en el Inspector");
        }
    }

    private void LogWarningAndSetResult(string mesageWaring)
    {
        Debug.LogWarning(mesageWaring);
    }

    public void CloseMenu()
    {
        if(isImageChanged)
        {
            fileManager.DeletePreviousCopyImage();
        }
        uiApp.HideMenu();
    }

    /// <summary>
    /// Cuando el usuario hace click en aceptar en el botón del dialogo,
    /// cuando completamos una accion
    /// </summary>
    public void ConfirmDialogInfo()
    {
        uiApp.HideMenu();
    }

    /// <summary>
    /// Reset menu cuando desactivamos
    /// </summary>
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

    public void ShowInterstitialAd()
    {
        if (ads != null)
        {
            InterstitialAd interstitialAd = ads.GetComponent<InterstitialAd>();
            if (interstitialAd != null)
            {
                // interstitialAd.LoadAd();
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
                SetMsjInfoUI("¡Permiso concedido!");
                fileManager.CreateIntentFileAndroid();
                break;
            case PermissionStatus.Denied:
                Debug.LogWarning("Permiso denegado por el usuario.");
                SetMsjInfoUI("Permiso denegado por el usuario.");
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

            byte[] fileData = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);
            // Esta línea convierte los datos de la imagen en la textura
            texture.LoadImage(fileData); 
            SetImagePreview(texture);
            SetImageChange(true);
            // borramos la imagén anterior seleccionada
            fileManager.DeletePreviousCopyImage();
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
