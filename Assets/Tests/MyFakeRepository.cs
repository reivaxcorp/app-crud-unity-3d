using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    public void DeleteLocalItemById(string id)
    {
        localDb.DeleteLocalItemById(id);
    }

    public void DeleteItemRemoteById(string id)
    {
        remoteDb.DeleteItemRemoteById(id);
    }

    public ItemLocalTest GetLocalItemById(string id)
    {
        throw new System.NotImplementedException();
    }

    public ItemRemoteTest GetItemRemoteById(string id)
    {
        return remoteDb.GetItemRemoteById(id);
    }

    public List<ItemLocalTest> GetLocalItems()
    {
        return localDb.GetLocalItems();
    }

 
    public Texture2D LoadTextureAsPNG(string imageId)
    {
        throw new System.NotImplementedException();
    }

    public void RemoveTexture(string imageId)
    {
        Debug.Log("Texure remove id: " + imageId);
    }

    public void SaveItemRemote(ItemRemoteTest itemRemote, IResult resultUi)
    {
        remoteDb.SaveItemRemote(itemRemote, resultUi);
    }

    public void SaveLocalItems(List<ItemLocalTest> listItemsLocal)
    {
        localDb.SaveLocalItems(listItemsLocal);
    }

    public void SaveTextureAsPNG(Texture2D textureToSave, string imageId)
    {
        throw new System.NotImplementedException();
    }

    public void UpdateLocalItemById(string id)
    {
        throw new System.NotImplementedException();
    }

    public void UpdateItemRemoteById(string id)
    {
        throw new System.NotImplementedException();
    }

    public void SaveLocalItem(ItemLocalTest itemLocal)
    {
        localDb.SaveLocalItem(itemLocal);
    }

    public List<ItemRemoteTest> GetItemsRemote()
    {
        return remoteDb.GetItemsRemote();
    }

    public RemoteDbTest GetRemoteDb()
    {
        throw new System.NotImplementedException();
    }
}
