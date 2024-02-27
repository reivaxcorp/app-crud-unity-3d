using System.Collections;
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
        MenuManager menuManager = gameObject.transform.parent.GetComponent<MenuManager>();
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
