using System.Collections;
using UnityEngine;
using UnityEngine.Networking; // Necesario para UnityWebRequest

public class ReceiverMessagesFromAndroid : MonoBehaviour
{
    private MenuCrud currentMenu;

    public void SetCurrentMenu(MenuCrud menu)
    {
        this.currentMenu = menu;
    }

    // Ahora recibe una URI (ej: content://media/external/images/media/123)
    public void ReceiveDataFromAndroid(string filePath)
    {
        if (!string.IsNullOrEmpty(filePath))
        {
            Debug.Log("SISTEMA: Recibida ruta de archivo: " + filePath);

            // Cargamos los bytes directamente desde el disco
            byte[] fileData = System.IO.File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData); // Esto funciona perfecto con archivos reales

            if (currentMenu != null)
            {
                currentMenu.SetImagePreview(texture);
                currentMenu.SetImageChange(true);

                // Usamos un nombre único basado en el tiempo
                string fileName = "img_" + System.DateTime.Now.Ticks;

                currentMenu.fileManager.DeletePreviousCopyImage();
                currentMenu.fileManager.SetCurrentImageName(fileName);
                currentMenu.fileManager.SaveFileInternalExtorage(texture, fileName);
            }

            // Opcional: Borrar el temporal del cache para no ocupar espacio
            System.IO.File.Delete(filePath);
        }
    }

    private IEnumerator LoadImageFromUriCoroutine(string fileUri)
    {
        // Usamos UnityWebRequest porque es la forma más segura de leer 
        // una "content:// URI" de Android sin problemas de permisos de archivos.
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(fileUri))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("SISTEMA: Error al cargar imagen desde URI: " + uwr.error);
            }
            else
            {
                // Obtenemos la textura directamente del pedido
                Texture2D texture = DownloadHandlerTexture.GetContent(uwr);

                // Inventamos un nombre basado en el tiempo si no viene uno, 
                // o podés seguir pasándolo por el string con un separador
                string fileName = "upload_" + System.DateTime.Now.Ticks.ToString();

                Debug.Log("Carga desde URI terminada con éxito");

                if (currentMenu != null)
                {
                    currentMenu.SetImagePreview(texture);
                    currentMenu.SetImageChange(true);
                    currentMenu.fileManager.DeletePreviousCopyImage();
                    currentMenu.fileManager.SetCurrentImageName(fileName);
                    currentMenu.fileManager.SaveFileInternalExtorage(texture, fileName);
                }
            }
        }
    }
}