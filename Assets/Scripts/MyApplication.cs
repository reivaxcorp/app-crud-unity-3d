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

        RemoteDb remoteDb = new RemoteDb();
        LocalDb localDb = new LocalDb();
        TextureManager textureManager = new TextureManager();

        repository = new MyRepository(localDb, remoteDb, textureManager);

        // FIRST wait to initialize Sdk Firebase
        bool isInitialize = await InicializeFirebase();

        if (isInitialize)
        {
            return repository;
        }
        else
        {
            Debug.LogWarning("Error al crear el respositorio");
            return null;
        }
    }

    // we wait firebase start
    private async Task<bool> InicializeFirebase()
    {
        FirebaseSDK firebaseSdk = FirebaseSDK.GetInstance();

        try
        {
            bool firebaseInitialized = await firebaseSdk.InitFirebaseDependenciesAsync();

            if (firebaseInitialized)
            {
                Debug.Log($"Firebase running");
                return firebaseInitialized;
            }
            else
            {
                // Handle the exception where Firebase initialization failed.
                Debug.Log($"Firebase initialization it's false");
                return false;
            }
        }
        catch (Exception ex)
        {
            // Handle the exception where Firebase initialization failed.
            Debug.Log($"Firebase initialization error: {ex.Message}");
            return false;
        }
    }

}
