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

    public async Task<List<ItemRemoteTest>> GetItemsRemote()
    {
        await Task.Delay(1000); // Espera 1 segundos 
        return itemRemoteList;
    }

    public async void SaveItemRemote(ItemRemoteTest itemRemote)
    {
        await Task.Delay(1000); // fake delay
        itemRemoteList.Add(itemRemote);
    }

    public async void SaveItemRemote(ItemRemoteTest itemRemote, IResult resultUi)
    {
        await Task.Delay(1000); // fake delay
        itemRemoteList.Add(itemRemote);
        resultUi.SetResultCrudUi(true, "Ítem guardado");
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
