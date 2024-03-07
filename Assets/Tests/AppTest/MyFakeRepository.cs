using System.Collections.Generic;

public class MyFakeRepository : IRepositoryLocalTest, IRepositoryRemoteTest
{
    private IRepositoryLocalTest localDb;
    private IRepositoryRemoteTest remoteDb;
    public MyFakeRepository(IRepositoryLocalTest localDb, IRepositoryRemoteTest remoteDb)
    {
        this.localDb = localDb;
        this.remoteDb = remoteDb;
    }

    public void DeleteItemRemoteById(string id)
    {
        remoteDb.DeleteItemRemoteById(id);
    }

    public List<ItemLocalTest> GetLocalItemsAsync()
    {
        return localDb.GetLocalItemsAsync();
    }

    public void SaveLocalItemsAsync(List<ItemLocalTest> listItemsLocal)
    {
        localDb.SaveLocalItemsAsync(listItemsLocal);
    }

    public void SaveItemRemote(ItemRemoteTest itemRemote, IResultTest resultUi)
    {
        remoteDb.SaveItemRemote(itemRemote, resultUi);
    }

    public void UpdateItemRemote(ItemRemoteTest itemRemote, IResultTest iResult)
    {
        remoteDb.UpdateItemRemote(itemRemote, iResult); 
    }
    public ItemLocalTest GetLocalItemById(string id)
    {
        return localDb.GetLocalItemById(id);
    }

    public RemoteDbTest GetRemoteDb()
    {
        return remoteDb.GetRemoteDb();
    }
    public LocalDbTest GetLocalDb()
    {
        return localDb as LocalDbTest;
    }
}
