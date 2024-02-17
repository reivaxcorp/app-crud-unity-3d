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
         localDb.DeleteLocalItemById(id);
    }

    public void DeleteItemRemoteById(string id, IResult iResult)
    {
        remoteDb.DeleteItemRemoteById(id, iResult);
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

    public void UpdateItemRemote(ItemRemote itemRemote, IResult iResult)
    {
        remoteDb.UpdateItemRemote(itemRemote, iResult);
    }

    public void SaveTextureAsPNG(Texture2D textureToSave, string imageName)
    {
        textureManager.SaveTextureAsPNG(textureToSave, imageName);
    }

    public void RemoveLocalTexture(string imageId)
    {
        textureManager.RemoveLocalTexture(imageId);
    }

    public Texture2D LoadTextureAsPNG(string imageName)
    {
        return textureManager.LoadTextureAsPNG(imageName);
    }

    public ItemLocal GetLocalItemById(string id)
    {
        return localDb.GetLocalItemById(id);
    }

    public RemoteDb GetRemoteDb()
    {
        return remoteDb as RemoteDb;
    }
}