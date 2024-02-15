using System;
using System.Threading.Tasks;
using UnityEngine;

public class MyApplication : MonoBehaviour
{
    public static MyRepository repository
    {
        private set { _repository = value; }
        get { return _repository; }
    }
    private static MyRepository _repository;


    private async void Start()
    {
        await CreateRepositoryAsync();
    }

    private async Task<MyRepository> CreateRepositoryAsync()
    {
        try
        {
            // FIRST wait to initialize Sdk Firebase
            await InicializeFirebase();

            RemoteDb remoteDb = new RemoteDb();
            LocalDb localDb = new LocalDb();
            repository = new MyRepository(localDb, remoteDb);
            return repository;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
        
        throw new Exception("Error al crear el respositorio");
    }

    // we wait firebase start
    private async Task<FirebaseSDK> InicializeFirebase()
    {
        FirebaseSDK firebaseSdk = FirebaseSDK.GetInstance();

        try
        {
            bool firebaseInitialized = await firebaseSdk.InitFirebaseDependenciesAsync();

            if (firebaseInitialized)
            {
                Debug.Log($"Firebase running");
                return firebaseSdk;
            }
            else
            {
                // Handle the exception where Firebase initialization failed.
                Debug.Log($"Firebase initialization it's false");
                throw new Exception("Firebase initialization it's false");
            }
        }
        catch (Exception ex)
        {
            // Handle the exception where Firebase initialization failed.
            Debug.Log($"Firebase initialization error: {ex.Message}");
            throw new Exception($"Firebase initialization error: {ex.Message}");
        }

    }

}
