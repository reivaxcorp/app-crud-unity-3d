using UnityEngine;
using static UnityEngine.Networking.UnityWebRequest;

public class MenuUserSesion : Menu
{
    [SerializeField] private GameObject loginBtn;
    [SerializeField] private GameObject enterBtn;
    [SerializeField] private GameObject logOutBtn;

    private bool resultPut;

    // Start is called before the first frame update
    void Start()
    {
        ShowLoginButton(false);
        resultPut = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(FirebaseSDK.GetInstance().isFirebaseReady)
        {
            if (!resultPut && FirebaseSDK.GetInstance().auth.CurrentUser != null)
            {
                resultPut = true;
                AccountAuthResult result = new AccountAuthResult("Logeado con mail: \n" + FirebaseSDK.GetInstance().auth.CurrentUser.Email, Color.green, false);
                SetResult(result);
            }
        }
    }


    public void LogOut()
    {
        FirebaseSDK.GetInstance().LogOut();
        HideButtonSessionOn();
        AccountAuthResult result = new AccountAuthResult("", Color.white, false);
        SetResult(result);
    }

    private void HideButtonSessionOn()
    {
        if(enterBtn != null && logOutBtn != null) {
            enterBtn.SetActive(false);
            logOutBtn.SetActive(false);
            ShowLoginButton(true);
        } else
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
 
}
