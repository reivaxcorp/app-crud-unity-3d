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

    async void Start()
    {
        await UnityServices.InitializeAsync();
        firebaseAuth = FirebaseAuth.DefaultInstance;
        PlayGamesPlatform.Activate();
    }

    public void DoLogin()
    {
        // 1. Primero autenticamos al usuario en el dispositivo
        PlayGamesPlatform.Instance.Authenticate((status) =>
        {
            if (status == SignInStatus.Success)
            {
                Debug.Log("1. Autenticación local exitosa. Ahora solicitando acceso al servidor...");

                // 2. SOLO AQUÍ, una vez confirmado el éxito, pedimos el ServerSideAccess
                PlayGamesPlatform.Instance.RequestServerSideAccess(false, (authCode) =>
                {
                    if (string.IsNullOrEmpty(authCode))
                    {
                        Debug.LogError("Error: El authCode llegó vacío. Revisa el Web Client ID.");
                        return;
                    }

                    Debug.Log("2. AuthCode recibido correctamente. Iniciando sesión dual...");
                    _ = ProcesarLoginDual(authCode);
                });
            }
            else
            {
                Debug.LogError("Fallo el login inicial de Google Play: " + status);
                // Si status es 'Canceled', revisa que tu mail esté en la lista de Testers de la consola
            }
        });
    }


    private async Task ProcesarLoginDual(string authCode)
    {
        try
        {
            // --- PASO A: Firebase ---
            Credential credential = PlayGamesAuthProvider.GetCredential(authCode);
            await firebaseAuth.SignInAndRetrieveDataWithCredentialAsync(credential);
            Debug.Log("Firebase OK");

            // --- PASO B: UGS ---
            await AuthenticationService.Instance.SignInWithGooglePlayGamesAsync(authCode);
            Debug.Log("UGS OK. PlayerID: " + AuthenticationService.Instance.PlayerId);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error en el login dual: " + e.Message);
        }
    }



}


