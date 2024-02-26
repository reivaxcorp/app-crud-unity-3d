
public interface IRepositoryRemoteTest {
    void SaveItemRemote(ItemRemoteTest itemRemote, IResultTest resultUi);
    void UpdateItemRemote(ItemRemoteTest itemRemoteTest, IResultTest resultUi);
    void DeleteItemRemoteById(string id);
    RemoteDbTest GetRemoteDb();
}
