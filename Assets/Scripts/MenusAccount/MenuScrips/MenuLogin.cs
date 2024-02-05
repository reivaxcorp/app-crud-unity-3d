using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuLogin : MenuAuth
{
    [SerializeField] private TMP_InputField inputMail;
    [SerializeField] private TMP_InputField inputPassword;

    public void LoginWithExistingAccount()
    {
        if(IsInputsSetted())
        {
            if (validateInputs.IsValidEmail(inputMail.text, resultMsj))
            {
                if (validateInputs.IsValidPassword(inputPassword, resultMsj))
                {
                    firebaseAuthManager.OnAccountAuthResult += SetResult;
                    firebaseAuthManager.LoginWithExistingAccount(inputMail.text, inputPassword.text);
                }
            }
        }
    }

    private bool IsInputsSetted()
    {
        if (inputMail == null)
        {
            Debug.LogWarning("Please put inputMail on inspector");
            return false;
        }
        if (inputPassword == null)
        {
            Debug.LogWarning("Please put inputPassword on inspector");
            return false;
        }
        return true;
    }
}
