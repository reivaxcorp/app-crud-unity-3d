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
        //Debug.Log(Application.persistentDataPath);
    }

    private async Task<MyRepository> CreateRepositoryAsync()
    {

        FirebaseSDK firebaseSdk = FirebaseSDK.GetInstance();
        // FIRST wait to initialize Sdk Firebase
        IsFirebaseReady = await firebaseSdk.InicializeFirebase();

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
}
