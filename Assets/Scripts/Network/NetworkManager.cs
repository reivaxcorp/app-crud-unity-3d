/*********************************************************************************
 * Nombre del Archivo:     NetworkManager.cs 
 * Descripción:            Verificamos que tangamos conexion a internet para poder añadir nuevos
 *                         ítems en la aplicación, en caso de que no hay internet, se deshabilitaran los 
 *                         botónes correspondientes a añadir ítem en el script ManageItems.cs.
 *                         
 * Autor:                  Javier
 * Organización:           ReivaxCorp.
 *
 * Derechos de Autor (c) [2024] ReivaxCorp
 * 
 * Permiso es otorgado, sin cargo, para que cualquier persona obtenga una copia
 * de este software y de los archivos de documentación asociados (el "Software"),
 * para tratar en el Software sin restricción, incluyendo sin limitación los
 * derechos para usar, copiar, modificar, fusionar, publicar, distribuir,
 * sublicenciar, y/o vender copias del Software, y para permitir a las personas a
 * quienes pertenezca el Software, sujeto a las siguientes condiciones:
 *
 * El aviso de derechos de autor anterior y este aviso de permiso se incluirán en
 * todas las copias o partes sustanciales del Software.
 *
 * EL SOFTWARE SE PROPORCIONA "TAL CUAL", SIN GARANTÍA DE NINGÚN TIPO, EXPRESA O
 * IMPLÍCITA, INCLUYENDO PERO NO LIMITADO A LAS GARANTÍAS DE COMERCIABILIDAD,
 * IDONEIDAD PARA UN PROPÓSITO PARTICULAR Y NO INFRACCIÓN. EN NINGÚN CASO LOS
 * AUTORES O TITULARES DE DERECHOS DE AUTOR SERÁN RESPONSABLES DE CUALQUIER
 * RECLAMACIÓN, DAÑO O OTRA RESPONSABILIDAD, YA SEA EN UNA ACCIÓN DE CONTRATO, AGRAVIO
 * O DE OTRO MODO, DERIVADAS DE, FUERA DE O EN CONEXIÓN CON EL SOFTWARE O EL USO U OTROS
 * TRATOS EN EL SOFTWARE.
 *********************************************************************************/

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
                // Debug.Log("No hay conexión a Internet.");
                handleInternetAvariableResult?.Invoke(false);
                break;
            case NetworkReachability.ReachableViaCarrierDataNetwork:
            case NetworkReachability.ReachableViaLocalAreaNetwork:
                // Debug.Log("Conexión a Internet disponible.");
                handleInternetAvariableResult?.Invoke(true);
                break;
        }
    }
}
