using UnityEngine;
using System.IO;
public class TextureLocalData : IDataTextureLocalSaved
{
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

    public void RemoveTexture(string imageName)
    {
        string path = Path.Combine(Application.persistentDataPath, imageName + ".png");

        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Imagen eliminada: " + path);
        }
        else
        {
            Debug.LogError("No se encontró la imagen para eliminar en: " + path);
        }
    }

    public void SaveTextureAsPNG(Texture2D textureToSave, string imageName)
    {
        byte[] bytes = textureToSave.EncodeToPNG(); // Convierte la textura en formato PNG

        string path = Application.persistentDataPath + "/" + imageName + ".png"; // Ruta de destino del archivo PNG
        File.WriteAllBytes(path, bytes); // Escribe los bytes en un archivo PNG
    }
}
