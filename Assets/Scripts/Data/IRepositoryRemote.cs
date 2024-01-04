using System.Collections.Generic;
using System.Threading.Tasks;

public interface IRepositoryRemote { 
    Task<List<ItemRemote>> GetProductsRemoteAsync();
    ItemRemote GetItemRemoteById(string id);
    void SaveItemRemote(ItemRemote itemLocal);
    void UpdateItemRemoteById(ItemRemote itemLocal);
    void DeleteItemRemoteById(ItemRemote itemLocal);
}
