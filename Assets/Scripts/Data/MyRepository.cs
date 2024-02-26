using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MyRepository : IRepositoryLocal, IRepositoryRemote
{
    private readonly IRepositoryLocal localDb;

    private IRepositoryRemote remoteDb;


    public MyRepository(IRepositoryLocal localDb, IRepositoryRemote remoteDb)
    {
        this.localDb = localDb;
        this.remoteDb = remoteDb;
    }

    public async Task<bool> DeleteItemRemoteById(string id, IResult iResultUi)
    {
        return await remoteDb.DeleteItemRemoteById(id, iResultUi);
    }
   
    public async Task<List<ItemLocal>> GetLocalItemsAsync()
    {
        return await localDb.GetLocalItemsAsync();
    }

    public async Task SaveLocalItemsAsync(List<ItemLocal> listItemsLocal)
    {
        await localDb.SaveLocalItemsAsync(listItemsLocal);
    }

    public void SaveItemRemote(ItemRemote itemRemote, IResult resultUi)
    {
        remoteDb.SaveItemRemote(itemRemote, resultUi);
    }

    public void UpdateItemRemote(ItemRemote itemRemote, IResult resultUi)
    {
        remoteDb.UpdateItemRemote(itemRemote, resultUi);
    }

    public async Task<ItemLocal> GetLocalItemById(string id)
    {
        return await localDb.GetLocalItemById(id);
    }

    public RemoteDb GetRemoteDb()
    {
        return remoteDb as RemoteDb;
    }

    public LocalDb GetLocalDb()
    {
        return localDb as LocalDb;
    }
}