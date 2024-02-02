using System.Collections.Generic;
using System.Threading.Tasks;

public interface IRepositoryRemote {
    Task<List<ItemRemote>> GetProductsRemoteAsync();
    ItemRemote GetItemRemoteById(string id);
    void SaveItemRemote(string itemName, string remoteFilePath, IResult resultUi);
    void UpdateItemRemoteById(ItemRemote itemRemote);
    void DeleteItemRemoteById(ItemRemote itemRemote);
}
