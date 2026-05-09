using Firebase.Auth;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.OurUtils;
using UnityEngine;

public class PlayGamesManager : MonoBehaviour
{
    private NetworkManager networkManager;
    private MyApplication myApplication;
    private bool isFirebaseAvariable;
    private FirebaseAuth auth;

    private void Awake()
    {
        isFirebaseAvariable = false;
        networkManager = GetComponent<NetworkManager>();
        myApplication = GetComponent<MyApplication>();
    }

    private void Update()
    {
        if(!isFirebaseAvariable && myApplication.IsFirebaseReady)
        {
            isFirebaseAvariable = true;
            PlayGamesPlatform.Activate();
            auth = FirebaseAuth.DefaultInstance;
            Login();
        }
    }


    void Login()
    {
        if (networkManager != null && networkManager.HasInternet())
        {
            PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
        }
    }

    internal void ProcessAuthentication(SignInStatus status)
    {
        // Forzamos que esto corra en el hilo de Unity para ver los Logs
        PlayGamesHelperObject.RunOnGameThread(() => {
            Debug.Log("Código de estado de Play Games: " + (int)status + " - " + status.ToString());

            if (status == SignInStatus.Success)
            {
                PlayGamesPlatform.Instance.RequestServerSideAccess(false, (authCode) =>
                {
                    // Nuevamente, aseguramos el hilo para el chequeo del código
                    PlayGamesHelperObject.RunOnGameThread(() => {
                        if (!string.IsNullOrEmpty(authCode))
                        {
                            Debug.Log("AuthCode recibido correctamente: " + authCode.Substring(0, 5) + "...");
                            ExchangeAuthCodeWithFirebase(authCode);
                        }
                        else
                        {
                            Debug.LogError("No se pudo obtener el AuthCode de Google (Vino vacío o nulo).");
                        }
                    });
                });
            }
            else
            {
                Debug.LogWarning("Fallo el login de Google Play Games. Status: " + status);
            }
        });
    }

    private void ExchangeAuthCodeWithFirebase(string authCode)
    {
        Credential credential = PlayGamesAuthProvider.GetCredential(authCode);

        auth.SignInAndRetrieveDataWithCredentialAsync(credential).ContinueWith(task => {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log("Firebase autenticado vía Google Play Games.");
                // Aquí puedes cargar el nombre del usuario
                string name = PlayGamesPlatform.Instance.GetUserDisplayName();
                Debug.Log("Bienvenido: " + name);
            }
            else
            {
                Debug.LogError("Error al vincular con Firebase: " + task.Exception);
            }
        });
    }
}