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
       // if (initialized) return;

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

    public void DoLogin()
    {
       // SceneManager.LoadScene("AppScene");
        //return;
        // Agregamos un Timeout manual o una validación de seguridad
        PlayGamesPlatform.Instance.Authenticate((status) =>
        {
            if (status == SignInStatus.Success)
            {
                // ... tu lógica de éxito ...

                SceneManager.LoadScene("AppScene");
            }
            else
            {
                Debug.LogWarning("Login fallido o cancelado. Estado: " + status);
                // IMPORTANTE: No dejes que la app se quede en un loop infinito.
                // Muestra un botón de "Entrar como invitado" o un mensaje de error 
                // que el revisor de Google pueda cerrar.
            }
        });

        /* // con google play?????
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
        */
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

    public void LogOut()
    {
        FirebaseSDK.GetInstance().LogOut();
        AccountAuthResult result = new AccountAuthResult(AuthType.LOGOUT, "Logged out");
        OnAccountAuthResult?.Invoke(result);
        FirebaseSDK.GetInstance().LogOut();    // Cerrar sesi�n en Firebase
        OnAccountAuthResult?.Invoke(new AccountAuthResult(AuthType.LOGOUT, "Sesi�n cerrada"));
    }

}




