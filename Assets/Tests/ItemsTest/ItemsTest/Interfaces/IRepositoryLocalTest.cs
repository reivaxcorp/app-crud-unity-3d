using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface IRepositoryLocalTest
{
    List<ItemLocalTest> GetLocalItems();
    ItemLocalTest GetLocalItemById(string id);
    void SaveLocalItem(ItemLocalTest itemLocal);
    void SaveLocalItems(List<ItemLocalTest> listItemsLocal);
    void UpdateLocalItemById(ItemLocalTest itemLocal);
    void DeleteLocalItemById(ItemLocalTest itemLocal);
}
