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
    private FirebaseAuthManage firebaseAuthManage;
    private ValidateInputs validateInputs;


    private void Awake()
    {
        validateInputs = gameObject.AddComponent<ValidateInputs>();
    }

    private void Start()
    {
         firebaseAuthManage = new FirebaseAuthManage();
    }

    public void CreateAccountWithMailAndPassword()
    {
        if (FirebaseSDK.GetInstance().isFirebaseReady)
        {
            if (validateInputs.IsValidEmail(inputMail.text))
            {
                if (validateInputs.IsFormatPasswordCorrect(inputPassword, inputRePassword, resultMsj))
                {
                    firebaseAuthManage.CreateAccountWithMailAndPassword(inputMail.text, inputPassword.text);
                    firebaseAuthManage.OnAccountCreated += SetMsjResult;
                }

            }
            else
            {
                SetMsjResult("Formato de email no valido!", Color.red);
            }
        }
        else
        {
            Debug.LogWarning("Firebase isn't running!");
        }
    }

    // desuscribe to prevent memory leak
    private void DesuscribeEvent()
    {
        if (firebaseAuthManage != null)
        {
            // Desuscripción del evento OnAccountCreated
            firebaseAuthManage.OnAccountCreated -= SetMsjResult;
        }
    }

    // call it when we use SetActive "false"
    private void OnDisable()
    {
        DesuscribeEvent();
    }

    private void OnDestroy()
    {
        DesuscribeEvent();
    }

}
