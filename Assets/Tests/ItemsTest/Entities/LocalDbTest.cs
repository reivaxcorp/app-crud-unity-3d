using System.Collections.Generic;

public class LocalDbTest : IRepositoryLocalTest
{

    public ItemLocalTest GetLocalItemById(string id)
    {
        return ItemLocalTestManager.GetInstance().GetLocalItemById(id);
    }

    public List<ItemLocalTest> GetLocalItemsAsync()
    {
        return ItemLocalTestManager.GetInstance().GetItemsLocal();
    }

    public void SaveLocalItemsAsync(List<ItemLocalTest> listItemsLocal)
    {
        ItemLocalTestManager.GetInstance().SaveItemsLocalList(listItemsLocal);
    }

    public void UpdateLocalItemById(string id)
    {
        throw new System.NotImplementedException();
    }

}
