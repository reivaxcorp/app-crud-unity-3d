using UnityEngine;

public class MenuUserSesion : Menu
{
    [SerializeField] private GameObject loginBtn;
    [SerializeField] private GameObject enterBtn;
    [SerializeField] private GameObject logOutBtn;

    private bool resultPut;

    // Start is called before the first frame update
    void Start()
    {
        if(loginBtn != null)
        {
            loginBtn.SetActive(false);
        } else
        {
            Debug.LogWarning("Please put Loginbtn on inspector");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(FirebaseSDK.GetInstance().isFirebaseReady)
        {
            if (!resultPut && FirebaseSDK.GetInstance().auth.CurrentUser != null)
            {
                SetMenuResult("Logeado con mail: \n" + FirebaseSDK.GetInstance().auth.CurrentUser.Email);
                resultPut = true;
            }
        }
    }

    public override void SetMenuResult(string name)
    {
        if (resultMsj != null)
        {
            resultMsj.text = name;
        }
        else
        {
            Debug.LogWarning("msj result menu is null");
        }
    }
 

    public void LogOut()
    {
        FirebaseSDK.GetInstance().LogOut();
        HideButtonSessionOn();
        SetMenuResult("Iniciar sesión");
    }

    private void HideButtonSessionOn()
    {
        if(enterBtn != null && logOutBtn != null) {
            enterBtn.SetActive(false);
            logOutBtn.SetActive(false);
            loginBtn.SetActive(true);
        } else
        {
            Debug.LogWarning("Please put btn on inspector EnterBtn and LogOutbtn");
        }

    }
}
