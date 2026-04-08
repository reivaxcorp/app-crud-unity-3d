using Firebase.Auth;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Authentication;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class FirebaseAuthManager : MonoBehaviour
{
    private FirebaseAuth firebaseAuth;
    private bool initialized = false;

    // Ya no usamos Start() para inicializar Firebase, 
    // lo hacemos bajo demanda o mediante una comprobación.

    private async Task EnsureAuthReady()
    {
        if (initialized) return;

        // Esperamos a que MyApplication termine su trabajo
        while (!MyApplication.IsFirebaseReady)
        {
            await Task.Delay(100); // Pequeña espera para no bloquear el hilo
        }

        await UnityServices.InitializeAsync();

        // Ahora es SEGURO llamar al DefaultInstance
        firebaseAuth = FirebaseAuth.DefaultInstance;
        PlayGamesPlatform.Activate();

        initialized = true;
        Debug.Log("FirebaseAuthManager: Conectado a la instancia validada por MyApplication.");
    }

    public async void DoLogin()
    {
        // Aseguramos la inicialización antes de proceder
        await EnsureAuthReady();

        PlayGamesPlatform.Instance.Authenticate((status) =>
        {
            if (status == SignInStatus.Success)
            {
                Debug.Log("Google Play OK. Solicitando acceso al servidor...");
                PlayGamesPlatform.Instance.RequestServerSideAccess(false, (authCode) =>
                {
                    if (!string.IsNullOrEmpty(authCode))
                    {
                        _ = ProcesarLoginDual(authCode);
                    }
                });
            }
            else
            {
                Debug.LogError("Fallo el login de Google: " + status);
            }
        });
    }

    private async Task ProcesarLoginDual(string authCode)
    {
        try
        {
            // PASO A: Firebase (Usa la instancia que ya validamos)
            Credential credential = PlayGamesAuthProvider.GetCredential(authCode);
            await firebaseAuth.SignInAndRetrieveDataWithCredentialAsync(credential);

            // PASO B: UGS
            await AuthenticationService.Instance.SignInWithGooglePlayGamesAsync(authCode);

            Debug.Log("LOGIN DUAL EXITOSO");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error en el login dual: " + e.Message);
        }
    }
}




