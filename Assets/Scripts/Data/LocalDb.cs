using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;

public class LocalDb : IRepositoryLocal
{
    private const string SAVE_FILE_NAME = "items.crud";

    public void DeleteItemById(ItemLocal itemLocal)
    {
        throw new System.NotImplementedException();
    }

    public ItemLocal GetItemById(string id)
    {
        throw new System.NotImplementedException();
    }

    public List<ItemLocal> GetItems()
    {
        string path = Application.persistentDataPath + "/" + SAVE_FILE_NAME;

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            List<ItemLocal> itemsLocal = formatter.Deserialize(stream) as List<ItemLocal>;
            stream.Close();
            return itemsLocal;
        }
        else
        {
            //  Debug.LogError("Save file not found in " + path);
            return null;
        }
    }

    public void SaveItemsLocal(List<ItemLocal> listItemsLocal)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + SAVE_FILE_NAME;
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, listItemsLocal);
        stream.Close();
    }

    public void UpdateItemById(ItemLocal itemLocal)
    {
        throw new System.NotImplementedException();
    }

}
