using System.Collections.Generic;

public class LocalDbTest : IRepositoryLocalTest
{
    private const string SAVE_FILE_NAME = "items.crud";
   
    public void DeleteLocalItemById(string id)
    {
        ItemLocalTestManager.GetInstance().DeleteItemLocalById(id);
    }

    public ItemLocalTest GetLocalItemById(string id)
    {
        throw new System.NotImplementedException();
    }

    public List<ItemLocalTest> GetLocalItems()
    {
        return ItemLocalTestManager.GetInstance().GetItemsLocal();
    }

    public void SaveLocalItem(ItemLocalTest itemLocal)
    {
        ItemLocalTestManager.GetInstance().SaveItemLocal(itemLocal);
    }

    public void SaveLocalItems(List<ItemLocalTest> listItemsLocal)
    {
        ItemLocalTestManager.GetInstance().SaveItemsLocalList(listItemsLocal);
    }

    public void UpdateLocalItemById(string id)
    {
        throw new System.NotImplementedException();
    }

}
