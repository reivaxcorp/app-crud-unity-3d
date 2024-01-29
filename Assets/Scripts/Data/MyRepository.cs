using System.Collections.Generic;
using System.Threading.Tasks;

public class MyRepository : IRepositoryLocal, IRepositoryRemote
{
    private readonly IRepositoryLocal localDb;
    private readonly IRepositoryRemote remoteDb;

    public MyRepository(IRepositoryLocal localDb, IRepositoryRemote remoteDb)
    {
        this.localDb = localDb;
        this.remoteDb = remoteDb;
    }

    public void DeleteItemById(ItemLocal itemLocal)
    {
        throw new System.NotImplementedException();
    }

    public void DeleteItemRemoteById(ItemRemote itemLocal)
    {
        throw new System.NotImplementedException();
    }

    public ItemLocal GetItemById(string id)
    {
        throw new System.NotImplementedException();
    }

    public ItemRemote GetItemRemoteById(string id)
    {
        throw new System.NotImplementedException();
    }

    public Task<List<ItemLocal>> GetItems()
    {
        throw new System.NotImplementedException();
    }

    public Task<List<ItemRemote>> GetProductsRemoteAsync()
    {
        throw new System.NotImplementedException();
    }

    public void SaveItem(ItemLocal itemLocal)
    {
        throw new System.NotImplementedException();
    }

    public void SaveItemRemote(string itemName, string remoteFilePath)
    {
        throw new System.NotImplementedException();
    }

    public void UpdateItemById(ItemLocal itemLocal)
    {
        throw new System.NotImplementedException();
    }

    public void UpdateItemRemoteById(ItemRemote itemLocal)
    {
        throw new System.NotImplementedException();
    }
}