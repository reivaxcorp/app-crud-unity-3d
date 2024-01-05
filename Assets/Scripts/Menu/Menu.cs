using System.Collections;
using TMPro;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public TextMeshProUGUI resultMsj;
    protected FirebaseAuthManage firebaseAuthManage;
    protected ValidateMenuInputs validateInputs;

    private void Awake()
    {
        validateInputs = gameObject.AddComponent<ValidateMenuInputs>();
    }

    private void Start()
    {
        firebaseAuthManage = new FirebaseAuthManage();
    }

    protected void SetResult(AccountAuthResult result) {

        if (resultMsj != null)
        {
            resultMsj.SetText(result.Message);
            resultMsj.color = result.MessageColor;
        }
        else
        {
            Debug.LogWarning("msj result menu is null");
        }
    }

    IEnumerator GoMenuUserSesion()
    {
        // you can show a progress bar here....

        yield return new WaitForSeconds(2);

        MenuManager menuManager = gameObject.transform.parent.GetComponent<MenuManager>();
        if (menuManager != null)
        {
            menuManager.ShowMenuByName("MenuUserSesion");
        } else
        {
            Debug.LogWarning("MenuManager doesn't exist in parent menu");
        }
    }

    // desuscribe to prevent memory leak
    private void DesuscribeEvent()
    {
        if (firebaseAuthManage != null)
        {
            // Desuscripción del evento OnAccountCreated
            firebaseAuthManage.OnAccountAuthResult -= SetResult;
        }
    }

    // call it when we use SetActive "false"
    private void OnDisable()
    {
        DesuscribeEvent();
    }

    private void OnDestroy()
    {
        DesuscribeEvent();
    }
  
}
