using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;

public class LocalDb : IRepositoryLocal
{
    private const string SAVE_FILE_NAME = "items.crud";
    private string folderUidUser;

    public LocalDb()
    {
        if (FirebaseSDK.GetInstance().auth != null)
        {
            this.folderUidUser = FirebaseSDK.GetInstance().user.UserId;
        }
        else
        {
            Debug.LogWarning("Firebase auth no esta inicializado");
        }
    }

    public void DeleteLocalItemById(string id)
    {
        List<ItemLocal> localItemsList = GetLocalItems();

        // Buscar si el item ya existe en la lista
        int existingIndex = localItemsList.FindIndex(x => x.Id == id);

        if (existingIndex != -1)
        {
            // Si el item existe, eliminarlo de la lista
            localItemsList.RemoveAt(existingIndex);
        }
        SaveLocalItems(localItemsList);
    }

    public ItemLocal GetItemById(string id)
    {
        throw new System.NotImplementedException();
    }

    public ItemLocal GetLocalItemById(string id)
    {
        throw new System.NotImplementedException();
    }

    public List<ItemLocal> GetLocalItems()
    {

        string folderPath = Path.Combine(Application.persistentDataPath, folderUidUser);
        string filePath = Path.Combine(folderPath, SAVE_FILE_NAME);

        // Verificar si la carpeta existe, si no, crearla
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        if (File.Exists(filePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(filePath, FileMode.Open);

            List<ItemLocal> itemsLocal = formatter.Deserialize(stream) as List<ItemLocal>;
            stream.Close();
            return itemsLocal;
        }
        else
        {
            Debug.Log("Archivos locales no encontrados");
            return new List<ItemLocal>();
        }
    }

    public void SaveLocalItem(ItemLocal itemLocal)
    {
        List<ItemLocal> currentLocalList = GetLocalItems();
        currentLocalList.Add(itemLocal);
        SaveLocalItems(currentLocalList);
    }

    public void SaveLocalItems(List<ItemLocal> listItemsLocal)
    {

        string folderPath = Path.Combine(Application.persistentDataPath, folderUidUser);
        string filePath = Path.Combine(folderPath, SAVE_FILE_NAME);

        // Verificar si la carpeta existe, si no, crearla
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(filePath, FileMode.Create);

        formatter.Serialize(stream, listItemsLocal);
        stream.Close();
    }

    public void UpdateLocalItemById(string id)
    {
        throw new System.NotImplementedException();
    }

}
