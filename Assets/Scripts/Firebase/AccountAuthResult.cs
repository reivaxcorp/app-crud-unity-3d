using UnityEngine;


public enum AuthType
{
    LOGOUT, LOGIN_SUCCESS, LOGIN_FAILURE, LOGIN_CANCEL,
    CREATE_ACCOUNT_SUCCESS, CREATE_ACCOUNT_FAILURE, CREATE_ACCOUNT_CANCEL,
    SEND_MAIL_VERIFICATION_SUCCESS, SEND_MAIL_VERIFICATION_CANCEL, SEND_MAIL_VERIFICATION_FAILURE
}

/// <summary>
/// Get result when we do auth actions.
/// </summary>
public class AccountAuthResult
{

    public string Message { get => _message; private set => _message = value; }
    public AuthType AuthType { get => _authType; private set => _authType = value; }

    private string _message;
    private AuthType _authType;

    public AccountAuthResult(AuthType authType, string message)
    {
        Message = message;
        AuthType = authType;
    }
}