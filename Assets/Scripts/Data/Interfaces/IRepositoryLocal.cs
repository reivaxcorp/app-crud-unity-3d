using System.Collections.Generic;

public interface IRepositoryLocal
{
    List<ItemLocal> GetLocalItems();
    ItemLocal GetLocalItemById(string id);
    void SaveLocalItems(List<ItemLocal> listItemsLocal);
    void DeleteLocalItemById(string id);
}
