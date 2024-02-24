using UnityEditor;
using UnityEngine;


/// <summary>
/// Mostramos los menus según corresponnda.
/// </summary>
public class MenuManager : MonoBehaviour
{
    [SerializeField] private MenuAuth[] menus;
    private const string MENU_LOGIN_NAME = "MenuLogin";
    private const string MENU_USER_ACCOUNT = "MenuUserAccount";
    
    private bool menuSet = false;

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

        if(!menuIsShowed) { Debug.LogWarning("El menu no existe, compruebe el nombre del menu"); }
    }

    private void Start()
    {
        menuSet = false;
    }

    private void Update()
    {
        if (!menuSet && FirebaseSDK.GetInstance().isFirebaseReady)
        {
            if (FirebaseSDK.GetInstance().auth.CurrentUser
                != null)
            {
                ShowMenuByName(MENU_USER_ACCOUNT);
            }
            else
            {
                ShowMenuByName(MENU_LOGIN_NAME);
            }
            menuSet = true;
        }
    }
}
