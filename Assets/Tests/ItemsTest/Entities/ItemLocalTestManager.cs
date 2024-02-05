using System.Collections.Generic;

public class ItemLocalTestManager
{
    private List<ItemLocalTest> itemLocalList;
    private static ItemLocalTestManager instance;

    public ItemLocalTestManager() 
    {
        itemLocalList = new List<ItemLocalTest>();
    }

    public List<ItemLocalTest> GetItemsLocal()
    {
        return itemLocalList;
    }

    public void SaveItemLocal(ItemLocalTest itemLocal)
    {
        itemLocalList.Add(itemLocal);
    }

    public void SaveItemsLocalList(List<ItemLocalTest> items)
    {
        itemLocalList.AddRange(items);  
    }
  
    public static ItemLocalTestManager GetInstance()
    {
        if(instance == null)
        {
            instance = new ItemLocalTestManager();
        }
        return instance;
    }
}
