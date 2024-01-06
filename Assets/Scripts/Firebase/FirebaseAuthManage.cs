using Firebase.Auth;
using System.Threading.Tasks;
using UnityEngine;
using Color = UnityEngine.Color;

/// <summary>
/// Manage account actions.
/// </summary>
public class FirebaseAuthManage
{
    
    public delegate void AuthCallback(AccountAuthResult result);
    public event AuthCallback OnAccountAuthResult;
    private ExceptionManager exceptionManager;

    public FirebaseAuthManage()
    {
        this.exceptionManager = new ExceptionManager();
    }

    public void CreateAccountWithMailAndPassword(string email, string password)
    {
        if (FirebaseSDK.GetInstance().isFirebaseReady)
        {
            FirebaseSDK.GetInstance().auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
            {
                AccountAuthResult authResult;

                if (task.IsCanceled)
                {
                    Debug.LogError("Was canceled.");
                    authResult = new AccountAuthResult("Fue cancelado!", Color.red, false);
                    OnAccountAuthResult?.Invoke(authResult);
                    return;
                }
                if (task.IsFaulted)
                {
                    authResult = new AccountAuthResult(exceptionManager.ManageExceptionForm(task), Color.red, false);
                    OnAccountAuthResult?.Invoke(authResult);
                    return;
                }

                // Firebase user has been created.
                Firebase.Auth.AuthResult result = task.Result;
                Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                    result.User.DisplayName, result.User.UserId);

                authResult = new AccountAuthResult("Cuenta creada", Color.green, true);
                OnAccountAuthResult?.Invoke(authResult); // we need TaskScheduler.FromCurrentSync.... to set text
            },
              TaskScheduler.FromCurrentSynchronizationContext() // Execute in main thread of Unity. ('case we need to update text "cuenta creada")
            );

        }
        else
        {
            Debug.LogWarning("Firebase isn't running!");
        }
    }

    public void LoginWithExistingAccount(string email, string password)
    {

        if (FirebaseSDK.GetInstance().isFirebaseReady)
        {
            FirebaseSDK.GetInstance()
                .auth
                .SignInWithEmailAndPasswordAsync(
                email,
                password)
                .ContinueWith(task =>
                {
                    AccountAuthResult authResult;

                    if (task.IsCanceled)
                    {
                        Debug.LogError("Was canceled.");
                        authResult = new AccountAuthResult("Fue cancelado!", Color.red, false);
                        OnAccountAuthResult?.Invoke(authResult);
                        return;
                    }
                    if (task.IsFaulted)
                    {
                        authResult = new AccountAuthResult(exceptionManager.ManageExceptionForm(task), Color.red, false);
                        OnAccountAuthResult?.Invoke(authResult);
                        return;
                    }

                    AuthResult result = task.Result;
                    Debug.LogFormat("User signed in successfully: {0} ({1})",
                    result.User.DisplayName, result.User.UserId);

                    authResult = new AccountAuthResult("Logeado como: \n" + result.User.Email, Color.green, true); // we need TaskScheduler.FromCurrentSync.... to set text
                    OnAccountAuthResult?.Invoke(authResult);
                },
              TaskScheduler.FromCurrentSynchronizationContext() // Execute in main thread of Unity. ('case we need to update text "cuenta creada")
            );
        }
    }

    public void LogOut()
    {
        FirebaseSDK.GetInstance().LogOut();
        AccountAuthResult result = new AccountAuthResult("", Color.white, false);
        OnAccountAuthResult?.Invoke(result);
    }
}
