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
                    return "El correo electronico ya se esta usando en otra cuenta";
                }
                if (firebaseException.ErrorCode == (int)Firebase.Auth.AuthError.WrongPassword)
                {
                    return "Contraseña incorrecta";
                }
                if (firebaseException.ErrorCode == (int)Firebase.Auth.AuthError.UserNotFound)
                {
                    return "Cuenta no encontrada";
                }
                if (firebaseException.ErrorCode == (int)Firebase.Auth.AuthError.UserMismatch)
                {
                    return "Usuario no coincidente";
                }
                if (firebaseException.ErrorCode == (int)Firebase.Auth.AuthError.UserDisabled)
                {
                    return "Usuario deshabilitado!";
                }
                if (firebaseException.ErrorCode == (int)Firebase.Auth.AuthError.InvalidCredential)
                {
                    return "Credenciales invalidas!";
                }
                if (firebaseException.ErrorCode == (int)Firebase.Auth.AuthError.InvalidAppCredential)
                {
                    return "Credenciales de aplicación invalidas!";
                }
                if (firebaseException.ErrorCode == (int)Firebase.Auth.AuthError.RejectedCredential)
                {
                    return "Credencial rechazada!";
                }
            }
        }

        Debug.LogError("Encontró un error: " + task.Exception);
        return "Encontró un error";
    }
}
