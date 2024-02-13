using UnityEngine;


public class NetworkManager : MonoBehaviour
{
    private NetworkReachability previousReachability;
    public delegate void OnInternetAvariable(bool isInternetAvariable);
    public event OnInternetAvariable handleInternetAvariableResult;
    private bool startListeningInternet;

    private void Start()
    {
        startListeningInternet = false;
    }

    public void ListeningInternetAvariable()
    {
        startListeningInternet = true;
        // Guardar el estado de la conectividad a Internet al inicio
        previousReachability = Application.internetReachability;

        // Llamar al método para manejar la conectividad
        HandleInternetReachability();
    }

    void Update()
    {
        if (startListeningInternet)
        {
            // Comprobar si ha cambiado el estado de la conectividad a Internet
            if (Application.internetReachability != previousReachability)
            {
                // Actualizar el estado anterior de la conectividad
                previousReachability = Application.internetReachability;

                // Llamar al método para manejar la conectividad
                HandleInternetReachability();
            }
        }
    }

    void HandleInternetReachability()
    {
        // Obtener el estado actual de la conectividad a Internet
        NetworkReachability reachability = Application.internetReachability;

        // Comprobar el estado y actuar en consecuencia
        switch (reachability)
        {
            case NetworkReachability.NotReachable:
                Debug.Log("No hay conexión a Internet.");
                handleInternetAvariableResult?.Invoke(false);
                break;
            case NetworkReachability.ReachableViaCarrierDataNetwork:
            case NetworkReachability.ReachableViaLocalAreaNetwork:
                Debug.Log("Conexión a Internet disponible.");
                handleInternetAvariableResult?.Invoke(true);
                break;
        }
    }
}
