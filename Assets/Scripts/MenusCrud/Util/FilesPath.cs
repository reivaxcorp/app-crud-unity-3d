using System.IO;
using UnityEngine;

public class FilesPath  
{
    private const string FOLDER_IMAGE_ITEM = "image_items";

    public static string GetFolderItemPath(string imageName, string folderNameUser)
    {
        string folderPath = Path.Combine(Application.persistentDataPath, folderNameUser);
        string folderItems = Path.Combine(folderPath, FOLDER_IMAGE_ITEM);
        string filePath = Path.Combine(folderItems, imageName + ".png");

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
}
