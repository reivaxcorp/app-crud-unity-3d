using Firebase.Auth;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirebaseAuthManager : MonoBehaviour
{
    private bool initialized = false;

    private void Start()
    {
        _ =  EnsureAuthReady();
    }

    private async Task EnsureAuthReady()
    {
        if (initialized) return;

        // Esperamos a que MyApplication termine su trabajo
        while (MyApplication.repository == null)
        {
            await Task.Delay(100); // Pequeña espera para no bloquear el hilo
        }


        await FirebaseSDK.GetInstance().DoLogin();
        initialized = true;
        Debug.Log("FirebaseAuthManager: Conectado a la instancia validada por FirebaseSDK con anonimo.");
    }


   
}




