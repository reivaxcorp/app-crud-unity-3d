using UnityEngine;
using System.IO;
using System;

public class TextureManager : IDataTextureLocalSaved
{
    private string folderNameUser;


    public void SetUserUidFolder(string folderNameUser)
    {
        this.folderNameUser = folderNameUser;
    }

    public Texture2D LoadTextureAsPNG(string imageName)
    {
        if (!IsUserFolderNameUid()) return null;

        string folderPath = Path.Combine(Application.persistentDataPath, folderNameUser);
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

    public void RemoveLocalTexture(string imageName)
    {
        if (!IsUserFolderNameUid()) return;

        string folderPath = Path.Combine(Application.persistentDataPath, folderNameUser);
        string filePath = Path.Combine(folderPath, imageName + ".png");

        // Verificar si la carpeta existe, si no, crearla
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("Imagen local eliminada: " + filePath);
        }
        else
        {
            Debug.LogError("No se encontró la imagen para eliminar en: " + filePath);
        }
    }

    public void SaveTextureAsPNG(Texture2D textureToSave, string imageName)
    {
        if (!IsUserFolderNameUid()) return;

        byte[] bytes = textureToSave.EncodeToPNG(); // Convierte la textura en formato PNG
        string folderPath = Path.Combine(Application.persistentDataPath, folderNameUser);
        string filePath = Path.Combine(folderPath, imageName + ".png");

        // Verificar si la carpeta existe, si no, crearla
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Escribir los bytes en un archivo PNG
        File.WriteAllBytes(filePath, bytes); // Escribe los bytes en un archivo PNG
    }

    private bool IsUserFolderNameUid()
    {
        if (folderNameUser == null)
        {
            Debug.LogWarning("No hay un userUid para nombre de la carpeta de usuario!!!");
            return false;
        }
        return true;
    }
}
