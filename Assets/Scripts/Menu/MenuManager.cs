using UnityEditor;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    private const string MENU_LOGIN_NAME = "MenuLogin";
    private const string MENU_USER_SESION = "MenuUserAccount";

    private bool menuSet = false;
    [SerializeField] private Menu [] menus;

    private void Start()
    {
        menuSet = false;
    }

    private void Update()
    {
        if(!menuSet && FirebaseSDK.GetInstance().isFirebaseReady)
        {
            if (FirebaseSDK.GetInstance().auth.CurrentUser != null)
            {
                ShowMenuByName(MENU_USER_SESION);
            } else
            {
                ShowMenuByName(MENU_LOGIN_NAME);
            }
            menuSet = true;
        }
    }

    public void ShowMenuByName(string menuName)
    {
        bool menuIsShowed = false;

        for(int menuIndex = 0; menuIndex < menus.Length; menuIndex++)
        {
            if (menus[menuIndex].name.Equals(menuName))
            {
                menus[menuIndex].gameObject.SetActive(true);
                menuIsShowed = true;
            } else
            {
                menus[menuIndex].gameObject.SetActive(false);
            }
        }

        if(!menuIsShowed) { Debug.LogWarning("Menu doesn't exist, please verify name menu in params"); }
    }

}
