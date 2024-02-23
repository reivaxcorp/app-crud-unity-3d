using System.Collections.Generic;

public class ItemLocalTestManager
{
    private List<ItemLocalTest> itemLocalList;
    private static ItemLocalTestManager instance;

    public ItemLocalTestManager() 
    {
        itemLocalList = new List<ItemLocalTest>();
    }

    public List<ItemLocalTest> GetItemsLocal()
    {
        return itemLocalList;
    }

    public ItemLocalTest GetLocalItemById(string id)
    {
        // Buscar si el item ya existe en la lista
        int existingIndex = itemLocalList.FindIndex(x => x.Id == id);

        if (existingIndex != -1)
        {
            return itemLocalList[existingIndex];
        }
        else
        {
            return null;
        }
    }

    public void SaveItemsLocalList(List<ItemLocalTest> items)
    {
        itemLocalList.Clear(); // En la real, sobreescribimos el archivo.
        itemLocalList.AddRange(items);  
    }

    public void ClearAllData()
    {
        itemLocalList?.Clear();
    }

    public static ItemLocalTestManager GetInstance()
    {
        if(instance == null)
        {
            instance = new ItemLocalTestManager();
        }
        return instance;
    }
}
