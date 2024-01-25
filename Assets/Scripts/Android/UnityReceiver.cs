using System.Collections;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.UI;
using System;
using UnityEngine.Android;
using System.Collections.Generic;

public class UnityReceiver : MonoBehaviour
{
    [SerializeField] TMP_InputField inputName;
    [SerializeField] Image image;
    private object inputStreamResult;
    private AndroidJavaObject currentActivity;

    private void Start()
    {
        // Obtener la actividad actual de Unity
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    }

    public void ReceiveData(string selectedFileUri)
    {
        if (!string.IsNullOrEmpty(selectedFileUri))
        {
            StartCoroutine(SetImageFromPathFromUriCoroutine(selectedFileUri));
        }
    }

    private IEnumerator SetImageFromPathFromUriCoroutine(string selectedFileUri)
    {
        yield return new WaitForSeconds(3.0f);

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

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        image.sprite = sprite;

        yield return null;


        byte[] bytesImage = texture.EncodeToPNG(); // Convierte la textura en formato PNG

        // Obtener información sobre la URI para obtener el nombre del archivo
        string[] projection = { "_display_name" };
        AndroidJavaObject cursor = currentActivity.Call<AndroidJavaObject>("getContentResolver")
            .Call<AndroidJavaObject>("query", uriObject, projection, null, null, null);

        if (cursor != null)
        {
            cursor.Call<bool>("moveToFirst");
            int columnIndex = cursor.Call<int>("getColumnIndex", "_display_name");
            string fileNameWithExtension = cursor.Call<string>("getString", columnIndex);
            cursor.Call("close");

            // Separar el nombre del archivo y la extensión
            string fileName = Path.GetFileNameWithoutExtension(fileNameWithExtension);

            string path = Application.persistentDataPath + "/" + fileName + ".png"; // Ruta de destino del archivo PNG
            File.WriteAllBytes(path, bytesImage); // Escribe los bytes en un archivo PNG

            // Puedes mostrar un mensaje de éxito o realizar otras acciones después de guardar la imagen
            Debug.Log("Imagen guardada con éxito en el almacenamiento interno de la aplicación");
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

    IEnumerator LoadImage(string filePath)
    {
        // Lee los bytes de la imagen desde la URI local
        byte[] imageBytes = File.ReadAllBytes(filePath);

        // Crea una textura y carga los bytes de la imagen
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageBytes);

        // Crea un sprite con la textura
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

        // Asigna el sprite al componente de imagen
        if(image != null)
        {
            image.sprite = sprite;
            inputName.text = image.name;
        } else
        {
            Debug.LogWarning("Please put image ref on inspector");
        }

        yield return null;
    }


    public Texture2D LoadTextureAsPNG(string imageName)
    {
        string path = Path.Combine(Application.persistentDataPath, imageName + ".png");

        if (File.Exists(path))
        {
            byte[] bytes = File.ReadAllBytes(path);
            Texture2D loadedTexture = new Texture2D(2, 2); // Crea una textura vacía
            loadedTexture.LoadImage(bytes); // Carga los bytes como textura PNG
            return loadedTexture;
        }
        else
        {
            //Debug.LogError("No se encontró la imagen en: " + path);
            return null;
        }
    }
}
