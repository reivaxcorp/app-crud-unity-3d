using System.Collections.Generic;
using System.Threading.Tasks;

public class RemoteDbTest : IRepositoryRemoteTest
{

    public void DeleteItemRemoteById(string id)
    {
        ItemRemoteTestManager.GetInstance().DeleteItemRemoteById(id);
    }

    public ItemRemoteTest GetItemRemoteById(string id)
    {
        return ItemRemoteTestManager.GetInstance().GetItemRemoteById(id);
    }

    public  List<ItemRemoteTest> GetItemsRemote()
    {
        return ItemRemoteTestManager.GetInstance().GetItemsRemote();
    }

    public RemoteDbTest GetRemoteDb()
    {
        return this;
    }

    public void SaveItemRemote(ItemRemoteTest itemRemote, IResultTest resultUi)
    {
         ItemRemoteTestManager.GetInstance().SaveItemRemote(itemRemote, resultUi);
    }

    public void UpdateItemRemote(ItemRemoteTest itemRemoteTest, IResultTest resultUi)
    {
        ItemRemoteTestManager.GetInstance().UpdateItemRemote(itemRemoteTest, resultUi);
    }
}
