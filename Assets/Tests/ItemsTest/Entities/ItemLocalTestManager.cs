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

    public void SaveItemLocal(ItemLocalTest itemLocal)
    {
        // Buscar si el item ya existe en la lista
        int existingIndex = itemLocalList.FindIndex(x => x.Id == itemLocal.Id);

        if (existingIndex != -1)
        {
            // Si el item existe, eliminarlo de la lista
            itemLocalList.RemoveAt(existingIndex);

            // Insertar el nuevo item en la misma posición
            itemLocalList.Insert(existingIndex, itemLocal);
        }
        else
        {
            // Si el item no existe, agregarlo al final de la lista
            itemLocalList.Add(itemLocal);
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
