using System.Collections.Generic;
using System.Threading.Tasks;

public class ItemRemoteTestManager 
{
    private static ItemRemoteTestManager instance;
    private List<ItemRemoteTest> itemRemoteList;

    public ItemRemoteTestManager()
    {
        itemRemoteList = new List<ItemRemoteTest>();
    }

    public  List<ItemRemoteTest> GetItemsRemote()
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

    public void DeleteItemRemote(ItemRemoteTest itemRemote)
    {
        int existingIndex = itemRemoteList.FindIndex(x => x.Id == itemRemote.Id);

        if (existingIndex != -1)
        {
            // Si el item existe, eliminarlo de la lista
            itemRemoteList.RemoveAt(existingIndex);
        }
       
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
