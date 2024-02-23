using System.Collections.Generic;

public interface IRepositoryLocalTest
{
    List<ItemLocalTest> GetLocalItemsAsync();
    ItemLocalTest GetLocalItemById(string id);
    void SaveLocalItemsAsync(List<ItemLocalTest> listItemsLocal);
}
