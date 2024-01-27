using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class RemoteDb : IRepositoryRemote
{

    public void DeleteItemRemoteById(ItemRemote itemLocal)
    {
        throw new System.NotImplementedException();
    }

    public ItemRemote GetItemRemoteById(string id)
    {
        throw new System.NotImplementedException();
    }

    public Task<List<ItemRemote>> GetProductsRemoteAsync()
    {
        throw new System.NotImplementedException();
    }

    public void SaveItemRemote(ItemRemote itemRemote)
    {
        throw new System.NotImplementedException();
    }

    public void UpdateItemRemoteById(ItemRemote itemRemote)
    {
        throw new System.NotImplementedException();
    }

}
