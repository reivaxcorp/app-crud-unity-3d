using UnityEngine;


/// <summary>
/// Get result when we do auth actions.
/// </summary>
public class AccountAuthResult
{

    public string Message { get => _message; private set => _message = value; }
    public Color MessageColor { get => _messageColor; private set => _messageColor = value; }
    public bool IsSuccessed { get => _isSuccessd; private set => _isSuccessd = value; }
   
    private string _message;
    private Color _messageColor;
    private bool _isSuccessd;

    public AccountAuthResult(string message, Color messageColor, bool isSuccessed)
    {
        Message = message;
        MessageColor = messageColor;
        IsSuccessed = isSuccessed;
    }
}