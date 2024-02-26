using System.Collections;
using UnityEngine;

public class ReceiverMessagesFromAndroid : MonoBehaviour
{
    private MenuCrud currentMenu;

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
                currentMenu.SetImageName(fileName);
                currentMenu.SetImageChange(true);
                currentMenu.fileManager.DeletePreviousCopyImage(); // borramos la imagén anterior seleccionada
                currentMenu.fileManager.SetCurrentImageName(fileName);
                currentMenu.fileManager.SaveFileInternalExtorage(texture, fileName); // salvamos una copia la imagén que selecciono
            }
            else
            {
                Debug.LogWarning("CurrentMenu es nulo");
            }
        }
        else
        {
            Debug.LogError("Datos invalidos para fileNameWithBase64");
        }
    }

}
