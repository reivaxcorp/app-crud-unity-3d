using System;
using System.Threading.Tasks;
using UnityEngine;

public class MyApplication : MonoBehaviour
{

    private static MyRepository _repository;
    public static MyRepository repository
    {
        private set { _repository = value; }
        get { return _repository; }
    }

    public async Task<MyRepository> GetRepositoryAsync()
    {
        if (repository == null)
        {
            await CreateRepositoryAsync();
        }
        return repository;
    }

    private async Task<MyRepository> CreateRepositoryAsync()
    {
        // FIRST wait to initialize Sdk Firebase
        FirebaseSDK firebaseSDK = await InicializeFirebaseSdk();

        RemoteDb remoteDb = new RemoteDb(firebaseSDK);
        LocalDb localDb = new LocalDb();
        repository = new MyRepository(localDb, remoteDb);
        return repository;
    }

    // we wait firebase start
    private async Task<FirebaseSDK> InicializeFirebaseSdk()
    {
        FirebaseSDK firebaseSdk = FirebaseSDK.GetInstance();

        try
        {
            bool firebaseInitialized = await firebaseSdk.InitFirebaseAsync();

            if (firebaseInitialized)
            {
                Debug.Log($"Firebase running");
                return firebaseSdk;
            }
            else
            {
                // Handle the exception where Firebase initialization failed.
                Debug.Log($"Firebase initialization it's false");
                return null;
            }
        }
        catch (Exception ex)
        {
            // Handle the exception where Firebase initialization failed.
            Debug.Log($"Firebase initialization error: {ex.Message}");
            return null;
        }

    }

    private async void Start()
    {
        await GetRepositoryAsync();
    }
}
