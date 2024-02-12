using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MyRepository : IRepositoryLocal, IRepositoryRemote, IDataTextureLocalSaved
{
    private readonly IRepositoryLocal localDb;

    private IRepositoryRemote remoteDb;

    private TextureManager textureManager;

    public MyRepository(IRepositoryLocal localDb, IRepositoryRemote remoteDb)
    {
        this.textureManager = new TextureManager();
        this.localDb = localDb;
        this.remoteDb = remoteDb;
    }

    public void DeleteLocalItemById(string id)
    {
        textureManager.RemoveTexture(id);
        localDb.DeleteLocalItemById(id);
    }

    public void DeleteItemRemoteById(string id)
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

    public void UpdateLocalItemById(string id)
    {
        throw new System.NotImplementedException();
    }

    public void UpdateItemRemoteById(string id)
    {
        throw new System.NotImplementedException();
    }

    public void SaveTextureAsPNG(Texture2D textureToSave, string imageId)
    {
        textureManager.SaveTextureAsPNG(textureToSave, imageId);
    }

    public void RemoveTexture(string imageId)
    {
        textureManager.RemoveTexture(imageId);
    }

    public Texture2D LoadTextureAsPNG(string imageId)
    {
        return textureManager.LoadTextureAsPNG(imageId);
    }

    public ItemLocal GetLocalItemById(string id)
    {
        throw new System.NotImplementedException();
    }

    public void SaveLocalItem(ItemLocal itemLocal)
    {
        localDb.SaveLocalItem(itemLocal);
    }

    public RemoteDb GetRemoteDb()
    {
        return remoteDb as RemoteDb;
    }
}