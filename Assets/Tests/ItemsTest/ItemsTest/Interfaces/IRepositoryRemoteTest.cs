using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.tvOS;

public interface IRepositoryRemoteTest {
    List<ItemRemoteTest> GetItemsRemote();
    ItemRemoteTest GetItemRemoteById(string id);
    void SaveItemRemote(ItemRemoteTest itemRemote, IResult resultUi);
    void UpdateItemRemoteById(string id);
    void DeleteItemRemoteById(string id);
    RemoteDbTest GetRemoteDb();
}
