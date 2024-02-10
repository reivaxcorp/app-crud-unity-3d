using System.Collections.Generic;

public interface IRepositoryLocal
{
    List<ItemLocal> GetLocalItems();
    ItemLocal GetLocalItemById(string id);
    void SaveLocalItem(ItemLocal itemLocal);
    void SaveLocalItems(List<ItemLocal> listItemsLocal);
    void UpdateLocalItemById(string id);
    void DeleteLocalItemById(string id);
}
