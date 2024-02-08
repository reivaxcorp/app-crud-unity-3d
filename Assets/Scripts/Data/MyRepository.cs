using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MyRepository : IRepositoryLocal, IRepositoryRemote, IDataTextureLocalSaved
{
    private readonly IRepositoryLocal localDb;
    private readonly IRepositoryRemote remoteDb;

    public MyRepository(IRepositoryLocal localDb, IRepositoryRemote remoteDb)
    {
        this.localDb = localDb;
        this.remoteDb = remoteDb;
    }

    public void DeleteLocalItemById(ItemLocal itemLocal)
    {
        throw new System.NotImplementedException();
    }

    public void DeleteItemRemote(ItemRemote itemLocal)
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

    public List<ItemLocal> GetLocalItems()
    {
        return localDb.GetLocalItems();
    }

    public async Task<List<ItemRemote>> GetItemsRemote()
    {
         return await remoteDb.GetItemsRemote();
    }

    public void SaveLocalItems(List<ItemLocal> listItemsLocal)
    {
        localDb.SaveLocalItems(listItemsLocal);
    }

    public void SaveItemRemote(ItemRemote itemRemote, IResult resultUi)
    {
        remoteDb.SaveItemRemote(itemRemote, resultUi);
    }

    public void UpdateLocalItemById(ItemLocal itemLocal)
    {
        throw new System.NotImplementedException();
    }

    public void UpdateItemRemoteById(ItemRemote itemLocal)
    {
        throw new System.NotImplementedException();
    }

    public void SaveTextureAsPNG(Texture2D textureToSave, string imageId)
    {
        throw new System.NotImplementedException();
    }

    public void RemoveTexture(string imageId)
    {
        throw new System.NotImplementedException();
    }

    public Texture2D LoadTextureAsPNG(string imageId)
    {
        throw new System.NotImplementedException();
    }

    public ItemLocal GetLocalItemById(string id)
    {
        throw new System.NotImplementedException();
    }

    public void SaveLocalItem(ItemLocal itemLocal)
    {
        localDb.SaveLocalItem(itemLocal);
    }
}