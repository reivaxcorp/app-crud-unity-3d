using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManagerApp : MonoBehaviour
{

    [Header("Menu CRUD")]
    [SerializeField] GameObject menuAddItem;
    [SerializeField] GameObject menuUpdateItem;

    [Header("Botones Main UI")]
    [SerializeField] GameObject addItemBtn;
    [SerializeField] GameObject menuCompanyBtn;
    [SerializeField] GameObject backBtn;

    [Header("APP info")]
    [SerializeField] GameObject menuCompany;
    [SerializeField] GameObject myItemsOrdened;
    [SerializeField] GameObject tutorialInfo;

    private MenuCrud menu;

    private bool firstTouch = false;
    private float timetouch = 0.0f;
    private float timepassed = 0.0f;
    private float timeToTouch = 0.5f;
 

    public void ShowMenuAddItem()
    {
        if (!menuAddItem.activeSelf && !menuUpdateItem.activeSelf)
        {
            menuAddItem.transform.parent.gameObject.SetActive(true);
            menuAddItem.SetActive(true);
            HideUiButtons(true);
        }
    }

    public void ShowMenuUpdateItem(string idItem)
    {
        if (!menuUpdateItem.activeSelf && !menuAddItem.activeSelf)
        {
            menuUpdateItem.transform.parent.gameObject.SetActive(true);
            menuUpdateItem.SetActive(true);
            HideUiButtons(true);
            menuUpdateItem.GetComponent<MenuUpdateItem>().InitMenu(idItem);
        }
    }

    public void ShowMenuCompany()
    {
        if(!menuCompany.activeSelf)
        {
            menuCompany.SetActive(true);
            HideUiButtons(true);
        }
    }

    public void HideMenuCompany()
    {
        if (menuCompany.activeSelf)
        {
            menuCompany.SetActive(false);
            HideUiButtons(false);
        }
    }


    public void SetCurrentMenu(MenuCrud menu)
    {
        this.menu = menu;
    }

    public void ShowMenu()
    {
        HideUiButtons(true);
        MenuSetActive(true);
    }

    public void HideMenu()
    {
        MenuSetActive(false);
        HideUiButtons(false);
    }

    public void HideUiButtons(bool isActive)
    {
        addItemBtn.SetActive(!isActive);
        backBtn.SetActive(!isActive);
        menuCompanyBtn.SetActive(!isActive);
        tutorialInfo.SetActive(!isActive);
    }

    public void MenuSetActive(bool isActive)
    {
        if (menu != null)
        {
            menu.gameObject.SetActive(isActive);
        }
        else
        {
            Debug.LogWarning("Esta llamando un menú que es null");
        }
    }

    public void GoBack()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex - 1);
    }

    private void Update()
    {
        timepassed = Time.time - timetouch;

        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetMouseButtonDown(0))
        {

            if (IsDobleTouch())
            {
                if (Camera.main != null)
                {
                    Ray ray;

                    if (Input.touchCount > 0)
                    {
                        ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                    }
                    else if (Input.GetMouseButtonDown(0))
                    {
                        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    }
                    else
                    {
                        // No se realizó ningún toque ni clic, no se realiza ninguna acción
                        return;
                    }

                    RaycastHit hit;
                    // Debug del rayo lanzado por el Raycast
                    Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.green); // Aquí puedes ajustar la longitud del rayo multiplicando la dirección por un valor específico

                    if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                    {

                        if (myItemsOrdened != null)
                        {
                            for (int indexChild = 0; indexChild < myItemsOrdened.transform.childCount; indexChild++)
                            {
                                if (myItemsOrdened.transform.GetChild(indexChild).name.Equals(hit.collider.name))
                                {
                                    //   Debug.Log("Item box clicked.. " + hit.collider);
                                    ShowMenuUpdateItem(hit.collider.name);
                                }
                            }
                        }
                        else
                        {
                            Debug.LogWarning("MyItemsOrdener no esta colocado en el inspector");
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("Colocar la camara principal, con la etiqueta MainCamera");
                }
            }
        }
    }

    private bool IsDobleTouch()
    {
        if (!firstTouch)
        {

            firstTouch = true;
            timetouch = Time.time;
            timepassed = 0.0f;
            return false;
        }

        if (timepassed > timeToTouch)
        {
            timetouch = Time.time;
            timepassed = 0.0f;
            return false;
        }

        return true;
    }

    private void Awake()
    {
        CheckReferences();
    }

    private void CheckReferences()
    {
        if (addItemBtn == null) { Debug.LogWarning("Por favor, por la referencia AddItemBtn (child MenuApp gameObject) en el  inspector"); }
        if (menuAddItem == null) { Debug.LogWarning("Por favor, por el gameobject MenuAddItem en el inspector"); }
        if(menuCompany == null) { Debug.LogWarning("Por favor, por el gameobject MenuCompany en el inspector"); }
        if (backBtn == null) { Debug.LogWarning("Por favor, por el gameobject BackBtn en el inspector"); }
        if(menuCompanyBtn == null) { Debug.LogWarning("Por favor, por el gameobject MenuCompanyBtn en el inspector"); }
        if(tutorialInfo == null) { Debug.LogWarning("Por favor, por el gameObject TutorialInfo en el inspector"); }
    }
}
