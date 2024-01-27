using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ReceiverMessagesFromAndroid : MonoBehaviour
{
    [SerializeField] MenuAddItem menuAddItem;
    private AndroidJavaObject currentActivity;
    private object inputStreamResult;

    private void Start()
    {
        SetCurrentActivity();
    }

    // The name must be equals to personalize activity in
    //  com.unity3d.player.UnityPlayer.UnitySendMessage("Manager", "ReceiveDataFromAndroid", selectedFileUri);
    public void ReceiveDataFromAndroid(string fileNameWithBase64)
    {
        if (!string.IsNullOrEmpty(fileNameWithBase64))
        {
            StartCoroutine(SetImageFromPathFromUriCoroutine(fileNameWithBase64));
        }
    }

    private IEnumerator SetImageFromPathFromUriCoroutine(string fileNameWithBase64)
    {
        yield return new WaitForSeconds(1.0f); // wait to return in our program.

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

            if (menuAddItem != null)
            {
                menuAddItem.SetImagePreview(texture);
                menuAddItem.fileManager.DeletePreviousCopyImage();
                menuAddItem.fileManager.SaveFileInternalExtorage(texture, fileName);
            }
            else
            {
                Debug.LogWarning("Please put MenuAddItem on inspector");
            }
            // Ahora puedes usar 'fileName' y 'base64Data' según tus necesidades
        }
        else
        {
            Debug.LogError("Invalid format for fileNameWithBase64");
        }

     
    }


     private IEnumerator ReadInputStreamAsync(AndroidJavaObject inputStream)
     {
         // Convertir el InputStream a un array de bytes en una corrutina
         List<byte> bytes = new List<byte>();
         int nextByte = inputStream.Call<int>("read");
         while (nextByte != -1)
         {
             bytes.Add((byte)nextByte);
             nextByte = inputStream.Call<int>("read");
         }

         inputStream.Call("close");

         // Almacenar el resultado en la variable para que sea accesible fuera de esta corrutina
         inputStreamResult = bytes.ToArray();

         yield return null; // Esperar un frame antes de continuar
     } 
     
    private void SetCurrentActivity()
    {
        if (Application.isMobilePlatform)
        {
            // Obtener la actividad actual de Unity
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        }
    }
}
