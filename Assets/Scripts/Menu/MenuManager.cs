using UnityEngine;

public class MenuManager : MonoBehaviour
{
    private const string MENU_LOGIN_NAME = "MenuLogin";
    private const string MENU_CREATE_ACCOUNT = "MenuCreateAccount";

    [SerializeField] private Menu [] menus;

    private void Start()
    {
        ShowMenuLogin();
    }

    public void ShowMenuLogin()
    {
        for(int menuIndex = 0; menuIndex < menus.Length; menuIndex++)
        {
            if (menus[menuIndex].name.Equals(MENU_LOGIN_NAME))
            {
                menus[menuIndex].gameObject.SetActive(true);
            } else
            {
                menus[menuIndex].gameObject.SetActive(false);
            }
        }
    }

    public void ShowMenuCreateAccount()
    {
        for (int menuIndex = 0; menuIndex < menus.Length; menuIndex++)
        {
            if (menus[menuIndex].name.Equals(MENU_CREATE_ACCOUNT))
            {
                menus[menuIndex].gameObject.SetActive(true);
            }
            else
            {
                menus[menuIndex].gameObject.SetActive(false);
            }
        }
    }
 

}
