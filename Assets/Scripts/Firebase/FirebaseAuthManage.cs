using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using UnityEngine;
using Color = UnityEngine.Color;

public class FirebaseAuthManage
{
    public delegate void AuthCallback(string message, Color color);
    public event AuthCallback OnAccountCreated;

    private ExceptionManager exceptionManager;

    public FirebaseAuthManage()
    {
        this.exceptionManager = new ExceptionManager();
    }

    public void CreateAccountWithMailAndPassword(string mail, string password)
    {
        if (FirebaseSDK.GetInstance().isFirebaseReady)
        {
            FirebaseSDK.GetInstance().auth.CreateUserWithEmailAndPasswordAsync(mail, password).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("Was canceled.");
                    OnAccountCreated.Invoke("Fue cancelado!", Color.red);
                    return;
                }
                if (task.IsFaulted)
                {
                    OnAccountCreated.Invoke(exceptionManager.ManageExceptionForm(task), Color.red);
                    return;
                }

                // Firebase user has been created.
                Firebase.Auth.AuthResult result = task.Result;
                Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                    result.User.DisplayName, result.User.UserId);
                OnAccountCreated.Invoke("Cuenta creada", Color.green); // we need TaskScheduler.FromCurrentSync.... to set text
            },
              TaskScheduler.FromCurrentSynchronizationContext() // Execute in main thread of Unity. ('case we need to update text "cuenta creada")
            );

        }
        else
        {
            Debug.LogWarning("Firebase isn't running!");
        }
    }

}
