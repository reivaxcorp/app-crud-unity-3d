using System.Collections.Generic;
using System.Threading.Tasks;

public interface IRepositoryLocal
{
    Task<List<ItemLocal>> GetLocalItemsAsync();
    Task<ItemLocal> GetLocalItemById(string id);
    Task SaveLocalItemsAsync(List<ItemLocal> listItemsLocal);
}
