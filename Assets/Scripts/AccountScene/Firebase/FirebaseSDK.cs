/*********************************************************************************
 * Nombre del Archivo:     FirebaseSDK.cs
 * Descripción:            Inicializamos las dependencias de firebase, y escuchamos los cambios de 
 *                         inicio de sesión de usuario. 
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

using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Storage;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class FirebaseSDK
{
    public FirebaseDatabase defaultInstance
    {
        private set { _defaultInstance = value; }
        get { return _defaultInstance; }
    }
    public FirebaseStorage firebaseStorage
    {
        private set { _firebaseStorage = value; }
        get { return _firebaseStorage; }
    }
    public FirebaseAuth auth
    {
        private set { _auth = value; }
        get { return _auth; }
    }
    public FirebaseUser user
    {
        private set { _user = value; }
        get { return _user; }
    }
    public bool isFirebaseReady
    {
        private set { _isFirebaseReady = value; }
        get { return _isFirebaseReady; }
    }
    public FirebaseApp app
    {
        private set { _app = value; }
        get { return _app; }
    }

    private static FirebaseSDK instance;
    private FirebaseApp _app;
    private FirebaseDatabase _defaultInstance;
    private FirebaseStorage _firebaseStorage;
    private FirebaseAuth _auth;
    private FirebaseUser _user;
    private bool _isFirebaseReady;


    /// <summary>
    /// Initialize firebase dependencies. 
    /// </summary>
    /// <returns></returns>
    public async Task<bool> InicializeFirebase()
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
            Firebase.FirebaseApp.Create(options) ;

            // 3. Verificamos dependencias como siempre
            var dependencyStatus = await Firebase.FirebaseApp.CheckAndFixDependenciesAsync();
            

            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                Debug.Log("SISTEMA: Firebase cargado manualmente con éxito.");
               
                this.auth = FirebaseAuth.DefaultInstance;
                this.auth.StateChanged += AuthStateChanged;
                this.firebaseStorage = FirebaseStorage.DefaultInstance;
                this.defaultInstance = FirebaseDatabase.DefaultInstance;
                this._app = FirebaseApp.DefaultInstance;

                isFirebaseReady = true;
                return true;
            }
            else
            {
                Debug.LogError($"SISTEMA: Dependencias no disponibles: {dependencyStatus}");
                isFirebaseReady = false;
                return false;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"SISTEMA: Error en inicialización manual: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DoLogin()
    {
        try
        {
            var result = await auth.SignInAnonymouslyAsync();
            _user = result.User;

            Debug.Log($"Login Anónimo OK. User ID: {result.User.UserId}");
           
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error en login anónimo: " + e.Message);
            return false;
        }
    }

    /// <summary>
    ///   La primera vez que entre el usuario, sera null.
    ///   Una vez que inicie sesión, tendremos un usuario, y podremos setear 
    ///   las propiedades de neustras base de datos y la app, para guardar lo datos.
    /// </summary>
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null
                && auth.CurrentUser.IsValid();

            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }

            user = auth.CurrentUser;

            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
                InitUidUserToApp(); // podemos inicializar ahora los datos principales.
            }
        }
    }

    public static FirebaseSDK GetInstance()
    {
        if (instance == null)
        {
            instance = new FirebaseSDK();
        }
        return instance;
    }

    public void LogOut()
    {
        if (auth != null)
        {
            auth.SignOut();
        }
    }

    /// <summary>
    /// Inicializamos el uid del usuario en la base de datos remota y local, asi como tambien
    /// en el Texture manager, ya que necesitamos para acceder a los datos de dicho usuario logeado
    /// </summary>
    private void InitUidUserToApp()
    {
        if (user != null && MyApplication.repository != null)
        {
            MyApplication.repository.GetRemoteDb().SetUserUid(user.UserId);
            MyApplication.repository.GetLocalDb().SetUserUidFolder(user.UserId);
            SaveLocalUserData(user.UserId);
        } else
        {
            Debug.Log("Usuario inexistente por ahora..");
        }
    }

    private void SaveLocalUserData(string uidUser)
    {
        if(!string.IsNullOrEmpty(uidUser))
        {
            UserLocalData userData = new UserLocalData();
            userData.localUserUi = uidUser;

            string json = JsonUtility.ToJson(userData, true);

            string path = Path.Combine(UnityEngine.Application.persistentDataPath, "data_user.json");

            // 4. Escribimos el archivo
            File.WriteAllText(path, json);

            Debug.Log("Datos locales de usuario guardados: " + path);
        }
    }

    private void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }
}
