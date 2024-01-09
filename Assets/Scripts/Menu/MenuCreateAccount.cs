using UnityEngine;
using TMPro;

public class MenuCreateAccount : Menu
{
    [SerializeField] private TMP_InputField inputMail;
    [SerializeField] private TMP_InputField inputPassword;
    [SerializeField] private TMP_InputField inputRePassword;

    public void CreateAccountWithMailAndPassword()
    {
        if (IsInputsSetted())
        {
            if (validateInputs.IsValidEmail(inputMail.text, resultMsj))
            {
                if (validateInputs.IsFormatPasswordCorrect(inputPassword, inputRePassword, resultMsj))
                {
                    firebaseAuthManager.OnAccountAuthResult += SetResult;
                    firebaseAuthManager.CreateAccountWithMailAndPassword(inputMail.text, inputPassword.text);
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
        if (inputRePassword == null)
        {
            Debug.LogWarning("Please put inputRePassword on inspector");
            return false;
        }
        return true;
    }
}

