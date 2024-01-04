using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Threading.Tasks;

public class MenuCreateAccount : Menu
{
    [SerializeField] private TMP_InputField inputMail;
    [SerializeField] private TMP_InputField inputPassword;
    [SerializeField] private TMP_InputField inputRePassword;
    private ValidateInputs validateInputs;
    private ExceptionManager exceptionManager;


    private void Awake()
    {
        validateInputs = gameObject.AddComponent<ValidateInputs>();
        exceptionManager = gameObject.AddComponent<ExceptionManager>();
    }

    public override void SetMenuResult(string name)
    {
        if (resultMsj != null)
        {
            resultMsj.text = name;
        }
        else
        {
            Debug.LogWarning("msj result menu is null");
        }
    }

    public void CreateAccountWithMailAndPassword()
    {
        if (FirebaseSDK.GetInstance().isFirebaseReady)
        {
            if (validateInputs.IsValidEmail(inputMail.text))
            {
                if (validateInputs.IsFormatPasswordCorrect(inputPassword, inputRePassword, resultMsj))
                {
                    FirebaseSDK.GetInstance().auth.CreateUserWithEmailAndPasswordAsync(inputMail.text, inputPassword.text).ContinueWith(task =>
                    {
                        if (task.IsCanceled)
                        {
                            Debug.LogError("Was canceled.");
                            SetMessageResult("Fue cancelado!", Color.red);
                            return;
                        }
                        if (task.IsFaulted)
                        {
                            SetMessageResult(exceptionManager.ManageExceptionForm(task), Color.red);
                            return;
                        }

                        // Firebase user has been created.
                        Firebase.Auth.AuthResult result = task.Result;
                        Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                            result.User.DisplayName, result.User.UserId);

                        resultMsj.SetText("Cuenta creada"); // we need TaskScheduler.FromCurrentSync.... to set text
                    },
                      TaskScheduler.FromCurrentSynchronizationContext() // Execute in main thread of Unity. ('case we need to update text "cuenta creada")
                    );
                }

            }
            else
            {
                SetMessageResult("Formato de email no valido!", Color.red);
            }
        }
        else
        {
            Debug.LogWarning("Firebase isn't running!");
        }
    }

    private void SetMessageResult(string msj, Color color)
    {
        resultMsj.SetText(msj);
        resultMsj.color = color;
    }
}
