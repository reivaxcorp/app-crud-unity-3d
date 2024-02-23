
public interface IRepositoryRemoteTest {
    void SaveItemRemote(ItemRemoteTest itemRemote, IResult resultUi);
    void UpdateItemRemote(ItemRemoteTest itemRemoteTest, IResult resultUi);
    void DeleteItemRemoteById(string id);
    RemoteDbTest GetRemoteDb();
}
