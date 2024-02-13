using UnityEngine;

public class MenuManagerApp : MonoBehaviour
{
    [SerializeField] GameObject addItemBtn;
    [SerializeField] GameObject menuAddItem;
    private MenuCrud menu;

    public void ShowMenuAddItem()
    {
        menuAddItem.transform.parent.gameObject.SetActive(true);
        menuAddItem.SetActive(true);
        ButtonAddItemSetActive(false);
    }

    public void SetCurrentMenu(MenuCrud menu)
    {
        this.menu = menu;
    }

    public void ShowMenu()
    {
        ButtonAddItemSetActive(false);
        MenuSetActive(true);
    }

    public void HideMenu()
    {
        MenuSetActive(false);
        ButtonAddItemSetActive(true);
    }

    public void ButtonAddItemSetActive(bool isActive)
    {
        addItemBtn.SetActive(isActive);
    }

    private void MenuSetActive(bool isActive)
    {
        if(menu != null)
        {
            menu.gameObject.SetActive(isActive);
        }
        else
        {
            Debug.LogWarning("Esta llamando un menú que es null");
        }
    }

    private void Awake()
    {
        CheckReferences();
    }

    private void CheckReferences()
    {
        if (addItemBtn == null) { Debug.LogWarning("Por favor por la referencia AddItemBtn (child MenuApp gameObject) en el  inspector"); }
        if (menuAddItem == null) { Debug.LogWarning("Por favor por el gameobject MenuAddItem en el  inspector"); }
    }
}
