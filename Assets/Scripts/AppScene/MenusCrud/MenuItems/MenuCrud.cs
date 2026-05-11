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
using System.Runtime.InteropServices;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MenuCrud : MonoBehaviour
{
    
    [SerializeField] MenuDialogConfirm dialogMsj;
    [SerializeField] ReceiverMessagesFromAndroid receiverMessagesFromAndroid;
    [SerializeField] ManageItems manageItems;
    [SerializeField] protected MenuManagerApp uiApp;
    [SerializeField] protected Image menuImagePreview;
    [SerializeField] protected TextMeshProUGUI resultMsj;
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

        if (menuImagePreview == null)
        {
            LogWarningAndSetResult("MenuImagePreview no asignado en el Inspector");
            return false;
        }

        if (menuImagePreview.sprite == null)
        {
            LogWarningAndSetResult("Select an image");
            SetMsjInfoUI("Select an image");
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
        // 1. Llamamos directamente a la función sin asignar a ninguna variable
        NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                // Tu lógica de procesamiento
                ProcessSelectedImage(path);
            }
        }, "Selecciona una imagen", "image/*");

        // Borra la línea del Debug.Log que usaba la variable 'permission' 
        // porque ya no existe.
    }

     
    private void OnEnable()
    {
        ResetMenu();
        SetCurrentMenu(this);
    }

    private void OnDisable()
    {
        ResetMenu();
    }

    private void SetCurrentMenu(MenuCrud menu)
    {
        receiverMessagesFromAndroid.SetCurrentMenu(menu);
        uiApp.SetCurrentMenu(menu);
    }
 
    private void ClearInputs()
    {
        menuImagePreview.sprite = null;
    }

    private void Awake()
    {
        progressText = gameObject.AddComponent<ProgressText>();
        CheckReferences();
    }

    private void Start()
    {
        waitForFirebaseSdk = true;
    }

    private void Update()
    {
        if (waitForFirebaseSdk)
        {
            if (MyApplication.IsFirebaseReady &&
                FirebaseSDK.GetInstance().auth.CurrentUser != null)
            {
                string uid = FirebaseSDK.GetInstance().auth.CurrentUser.UserId;
                fileManager = new FileManager(uid);

                fileManager.SetFolderUidName(uid);
                waitForFirebaseSdk = false;
                Debug.Log("<color=green>FileManager inicializado con éxito para el usuario: </color>" + uid);
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
        // Agregamos la condición para Windows compilado
        else if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            WindowsFileBrowser wfb = GetComponent<WindowsFileBrowser>();
            if (wfb != null)
            {
                // Modifica tu WindowsFileBrowser para que devuelva la ruta o 
                // llama directamente a ProcessSelectedImage desde allí.
                wfb.OpenExplorer((path) => ProcessSelectedImage(path));
            }
        }
        else
        {
            Debug.LogWarning("Plataforma no soportada");
        }
    }

    private void ProcessSelectedImage(string path)
    {
        if (string.IsNullOrEmpty(path)) return;

        // 1. Obtener el nombre del archivo
        string fileName = Path.GetFileNameWithoutExtension(path);

        // 2. Leer los bytes (Funciona igual en Editor y Windows Player)
        byte[] fileData = File.ReadAllBytes(path);

        // 3. Crear la textura y cargarla
        Texture2D texture = new Texture2D(2, 2);
        if (texture.LoadImage(fileData))
        {
            // 4. Aplicar toda tu lógica de UI y Archivos
            SetImagePreview(texture);
            SetImageChange(true);

            // Gestión de archivos local (Firebase/Local Storage)
            fileManager.DeletePreviousCopyImage();
            fileManager.SetCurrentImageName(fileName);
            fileManager.SaveFileInternalExtorage(texture, fileName);

            Debug.Log("Imagen cargada exitosamente desde: " + path);
        }
    }

    private void OpenFileEditor()
    {
#if UNITY_EDITOR 
        string path = EditorUtility.OpenFilePanel("Select Image", "", "png,jpg,jpeg,gif,bmp");
        if (!string.IsNullOrEmpty(path))
        {
            ProcessSelectedImage(path);
        }
#endif
    }

    private void CheckReferences()
    {
        if (uiApp == null) Debug.LogWarning("Coloca el script desde el UiApp (gameObject) el script MenuManagerApp en el Inspector");
        if (dialogMsj == null) Debug.LogWarning("Coloca el script DialogMsj desde el DialogMsj gameObject en MenuApp -> Canvas -> DialogMsj en el inspector");
        if (menuImagePreview == null) Debug.LogWarning("MenuImagePreview no asignado en el Inspector");
        if (receiverMessagesFromAndroid == null) Debug.LogWarning("Por favor coloca el script ReceiverMeesagesFromAndroid desde el Manager (gameObject) en el inspector");
        if (resultMsj == null) Debug.LogWarning("ResultMsj no está colocado en el inspector");
    }
}
