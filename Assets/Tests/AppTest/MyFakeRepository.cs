using System.Collections.Generic;
using UnityEngine;

public class MyFakeRepository : IRepositoryLocalTest, IRepositoryRemoteTest, IDataTextureLocalSavedTest
{
    private readonly IRepositoryLocalTest localDb;
    private readonly IRepositoryRemoteTest remoteDb;
    public MyFakeRepository(IRepositoryLocalTest localDb, IRepositoryRemoteTest remoteDb)
    {
        this.localDb = localDb;
        this.remoteDb = remoteDb;
    }

    public void DeleteItemRemoteById(string id)
    {
        remoteDb.DeleteItemRemoteById(id);
    }

    public ItemLocalTest GetLocalItemById(string id)
    {
       return localDb.GetLocalItemById(id);
    }

    public List<ItemLocalTest> GetLocalItemsAsync()
    {
        return localDb.GetLocalItemsAsync();
    }

    public Texture2D LoadTextureAsPNG(string imageId)
    {
        Debug.Log("Textura textura cargada de forma ficticia " + imageId);
        return new Texture2D(1, 1);
    }

    public void RemoveTexture(string imageId)
    {
        Debug.Log("Texure remove id: " + imageId);
    }

    public void SaveItemRemote(ItemRemoteTest itemRemote, IResultTest resultUi)
    {
        remoteDb.SaveItemRemote(itemRemote, resultUi);
    }

    public void SaveLocalItemsAsync(List<ItemLocalTest> listItemsLocal)
    {
        localDb.SaveLocalItemsAsync(listItemsLocal);
    }

    public void SaveTextureAsPNG(Texture2D textureToSave, string imageId)
    {
        Debug.Log("Textura textura salvada de forma ficticia " + imageId);
    }

    public void UpdateItemRemote(ItemRemoteTest itemRemote, IResultTest iResult)
    {
        remoteDb.UpdateItemRemote(itemRemote, iResult); 
    }

    public RemoteDbTest GetRemoteDb()
    {
        return remoteDb.GetRemoteDb();
    }
}
