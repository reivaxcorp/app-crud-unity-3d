/*********************************************************************************
 * Nombre del Archivo:     MenuManagerAccount.cs
 * Descripción:            Esta clase es la responsable de administrar los menus de inicio de sesión, 
 *                         creación de cuentas, y de el panel de usuario, mostrará ó ocultara los menús, 
 *                         según las interacción del usuario. 
 * Autor:                  Javier
 * Organización:           ReivaxCorp.
 *
 * Derechos de Autor (c) [2024] ReivaxCorp
 * 
 * Permiso es otorgado, sin cargo, para que cualquier persona obtenga una copia
 * de este software y de los archivos de documentación asociados (el "Software"),
 * para tratar en el Software sin restricción, incluyendo sin limitación los
 * derechos para usar, copiar, modificar, fusionar, publicar, distribuir,
 * sublicenciar, y/o vender copias del Software, y para permitir a las personas a
 * quienes pertenezca el Software, sujeto a las siguientes condiciones:
 *
 * El aviso de derechos de autor anterior y este aviso de permiso se incluirán en
 * todas las copias o partes sustanciales del Software.
 *
 * EL SOFTWARE SE PROPORCIONA "TAL CUAL", SIN GARANTÍA DE NINGÚN TIPO, EXPRESA O
 * IMPLÍCITA, INCLUYENDO PERO NO LIMITADO A LAS GARANTÍAS DE COMERCIABILIDAD,
 * IDONEIDAD PARA UN PROPÓSITO PARTICULAR Y NO INFRACCIÓN. EN NINGÚN CASO LOS
 * AUTORES O TITULARES DE DERECHOS DE AUTOR SERÁN RESPONSABLES DE CUALQUIER
 * RECLAMACIÓN, DAÑO O OTRA RESPONSABILIDAD, YA SEA EN UNA ACCIÓN DE CONTRATO, AGRAVIO
 * O DE OTRO MODO, DERIVADAS DE, FUERA DE O EN CONEXIÓN CON EL SOFTWARE O EL USO U OTROS
 * TRATOS EN EL SOFTWARE.
 *********************************************************************************/

using UnityEngine;


/// <summary>
/// Mostramos los menus según corresponnda.
/// </summary>
public class MenuManagerAccount : MonoBehaviour
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
