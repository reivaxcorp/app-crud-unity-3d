using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

public class PlayGamesManager : MonoBehaviour
{
    void Start()
    {
        PlayGamesPlatform.Activate();
        Login();
    }

    void Login()
    {
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    internal void ProcessAuthentication(SignInStatus status)
    {
        Debug.Log("Código de estado de Play Games: " + (int)status + " - " + status.ToString());

        if (status == SignInStatus.Success)
        {
            Debug.Log("Logueado en Google Play Games con éxito");
            // Aquí podrías pasar el token a Firebase para unificar cuentas
        }
        else
        {
            Debug.LogWarning("Fallo el login de Google Play Games");
        }
    }
}