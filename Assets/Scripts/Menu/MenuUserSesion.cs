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
                SetMsjResult("Logeado con mail: \n" + FirebaseSDK.GetInstance().auth.CurrentUser.Email, Color.green);
                resultPut = true;
            }
        }
    }


    public void LogOut()
    {
        FirebaseSDK.GetInstance().LogOut();
        HideButtonSessionOn();
        SetMsjResult("Iniciar sesión", Color.white);
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
