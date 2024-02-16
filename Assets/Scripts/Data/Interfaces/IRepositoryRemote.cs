using System.Collections.Generic;
using System.Threading.Tasks;
using static RemoteDb;

public interface IRepositoryRemote {
    Task<List<ItemRemote>> GetItemsRemote();
    ItemRemote GetItemRemoteById(string id);
    void SaveItemRemote(ItemRemote itemRemote, IResult resultUi);
    void UpdateItemRemote(ItemRemote itemRemote, IResult iResult);
    void DeleteItemRemoteById(string id);
    RemoteDb GetRemoteDb();
}
