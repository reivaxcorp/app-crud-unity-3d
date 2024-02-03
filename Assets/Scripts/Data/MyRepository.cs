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

    public List<ItemLocal> GetItems()
    {
        return localDb.GetItems();
    }

    public async Task<List<ItemRemote>> GetProductsRemoteAsync()
    {
        List<ItemRemote>  itemsRemote = await remoteDb.GetProductsRemoteAsync();
      //  List<ItemLocal> itemsLocal = itemsRemote.ItemsRemoteToItemLocal();
       // SaveItemsLocal(itemsLocal);
        return await remoteDb.GetProductsRemoteAsync();
    }

    public void SaveItemsLocal(List<ItemLocal> listItemsLocal)
    {
        localDb.SaveItemsLocal(listItemsLocal);
    }

    public void SaveItemRemote(ItemRemote itemRemote, IResult resultUi)
    {
        remoteDb.SaveItemRemote(itemRemote, resultUi);
    }

    public void UpdateItemById(ItemLocal itemLocal)
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
}