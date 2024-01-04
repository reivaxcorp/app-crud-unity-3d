using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class RemoteDb : IRepositoryRemote
{
    private FirebaseSDK _firebaseSdk;
    public FirebaseSDK firebaseSdk
    {
        private set { _firebaseSdk = value; }
        get { return _firebaseSdk; }
    }

    public RemoteDb(FirebaseSDK firebaseSdk)
    {
        if (firebaseSdk != null)
        {
            this.firebaseSdk = firebaseSdk;
        }
    }

    public void DeleteItemRemoteById(ItemRemote itemLocal)
    {
        throw new System.NotImplementedException();
    }

    public ItemRemote GetItemRemoteById(string id)
    {
        throw new System.NotImplementedException();
    }

    public Task<List<ItemRemote>> GetProductsRemoteAsync()
    {
        throw new System.NotImplementedException();
    }

    public void SaveItemRemote(ItemRemote itemLocal)
    {
        throw new System.NotImplementedException();
    }

    public void UpdateItemRemoteById(ItemRemote itemLocal)
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
