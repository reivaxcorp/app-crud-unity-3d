/*********************************************************************************
 * Descripción:            Prepara el Interstitial Ad, cada vez que agregamos o 
 *                         actualizamos un ítem.
 *                         Sin cambios significativos, se mantuvieron los comentarios en el Ingles,
 *                         y se agregaron info en Español. Se llama desde el menu, "MenuCrud.cs" en el 
 *                         método ShowInterstitialAd(). 
 *********************************************************************************/

using UnityEngine;
using UnityEngine.Advertisements;

public class InterstitialAd : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] string _androidAdUnitId = "Interstitial_Android";
    string _adUnitId;

    void Awake()
    {
        // Get the Ad Unit ID for the current platform:
        _adUnitId = _androidAdUnitId;
    }

    // Primero cargamos el Ad, luego lo mostrarmos a travez del callback, si se cargo.

    // Load content to the Ad Unit:
    public void LoadAd()
    {
        AdsInitializer adsInitializer = GetComponent<AdsInitializer>();

        if (adsInitializer == null)
        {
            Debug.LogWarning("ads es null, colocalo en el inspector");
            return;
        }

        if (adsInitializer.isInitializacionComplete)
        {
            // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
            Debug.Log("Loading Ad: " + _adUnitId);
            Advertisement.Load(_adUnitId, this);
        }
        else
        {
            Debug.LogWarning("Ads no se inicializó");
        }
    }

    // Show the loaded content in the Ad Unit:
    public void ShowAd()
    {
        // Note that if the ad content wasn't previously loaded, this method will fail
        Debug.Log("Showing Ad: " + _adUnitId);
        Advertisement.Show(_adUnitId, this);
    }

    // Implement Load Listener and Show Listener interface methods: 
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("Interstitial loaded");

        ShowAd(); // una vez cargado podemos mostrar el ad interstitial

        // Optionally execute code if the Ad Unit successfully loads content.
    }

    public void OnUnityAdsFailedToLoad(string _adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit: {_adUnitId} - {error.ToString()} - {message}");
        // Optionally execute code if the Ad Unit fails to load, such as attempting to try again.
    }

    public void OnUnityAdsShowFailure(string _adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {_adUnitId}: {error.ToString()} - {message}");
        // Optionally execute code if the Ad Unit fails to show, such as loading another ad.
    }

    public void OnUnityAdsShowStart(string _adUnitId) { }
    public void OnUnityAdsShowClick(string _adUnitId) { }
    public void OnUnityAdsShowComplete(string _adUnitId, UnityAdsShowCompletionState showCompletionState) { }
}
