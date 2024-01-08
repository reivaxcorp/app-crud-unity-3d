using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUserAccount : Menu
{
    [SerializeField] private GameObject loginBtn;
    [SerializeField] private GameObject misItemsBtn;
    [SerializeField] private GameObject logOutBtn;

    private bool getUserStatus;

    void Update()
    {
        ReadUserStatus();
    }

    public override void SetResult(AccountAuthResult result)
    {
        if (resultMsj != null)
        {
            if(result.IsSuccessed)
            {
                resultMsj.SetText(result.Message);
                resultMsj.color = result.MessageColor;
                ShowButtonsSessionOn();
            } else
            {
                HideButtonsSessionOn();
            }
        }
    }

    public void LogOut()
    {
        firebaseAuthManage.OnAccountAuthResult += SetResult;
        firebaseAuthManage.LogOut();
    }

    public void LoadSceneMyItems()
    {
        // Reemplaza "NombreDeTuEscena" con el nombre de la escena que deseas cargar
        SceneManager.LoadScene("AppScene");
    }

    private void ReadUserStatus()
    {
        if (FirebaseSDK.GetInstance().isFirebaseReady)
        {
            if (!getUserStatus && FirebaseSDK.GetInstance().auth.CurrentUser != null)
            {
                AccountAuthResult result = new AccountAuthResult("Logeado con email: \n" + FirebaseSDK.GetInstance().auth.CurrentUser.Email, Color.green, true);
                SetResult(result);
                getUserStatus = true;
            }
        }
    }

    private void HideButtonsSessionOn()
    {
        if(misItemsBtn != null && logOutBtn != null) {
            misItemsBtn.SetActive(false);
            logOutBtn.SetActive(false);
            ShowLoginButton(true);
            ClearMsjResult();
        } else
        {
            Debug.LogWarning("Please put btn on inspector EnterBtn and LogOutbtn");
        }
    }

    private void ShowButtonsSessionOn()
    {
        if (misItemsBtn != null && logOutBtn != null)
        {
            misItemsBtn.SetActive(true);
            logOutBtn.SetActive(true);
            ShowLoginButton(false);
        }
        else
        {
            Debug.LogWarning("Please put btn on inspector EnterBtn and LogOutbtn");
        }
    }

    private void ShowLoginButton(bool isVisible)
    {
        if (loginBtn != null)
        {
            loginBtn.SetActive(isVisible);
        }
        else
        {
            Debug.LogWarning("Please put Loginbtn on inspector");
        }
    }

    // when we disable, reset variable for next time reload menu
    private void OnDisable()
    {
        firebaseAuthManage.OnAccountAuthResult -= SetResult; // desuscribe event in this class. 
        ClearMsjResult();
        getUserStatus = false;
    }

}
