using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class ValidateMenuInputs : MonoBehaviour
{

    public bool IsValidEmail(string email, TextMeshProUGUI msjLoginResult)
    {
        // Defines a regular expression to validate emails.
        string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        // Checks if the user input matches the pattern.
        if (Regex.IsMatch(email, emailPattern))
        {
            return true;
        }
        else
        {
            msjLoginResult.SetText("Formato de email no valido!");
            return false;
        }
    }

    public bool IsValidPassword(TMP_InputField password, TextMeshProUGUI msjLoginResult)
    {

        if (password.text.Length > 5)
        {
            return true;
        }
        else
        {
            msjLoginResult.SetText("La contraseña debe ser mayor que cinco caracteres");
            return false;
        }
    }

    public bool IsFormatPasswordCorrect(TMP_InputField password, TMP_InputField rePassword, TextMeshProUGUI msjLoginResult)
    {
        if(password.text.Length > 5 && rePassword.text.Length > 5) {
        
            if(!password.text.Equals(rePassword.text))
            {
                msjLoginResult.SetText("Las contraseñas no coinciden");
                return false;
            }
            return true;
        } else
        {
            msjLoginResult.SetText("La contraseña debe ser mayor que cinco caracteres");
            return false;
        }
    }

}
