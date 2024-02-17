using System.Collections;
using TMPro;
using UnityEngine;

public class MenuAuth : MonoBehaviour
{
    public TextMeshProUGUI resultMsj;
    protected FirebaseAuthManager firebaseAuthManager;
    protected ValidateMenuInputs validateInputs;

    /// <summary>
    /// Establece el texto del resultado de la autenticación.
    /// </summary>
    /// <param name="result"></param>
    public virtual void SetResult(AccountAuthResult result) {

        if (resultMsj != null)
        {
            resultMsj.SetText(result.Message);
            resultMsj.color = result.MessageColor;
            if(result.IsSuccessed)
            {
                GoMenuUserAccount();
            }
        }
        else
        {
            Debug.LogWarning("msj result menu es null");
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
        } else
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
