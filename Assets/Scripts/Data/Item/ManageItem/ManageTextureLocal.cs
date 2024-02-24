using UnityEngine;
using System.IO;
using System;

/// <summary>
/// Salvamos las texturas en la carpeta image_items
/// </summary>
public class ManageTextureLocal : IDataTextureLocalSaved
{
    private string folderNameUser;
    private const string FOLDER_IMAGE_ITEM = "image_items";

    public void SetUserUidFolder(string folderNameUser)
    {
        this.folderNameUser = folderNameUser;
    }

    public Texture2D LoadTextureAsPNG(string imageName)
    {
        if (!IsUserFolderNameUid()) return null;

        string filePath = GetFilePath(imageName + ".png");

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

        string filePath = GetFilePath(imageName + ".png");

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
        string filePath = GetFilePath(imageName + ".png");
        // Escribir los bytes en un archivo PNG
        File.WriteAllBytes(filePath, bytes); // Escribe los bytes en un archivo PNG
    }

    private string GetFilePath(string imageName)
    {
        string folderPath = Path.Combine(Application.persistentDataPath, folderNameUser);
        string folderItems = Path.Combine(folderPath, FOLDER_IMAGE_ITEM);
        string filePath = Path.Combine(folderItems, imageName);

        // Verificar si la carpeta existe, si no, crearla
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        if (!Directory.Exists(folderItems))
        {
            Directory.CreateDirectory(folderItems);
        }

        return filePath;
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
