using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface IRepositoryLocal
{
    List<ItemLocal> GetItems();
    ItemLocal GetItemById(string id);
    void SaveItemsLocal(List<ItemLocal> listItemsLocal);
    void UpdateItemById(ItemLocal itemLocal);
    void DeleteItemById(ItemLocal itemLocal);
}
