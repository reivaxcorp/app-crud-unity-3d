using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class ReceiverMessagesFromAndroid : MonoBehaviour
{
    [SerializeField] MenuAddItem menuAddItem;
    private AndroidJavaObject currentActivity;
    private object inputStreamResult;

    private void Start()
    {
        SetCurrentActivity();
    }

    public void ReceiveDataFromAndroid(string selectedFileUri)
    {
        if (!string.IsNullOrEmpty(selectedFileUri))
        {
            StartCoroutine(SetImageFromPathFromUriCoroutine(selectedFileUri));
        }
    }

    private IEnumerator SetImageFromPathFromUriCoroutine(string selectedFileUri)
    {
        yield return new WaitForSeconds(2.0f); // wait to return in our program.

        // Convierte la cadena de URI de Android a un objeto AndroidJavaObject en C#
        AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
        AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", selectedFileUri);

        // Puedes usar uriObject según tus necesidades en C#
        Debug.Log("Received Android Uri in C#: " + uriObject.ToString());

        // Obtener un InputStream desde la URI
        AndroidJavaObject inputStream = currentActivity.Call<AndroidJavaObject>("getContentResolver")
            .Call<AndroidJavaObject>("openInputStream", uriObject);

        // Esperar hasta que la operación de lectura se complete
        yield return StartCoroutine(ReadInputStreamAsync(inputStream));

        // Obtener los bytes del resultado de la operación de lectura
        byte[] bytes = (byte[])inputStreamResult;

        // Crear una textura en Unity a partir del array de bytes
        Texture2D texture = new Texture2D(1, 1);
        texture.LoadImage(bytes);

        if(menuAddItem != null)
        {
            menuAddItem.SetImagePreview(texture);
            menuAddItem.fileManager.DeletePreviousCopyImage();
            menuAddItem.fileManager.SaveFileInternalExtorage(uriObject, texture, currentActivity);
        } else
        {
            Debug.LogWarning("Please put MenuAddItem on inspector");
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
