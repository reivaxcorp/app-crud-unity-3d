using System.Collections.Generic;

public class ItemRemoteTestManager
{
    private static ItemRemoteTestManager instance;
    private List<ItemRemoteTest> itemRemoteList;

    public ItemRemoteTestManager()
    {
        itemRemoteList = new List<ItemRemoteTest>();
    }

    public List<ItemRemoteTest> GetItemsRemote()
    {
        return itemRemoteList;
    }

    public ItemRemoteTest GetItemRemoteById(string id)
    {
        return itemRemoteList.Find(item => item.Id.Equals(id));
    }

    public void SaveItemRemote(ItemRemoteTest itemRemote, IResult resultUi)
    {
        itemRemoteList.Add(itemRemote);
        resultUi.SetResultCrudUi(true, "Ítem guardado");
    }

    public void DeleteItemRemoteById(string id)
    {
        int existingIndex = itemRemoteList.FindIndex(x => x.Id == id);

        if (existingIndex != -1)
        {
            // Si el item existe, eliminarlo de la lista
            itemRemoteList.RemoveAt(existingIndex);
        }
    }

    public void UpdateItemRemote(ItemRemoteTest itemRemoteTest, IResult resultUi)
    {
        int existingIndex = itemRemoteList.FindIndex(x => x.Id == itemRemoteTest.Id);

        if (existingIndex != -1)
        {
            // Si el item existe, eliminarlo de la lista
            itemRemoteList.RemoveAt(existingIndex);
            itemRemoteList.Add(itemRemoteTest);
            resultUi.SetResultCrudUi(true, "Se actualizo el documento");
            return;
        }
        resultUi.SetResultCrudUi(true, "No se encuentra el documento");
    }

    public void ClearAllData()
    {
        itemRemoteList?.Clear();
    }

    public static ItemRemoteTestManager GetInstance()
    {
        if (instance == null)
        {
            instance = new ItemRemoteTestManager();
        }
        return instance;
    }

}
