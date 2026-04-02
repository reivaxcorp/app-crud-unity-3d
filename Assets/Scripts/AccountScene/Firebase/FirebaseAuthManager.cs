using Firebase.Auth;
using Firebase.Extensions;
using Google; // Necesitas el plugin de Google Sign-In
using System.Threading.Tasks;
using UnityEngine;

public class FirebaseAuthManager: MonoBehaviour
{
    public delegate void AuthCallback(AccountAuthResult result);
    public event AuthCallback OnAccountAuthResult;

    // Sustituye esto con tu Client ID de la consola de Firebase (Tipo Web)
    private string webClientId = "88826351788-krlrdc0un44kigv8ppknh21noai3in5j.apps.googleusercontent.com";

    public void LoginWithGoogle()
    {
        GoogleSignIn.Configuration = new GoogleSignInConfiguration
        {
            RequestIdToken = true,
            WebClientId = webClientId
        };

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(task => {
            if (task.IsFaulted)
            {
                OnAccountAuthResult?.Invoke(new AccountAuthResult(AuthType.LOGIN_FAILURE, "Google Sign-In Failed"));
            }
            else if (task.IsCanceled)
            {
                OnAccountAuthResult?.Invoke(new AccountAuthResult(AuthType.LOGIN_CANCEL, "Canceled"));
            }
            else
            {
                // Éxito en Google, ahora vamos a Firebase
                SignInWithFirebase(task.Result.IdToken);
            }
        });
    }

    private void SignInWithFirebase(string idToken)
    {
        Credential credential = GoogleAuthProvider.GetCredential(idToken, null);
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
        GoogleSignIn.DefaultInstance.SignOut(); // Cerrar sesión en Google
        FirebaseSDK.GetInstance().LogOut();    // Cerrar sesión en Firebase
        OnAccountAuthResult?.Invoke(new AccountAuthResult(AuthType.LOGOUT, "Sesión cerrada"));
    }
}