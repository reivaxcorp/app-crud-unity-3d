using Firebase.Auth;
using Firebase.Extensions;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

public class FirebaseAuthManager : MonoBehaviour
{
    public delegate void AuthCallback(AccountAuthResult result);
    public event AuthCallback OnAccountAuthResult;

    void Start()
    {
        // En v2.x.x ya no existe InitializeInstance ni Configuration. 
        // Solo se activa y él lee los ajustes del menú Window > Google Play Games.
        PlayGamesPlatform.Activate();
    }

    public void LoginWithGoogle()
    {
        // En v2.x.x se usa SignIn en lugar de Authenticate para mayor claridad
        PlayGamesPlatform.Instance.Authenticate(status => {
            if (status == SignInStatus.Success)
            {
                // CAMBIO CLAVE: RequestServerSideAccess es el nuevo nombre
                PlayGamesPlatform.Instance.RequestServerSideAccess(false, (authCode) => {
                    SignInWithFirebase(authCode);
                });
            }
            else
            {
                OnAccountAuthResult?.Invoke(new AccountAuthResult(AuthType.LOGIN_FAILURE, "Google Login Failed: " + status));
            }
        });
    }

    private void SignInWithFirebase(string authCode)
    {
        // PlayGamesAuthProvider sigue siendo el puente con Firebase
        Credential credential = PlayGamesAuthProvider.GetCredential(authCode);

        FirebaseSDK.GetInstance().auth.SignInAndRetrieveDataWithCredentialAsync(credential)
            .ContinueWithOnMainThread(task => {
                if (task.IsFaulted || task.IsCanceled)
                {
                    OnAccountAuthResult?.Invoke(new AccountAuthResult(AuthType.LOGIN_FAILURE, "Firebase Auth Failed"));
                    return;
                }

                Firebase.Auth.AuthResult result = task.Result;
                OnAccountAuthResult?.Invoke(new AccountAuthResult(AuthType.LOGIN_SUCCESS, "Bienvenido: " + result.User.DisplayName));
            });
    }

    public void LogOut()
    {
        // Cerrar sesión en Firebase
        FirebaseAuth.DefaultInstance.SignOut();

        // Re-inicializar la plataforma para limpiar el estado de GPGS
        PlayGamesPlatform.Instance.Authenticate((_) => { }); // opcional: forzar re-auth al próximo login

        // O directamente limpiar el estado interno desactivando:
        Social.Active = null;
        PlayGamesPlatform.Activate(); // re-activa limpia

        Debug.Log("Sesión de Firebase cerrada. GPGS reseteado.");
    }
}