using System.Collections.Generic;
using System.Threading.Tasks;

public interface IRepositoryLocal
{
    Task<List<ItemLocal>> GetItems();
    ItemLocal GetItemById(string id);
    void SaveItem(ItemLocal itemLocal);
    void UpdateItemById(ItemLocal itemLocal);
    void DeleteItemById(ItemLocal itemLocal);
}
