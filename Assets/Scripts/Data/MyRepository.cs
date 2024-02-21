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

    public async Task DeleteLocalItemById(string id)
    {
         await localDb.DeleteLocalItemById(id);
    }

    public async Task<bool> DeleteItemRemoteById(string id, IResult iResult)
    {
        return await remoteDb.DeleteItemRemoteById(id, iResult);
    }
   
    public async Task<List<ItemLocal>> GetLocalItemsAsync()
    {
        return await localDb.GetLocalItemsAsync();
    }

    public async Task<List<ItemRemote>> GetItemsRemote()
    {
         return await remoteDb.GetItemsRemote();
    }

    public async Task SaveLocalItemsAsync(List<ItemLocal> listItemsLocal)
    {
        await localDb.SaveLocalItemsAsync(listItemsLocal);
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

    public async Task<ItemLocal> GetLocalItemById(string id)
    {
        return await localDb.GetLocalItemById(id);
    }

    public RemoteDb GetRemoteDb()
    {
        return remoteDb as RemoteDb;
    }
}