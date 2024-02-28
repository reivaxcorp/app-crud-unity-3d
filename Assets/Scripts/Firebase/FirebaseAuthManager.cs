/*********************************************************************************
 * Nombre del Archivo:     FirebaseAuthManager.cs
 * Descripción:            LLamadas al sdk de firebase para la cración de cuentas e inicio de sesión de 
 *                         usuario, nos ayudamos por medio de eventos, al suscribirnos en las clases 
 *                         correspondientes. 
 *                         
 * Autor:                  Javier
 * Organización:           ReivaxCorp.
 *
 * Derechos de Autor (c) [2024] ReivaxCorp
 * 
 * Permiso es otorgado, sin cargo, para que cualquier persona obtenga una copia
 * de este software y de los archivos de documentación asociados (el "Software"),
 * para tratar en el Software sin restricción, incluyendo sin limitación los
 * derechos para usar, copiar, modificar, fusionar, publicar, distribuir,
 * sublicenciar, y/o vender copias del Software, y para permitir a las personas a
 * quienes pertenezca el Software, sujeto a las siguientes condiciones:
 *
 * El aviso de derechos de autor anterior y este aviso de permiso se incluirán en
 * todas las copias o partes sustanciales del Software.
 *
 * EL SOFTWARE SE PROPORCIONA "TAL CUAL", SIN GARANTÍA DE NINGÚN TIPO, EXPRESA O
 * IMPLÍCITA, INCLUYENDO PERO NO LIMITADO A LAS GARANTÍAS DE COMERCIABILIDAD,
 * IDONEIDAD PARA UN PROPÓSITO PARTICULAR Y NO INFRACCIÓN. EN NINGÚN CASO LOS
 * AUTORES O TITULARES DE DERECHOS DE AUTOR SERÁN RESPONSABLES DE CUALQUIER
 * RECLAMACIÓN, DAÑO O OTRA RESPONSABILIDAD, YA SEA EN UNA ACCIÓN DE CONTRATO, AGRAVIO
 * O DE OTRO MODO, DERIVADAS DE, FUERA DE O EN CONEXIÓN CON EL SOFTWARE O EL USO U OTROS
 * TRATOS EN EL SOFTWARE.
 *********************************************************************************/

using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;

/// <summary>
/// Manejar las acciones de auntentificación.
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
                    authResult = new AccountAuthResult(AuthType.CREATE_ACCOUNT_CANCEL, "¡Creación de cuenta cancelada!");
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
                        authResult = new AccountAuthResult(AuthType.LOGIN_CANCEL, "¡Login cancelado!");
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
            FirebaseSDK.GetInstance().auth.CurrentUser.SendEmailVerificationAsync()
                .ContinueWithOnMainThread(task =>
                {
                    AccountAuthResult authResult;

                    if (task.IsCanceled)
                    {
                        Debug.LogError("SendEmailVerificationAsync was canceled.");
                        authResult = new AccountAuthResult(AuthType.SEND_MAIL_VERIFICATION_CANCEL, "Email de verificación cancelado");
                        OnAccountAuthResult?.Invoke(authResult);
                        return;
                    }
                    if (task.IsFaulted)
                    {
                        Debug.LogError("SendEmailVerificationAsync encountered an error: " + task.Exception);
                        authResult = new AccountAuthResult(AuthType.SEND_MAIL_VERIFICATION_FAILURE, "Error al enviar el email de verificación");
                        OnAccountAuthResult?.Invoke(authResult);
                        return;
                    }

                    authResult = new AccountAuthResult(AuthType.SEND_MAIL_VERIFICATION_SUCCESS, "Se acaba de enviar el email de verificación\nVerifica tu mail e inicia sesión");
                    OnAccountAuthResult?.Invoke(authResult);
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
