/*********************************************************************************
 * Nombre del Archivo:     MenuAuth.cs
 * Descripción:            Clase encargada de mostrarnos, el resultado de las operaciones de creación de cuentas
 *                         ó inicio de sesión. 
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

using TMPro;
using UnityEngine;

public class MenuAuth : MonoBehaviour
{
    public TextMeshProUGUI resultMsj;
    protected FirebaseAuthManager firebaseAuthManager;
    protected ValidateMenuInputs validateInputs;
    [SerializeField] GameObject loadingScreen;

    /// <summary>
    /// Establece el texto del resultado de la autenticación.
    /// </summary>
    /// <param name="result"></param>
    public virtual void SetResult(AccountAuthResult result)
    {

        if (resultMsj != null)
        {

            resultMsj.SetText(result.Message);

            switch (result.AuthType)
            {
                case AuthType.LOGOUT:
                    resultMsj.color = Color.gray;
                    break;
                case AuthType.LOGIN_SUCCESS:
                    resultMsj.color = Color.green;
                    GoMenuUserAccount();
                    break;
                case AuthType.LOGIN_FAILURE:
                    resultMsj.color = Color.red;
                    break;
                case AuthType.LOGIN_CANCEL:
                    resultMsj.color = Color.gray;
                    break;
                case AuthType.CREATE_ACCOUNT_SUCCESS:
                    resultMsj.color = Color.green;
                    GoMenuUserAccount();
                    break;
                case AuthType.CREATE_ACCOUNT_FAILURE:
                    resultMsj.color = Color.red;
                    break;
                case AuthType.CREATE_ACCOUNT_CANCEL:
                    resultMsj.color = Color.gray;
                    break;
                default:
                    break;
            }
            ShowScreenLoading(false);
        }
        else
        {
            Debug.LogWarning("msj result menu es null");
        }
    }

    public void ShowScreenLoading(bool isShowScreen)
    {
        if(loadingScreen != null)
        {
            loadingScreen.SetActive(isShowScreen);
        }
        else
        {
            Debug.LogWarning("Coloca el loading screen (prefab) en el inspector del menu");
        }
    }

    // desuscribe to prevent memory leak
    public void DesuscribeEvent()
    {
        if (firebaseAuthManager != null)
        {
            // desuscribe event OnAccountCreated
            firebaseAuthManager.OnAccountAuthResult -= SetResult;
        }
    }

    // clar when we desactived menu
    public void ClearMsjResult()
    {
        resultMsj.SetText("");
        resultMsj.color = Color.white;
    }

    private void GoMenuUserAccount()
    {
        MenuManagerAccount menuManager = gameObject.transform.parent.GetComponent<MenuManagerAccount>();
        if (menuManager != null)
        {
            menuManager.ShowMenuByName("MenuUserAccount");
        }
        else
        {
            Debug.LogWarning("MenuManager no existe en el menú padre");
        }
    }

    private void Awake()
    {
        validateInputs = gameObject.AddComponent<ValidateMenuInputs>();
    }

    private void Start()
    {
        firebaseAuthManager = new FirebaseAuthManager();
    }

    // Se llama cuando se decativa el gameObject 
    private void OnDisable()
    {
        ClearMsjResult();
        DesuscribeEvent();
    }

    private void OnDestroy()
    {
        DesuscribeEvent();
    }

}
