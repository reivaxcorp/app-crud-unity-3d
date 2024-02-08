using System.Collections.Generic;
using System.Threading.Tasks;

public class RemoteDbTest : IRepositoryRemoteTest
{

    public void DeleteItemRemote(ItemRemoteTest itemRemote)
    {
        ItemRemoteTestManager.GetInstance().DeleteItemRemote(itemRemote);
    }

    public ItemRemoteTest GetItemRemoteById(string id)
    {
        return ItemRemoteTestManager.GetInstance().GetItemRemoteById(id);
    }

    public  List<ItemRemoteTest> GetItemsRemote()
    {
        return ItemRemoteTestManager.GetInstance().GetItemsRemote();
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
