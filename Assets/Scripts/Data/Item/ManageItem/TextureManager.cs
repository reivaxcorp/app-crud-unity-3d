using UnityEngine;
using System.IO;
using System;

public class TextureManager : IDataTextureLocalSaved
{
    private string folderUidUser;

    public TextureManager()
    {
        if(FirebaseSDK.GetInstance().auth != null)
        {
            this.folderUidUser = FirebaseSDK.GetInstance().user.UserId;
        } else
        {
            Debug.LogWarning("Firebase auth no esta inicializado");
        }
    }

    public Texture2D LoadTextureAsPNG(string imageName)
    {
        string folderPath = Path.Combine(Application.persistentDataPath, folderUidUser);
        string filePath = Path.Combine(folderPath, imageName + ".png");

        // Verificar si la carpeta existe, si no, crearla
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        if (File.Exists(filePath))
        {
            byte[] bytes = File.ReadAllBytes(filePath);
            Texture2D loadedTexture = new Texture2D(2, 2); // Crea una textura vacía
            loadedTexture.LoadImage(bytes); // Carga los bytes como textura PNG
            return loadedTexture;
        }
        else
        {
            return null;
        }
    }

    public void RemoveTexture(string imageName)
    {
        string folderPath = Path.Combine(Application.persistentDataPath, folderUidUser);
        string filePath = Path.Combine(folderPath, imageName + ".png");

        // Verificar si la carpeta existe, si no, crearla
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("Imagen eliminada: " + filePath);
        }
        else
        {
            Debug.LogError("No se encontró la imagen para eliminar en: " + filePath);
        }
    }

    public void SaveTextureAsPNG(Texture2D textureToSave, string imageName)
    {
        byte[] bytes = textureToSave.EncodeToPNG(); // Convierte la textura en formato PNG
        string folderPath = Path.Combine(Application.persistentDataPath, folderUidUser);
        string filePath = Path.Combine(folderPath, imageName + ".png");

        // Verificar si la carpeta existe, si no, crearla
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Escribir los bytes en un archivo PNG
        File.WriteAllBytes(filePath, bytes); // Escribe los bytes en un archivo PNG
    }
}
