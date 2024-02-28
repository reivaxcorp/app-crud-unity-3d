/*********************************************************************************
 * Nombre del Archivo:     ReceiverMessagesFromAndroid.cs 
 * Descripción:            Esta clase es responsable, de recibir los resultados del sistema de android, 
 *                         especificamente en el OnActivityResult, cuando el usuario elige una imagén,
 *                         los bytes de esa imagen son recibidos por esta clase, en su correspondiente metodo.
 *                         
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

using System.Collections;
using UnityEngine;

public class ReceiverMessagesFromAndroid : MonoBehaviour
{
    private MenuCrud currentMenu;

    /// <summary>
    /// Puede ser el MenuAddItem ó el MenuUpdate Item
    /// </summary>
    /// <param name="menu"></param>
    public void SetCurrentMenu(MenuCrud menu)
    {
        this.currentMenu = menu;    
    }

    // El nombre de la función debe ser la misma que se llama
    // desde la activity personalizada "CrudUnityPlayerActivity"
    public void ReceiveDataFromAndroid(string fileNameWithBase64)
    {
        if (!string.IsNullOrEmpty(fileNameWithBase64))
        {
            StartCoroutine(SetImageFromPathFromUriCoroutine(fileNameWithBase64));
        }
    }

    private IEnumerator SetImageFromPathFromUriCoroutine(string fileNameWithBase64)
    {
        yield return new WaitForSeconds(1.0f); // Esperar que volvamos del selector de archivos.

        // Separar el nombre del archivo y los datos en Base64
        string[] parts = fileNameWithBase64.Split('|');

        if (parts.Length == 2)
        {
            string fileName = parts[0];
            string base64Data = parts[1];

            // Convertir la cadena Base64 a bytes
            byte[] imageData = System.Convert.FromBase64String(base64Data);

            // Hacer algo con los bytes de la imagen (por ejemplo, convertirlos a una textura)
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(imageData);

            Debug.Log("Carga terminada");

            if (currentMenu != null)
            {
                currentMenu.SetImagePreview(texture);
                currentMenu.SetImageChange(true);
                currentMenu.fileManager.DeletePreviousCopyImage(); // borramos la imagén anterior seleccionada
                currentMenu.fileManager.SetCurrentImageName(fileName);
                currentMenu.fileManager.SaveFileInternalExtorage(texture, fileName); // salvamos una copia la imagén que selecciono
            }
            else
            {
                Debug.LogWarning("CurrentMenu es Null");
            }
        }
        else
        {
            Debug.LogError("Datos invalidos para fileNameWithBase64");
        }
    }

}
