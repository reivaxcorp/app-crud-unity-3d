using System.Collections;
using TMPro;
using UnityEngine;

public class MenuAuth : MonoBehaviour
{
    public TextMeshProUGUI resultMsj;
    protected FirebaseAuthManager firebaseAuthManager;
    protected ValidateMenuInputs validateInputs;

    private void Awake()
    {
        validateInputs = gameObject.AddComponent<ValidateMenuInputs>();
    }

    private void Start()
    {
        firebaseAuthManager = new FirebaseAuthManager();
    }

    /// <summary>
    /// We show the result for interactions with Sdk. 
    /// </summary>
    /// <param name="result"></param>
    public virtual void SetResult(AccountAuthResult result) {

        if (resultMsj != null)
        {
            resultMsj.SetText(result.Message);
            resultMsj.color = result.MessageColor;
            if(result.IsSuccessed)
            {
                StartCoroutine(GoMenuUserAccount());
            }
        }
        else
        {
            Debug.LogWarning("msj result menu is null");
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

    IEnumerator GoMenuUserAccount()
    {
        // you can show a progress bar here....

        yield return new WaitForSeconds(2);

        MenuManager menuManager = gameObject.transform.parent.GetComponent<MenuManager>();
        if (menuManager != null)
        {
            menuManager.ShowMenuByName("MenuUserAccount");
        } else
        {
            Debug.LogWarning("MenuManager doesn't exist in parent menu");
        }
    }

    // call it when we use SetActive "false"
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
