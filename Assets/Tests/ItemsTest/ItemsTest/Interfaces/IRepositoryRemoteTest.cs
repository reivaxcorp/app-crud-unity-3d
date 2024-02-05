using System.Collections.Generic;
using System.Threading.Tasks;

public interface IRepositoryRemoteTest {
    Task<List<ItemRemoteTest>> GetItemsRemote();
    ItemRemoteTest GetItemRemoteById(string id);
    void SaveItemRemote(ItemRemoteTest itemRemote, IResult resultUi);
    void UpdateItemRemoteById(ItemRemoteTest itemRemote);
    void DeleteItemRemoteById(ItemRemoteTest itemRemote);
}
