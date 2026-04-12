/*********************************************************************************
 * Nombre del Archivo:     MyApplication.cs 
 * Descripción:            Inicializamos nuestro repositorio, los servicios de firebase
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

using System;
using System.Threading.Tasks;
using UnityEngine;

public class MyApplication : MonoBehaviour
{
    public static MyRepository repository
    {
        private set { _repository = value; }
        get { return _repository; }
    }
    private static MyRepository _repository;

    // Propiedad para que el Manager de Auth sepa cuándo arrancar
    public static bool IsFirebaseReady { get; private set; } = false;

    private async void Start()
    {
       if(repository == null)
        {
            await CreateRepositoryAsync();
        }
    }

    private async Task<MyRepository> CreateRepositoryAsync()
    {

        Debug.Log("SISTEMA: Firebase y Repositorio listos.");
        // FIRST wait to initialize Sdk Firebase
        IsFirebaseReady = await InicializeFirebase();

        if (IsFirebaseReady)
        {
            RemoteDb remoteDb = new RemoteDb();
            LocalDb localDb = new LocalDb();
            repository = new MyRepository(localDb, remoteDb);

            Debug.Log("SISTEMA: Firebase y Repositorio listos.");
            return repository;
        }
        else
        {
            Debug.LogError("SISTEMA: Falló la inicialización crítica de Firebase.");
            return null;
        }
    }

    // we wait firebase start
    /* private async Task<bool> InicializeFirebase()
     {
         FirebaseSDK firebaseSdk = FirebaseSDK.GetInstance();

         try
         {
             // Este método DEBE llamar internamente a FirebaseApp.CheckAndFixDependenciesAsync()
             return await firebaseSdk.InitFirebaseDependenciesAsync();
         }
         catch (Exception ex)
         {
             Debug.LogError($"Error fatal en InitFirebase: {ex.Message}");
             return false;
         }
     }*/

    private async Task<bool> InicializeFirebase()
    {
        try
        {
            // 1. Configuramos las opciones manualmente con los datos de tu XML
            Firebase.AppOptions options = new Firebase.AppOptions
            {
                ApiKey = "AIzaSyAeeNWpjVs2vhpTJZCbtp7iZkjHzeGQMjE",
                AppId = "1:88826351788:android:1dacb2cf5eeb16d054cf09",
                ProjectId = "appcrudunity3d",
                DatabaseUrl = new System.Uri("https://appcrudunity3d-default-rtdb.firebaseio.com"),
                StorageBucket = "appcrudunity3d.appspot.com",
                MessageSenderId = "88826351788"
            };

            // 2. Intentamos crear la App con estas opciones
            Firebase.FirebaseApp.Create(options);

            // 3. Verificamos dependencias como siempre
            var dependencyStatus = await Firebase.FirebaseApp.CheckAndFixDependenciesAsync();

            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                Debug.Log("SISTEMA: Firebase cargado manualmente con éxito.");
                return true;
            }
            else
            {
                Debug.LogError($"SISTEMA: Dependencias no disponibles: {dependencyStatus}");
                return false;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"SISTEMA: Error en inicialización manual: {ex.Message}");
            return false;
        }
    }

}
