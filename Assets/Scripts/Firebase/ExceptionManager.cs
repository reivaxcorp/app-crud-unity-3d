using System;
using System.Threading.Tasks;
using UnityEngine;

public class ExceptionManager 
{
    /// <summary>
    /// Manage firebase Exception
    /// </summary>
    /// <param name="task"></param>
    public string ManageExceptionForm(Task task)
    {
        AggregateException exception = task.Exception.Flatten();

        foreach (Exception innerException in exception.InnerExceptions)
        {
            Firebase.FirebaseException firebaseException = innerException as Firebase.FirebaseException;

            if (firebaseException != null)
            {
                // Check if the email address is already in use
                if (firebaseException.ErrorCode == (int)Firebase.Auth.AuthError.EmailAlreadyInUse)
                {
                    Debug.LogError("Email is already in use.");
                    return "El correo electronico ya se esta usando en otra cuenta";
                }

                if (firebaseException.ErrorCode == (int)Firebase.Auth.AuthError.WrongPassword)
                {
                    Debug.LogError("Wrong Password.");
                    return "Contraseña incorrecta";
                }

                if (firebaseException.ErrorCode == (int)Firebase.Auth.AuthError.UserNotFound)
                {
                    Debug.LogError("User not Found.");
                    return "Cuenta no encontrada";
                }

                if (firebaseException.ErrorCode == (int)Firebase.Auth.AuthError.UserMismatch)
                {
                    Debug.LogError("User mis match.");
                    return "Usuario no coincidente";
                }
                if (firebaseException.ErrorCode == (int)Firebase.Auth.AuthError.UserDisabled)
                {
                    Debug.LogError("User mis disable.");
                    return "Usuario deshabilitado!";
                }
            }
        }

        Debug.LogError("Encountered an error: " + task.Exception);
        return "Encountered an error";
    }
}
