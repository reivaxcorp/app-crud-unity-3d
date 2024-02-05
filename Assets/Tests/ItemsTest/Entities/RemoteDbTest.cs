using System.Collections.Generic;
using System.Threading.Tasks;

public class RemoteDbTest : IRepositoryRemoteTest
{

    public void DeleteItemRemoteById(ItemRemoteTest itemLocal)
    {
        throw new System.NotImplementedException();
    }

    public ItemRemoteTest GetItemRemoteById(string id)
    {
        throw new System.NotImplementedException();
    }

    public async Task<List<ItemRemoteTest>> GetItemsRemote()
    {
        return await ItemRemoteTestManager.GetInstance().GetItemsRemote();
    }

    public void SaveItemRemote(ItemRemoteTest itemRemote, IResult resultUi)
    {
         ItemRemoteTestManager.GetInstance().SaveItemRemote(itemRemote, resultUi);
    }

    public void UpdateItemRemoteById(ItemRemoteTest itemRemote)
    {
        throw new System.NotImplementedException();
    }

}
