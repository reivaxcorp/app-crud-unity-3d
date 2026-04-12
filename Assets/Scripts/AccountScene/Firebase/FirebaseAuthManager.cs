using Firebase.Auth;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirebaseAuthManager : MonoBehaviour
{
    private FirebaseAuth firebaseAuth;
    private bool initialized = false;

    public delegate void AuthCallback(AccountAuthResult result);
    public event AuthCallback OnAccountAuthResult;

    private void Start()
    {
        _ =  EnsureAuthReady();
    }

    private async Task EnsureAuthReady()
    {
        if (initialized) return;

        // Esperamos a que MyApplication termine su trabajo
        while (!MyApplication.IsFirebaseReady)
        {
            await Task.Delay(100); // Pequeña espera para no bloquear el hilo
        }

        firebaseAuth = FirebaseAuth.DefaultInstance;

        initialized = true;
        Debug.Log("FirebaseAuthManager: Conectado a la instancia validada por MyApplication.");
        
        await DoLogin();
    }

    public async Task DoLogin()
    {
        try
        {
            var result = await firebaseAuth.SignInAnonymouslyAsync();
            Debug.Log($"Login Anónimo OK. User ID: {result.User.UserId}");
            SceneManager.LoadScene("AppScene");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error en login anónimo: " + e.Message);
        }
    }
}




