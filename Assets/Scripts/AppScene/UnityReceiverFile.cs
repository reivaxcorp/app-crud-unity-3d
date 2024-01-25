using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnityReceiverFile : MonoBehaviour
{
    public AndroidJavaObject currentActivity;

    [SerializeField] TMP_InputField menuImageNameInput;
    [SerializeField] Image menuImagePreview;

    
    public void LoadTextureFromFile(string filePath)
    {
        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData); // Esta línea convierte los datos de la imagen en la textura
        SetImagePreview(texture);
    }

    public void SetImagePreview(Texture2D texture)
    {
        // Crea un sprite con la textura cargada
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        // Asigna el sprite al componente Image
        if (menuImagePreview != null)
        {
            menuImagePreview.sprite = sprite;
        }
        else
        {
            Debug.LogError("Image component not assigned in the Inspector");
        }
    }
}
