using System.Collections.Generic;
using System.Threading.Tasks;

public interface IRepositoryRemote {
    Task<List<ItemRemote>> GetItemsRemote();
    ItemRemote GetItemRemoteById(string id);
    void SaveItemRemote(ItemRemote itemRemote, IResult resultUi);
    void UpdateItemRemoteById(ItemRemote itemRemote);
    void DeleteItemRemote(ItemRemote itemRemote);
}
