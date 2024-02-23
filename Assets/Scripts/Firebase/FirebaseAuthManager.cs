using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;
using Color = UnityEngine.Color;

/// <summary>
/// Manage account actions.
/// </summary>
public class FirebaseAuthManager
{

    public delegate void AuthCallback(AccountAuthResult result);
    public event AuthCallback OnAccountAuthResult;
    private ExceptionManager exceptionManager;

    public FirebaseAuthManager()
    {
        this.exceptionManager = new ExceptionManager();
    }

    public void CreateAccountWithMailAndPassword(string email, string password)
    {
        if (FirebaseSDK.GetInstance().isFirebaseReady)
        {
            FirebaseSDK.GetInstance().auth.CreateUserWithEmailAndPasswordAsync(email, password)
                .ContinueWithOnMainThread(task =>
            {
                AccountAuthResult authResult;

                if (task.IsCanceled)
                {
                    Debug.LogError("Was canceled.");
                    authResult = new AccountAuthResult(AuthType.CREATE_ACCOUNT_CANCEL, "Fue cancelado!");
                    OnAccountAuthResult?.Invoke(authResult);
                    return;
                }
                if (task.IsFaulted)
                {
                    authResult = new AccountAuthResult(AuthType.CREATE_ACCOUNT_FAILURE, exceptionManager.ManageExceptionForm(task));
                    OnAccountAuthResult?.Invoke(authResult);
                    return;
                }

                // Firebase user has been created.
                Firebase.Auth.AuthResult result = task.Result;
                Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                    result.User.DisplayName, result.User.UserId);

                authResult = new AccountAuthResult(AuthType.CREATE_ACCOUNT_SUCCESS, "Cuenta creada");
                OnAccountAuthResult?.Invoke(authResult); // we need TaskScheduler.FromCurrentSync.... to set text
            });

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
                .ContinueWithOnMainThread(task =>
                {
                    AccountAuthResult authResult;

                    if (task.IsCanceled)
                    {
                        Debug.LogError("Was canceled.");
                        authResult = new AccountAuthResult(AuthType.LOGIN_CANCEL, "Login cancelado!");
                        OnAccountAuthResult?.Invoke(authResult);
                        return;
                    }
                    if (task.IsFaulted)
                    {
                        authResult = new AccountAuthResult(AuthType.LOGIN_FAILURE, exceptionManager.ManageExceptionForm(task));
                        OnAccountAuthResult?.Invoke(authResult);
                        return;
                    }

                    AuthResult result = task.Result;
                    Debug.LogFormat("User signed in successfully: {0} ({1})",
                    result.User.DisplayName, result.User.UserId);

                    authResult = new AccountAuthResult(AuthType.LOGIN_SUCCESS,"Logeado como: \n" + result.User.Email);
                    OnAccountAuthResult?.Invoke(authResult);
                });
        }
    }

    public void SendEmailUserVerification()
    {
        if (FirebaseSDK.GetInstance().isFirebaseReady)
        {
            FirebaseSDK.GetInstance().user.SendEmailVerificationAsync()
                .ContinueWithOnMainThread(task =>
                {

                    if (task.IsCanceled)
                    {
                        Debug.LogError("SendEmailVerificationAsync was canceled.");
                        return;
                    }
                    if (task.IsFaulted)
                    {
                        Debug.LogError("SendEmailVerificationAsync encountered an error: " + task.Exception);
                        return;
                    }

                    Debug.Log("Email sent successfully.");
                });
        }
    }

    public void LogOut()
    {
        FirebaseSDK.GetInstance().LogOut();
        AccountAuthResult result = new AccountAuthResult(AuthType.LOGOUT, "Deslogeado");
        OnAccountAuthResult?.Invoke(result);
    }

}
