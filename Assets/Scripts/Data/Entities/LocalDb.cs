using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;

public class LocalDb : IRepositoryLocal
{
    private const string SAVE_FILE_NAME = "items.crud";

    public void DeleteLocalItemById(ItemLocal itemLocal)
    {
        throw new System.NotImplementedException();
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
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + SAVE_FILE_NAME;
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, listItemsLocal);
        stream.Close();
    }

    public void UpdateLocalItemById(ItemLocal itemLocal)
    {
        throw new System.NotImplementedException();
    }

}
