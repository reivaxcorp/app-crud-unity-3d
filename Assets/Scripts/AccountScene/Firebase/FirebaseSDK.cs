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
using Firebase.Extensions;
using Firebase.Storage;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class FirebaseSDK
{
    public FirebaseApp app
    {
        private set { _app = value; }
        get { return _app; }
    }
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
    public async Task<bool> InitFirebaseDependenciesAsync()
    {

        await FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;

            if (dependencyStatus == DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.

                this.app = Firebase.FirebaseApp.DefaultInstance; // PRODUCTION MODE

                // Set a flag here to indicate whether Firebase is ready to use by your app.
                this.defaultInstance = FirebaseDatabase.DefaultInstance;

                this.firebaseStorage = FirebaseStorage.DefaultInstance;

                this.auth = FirebaseAuth.DefaultInstance;
                this.auth.StateChanged += AuthStateChanged;

                isFirebaseReady = true;
                AuthStateChanged(this, null);
            }
            else
            {
                Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
                isFirebaseReady = false;
                throw new Exception(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });
       
        return isFirebaseReady;
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
            }
        }

        InitUidUserToApp(); // podemos inicializar ahora los datos principales.
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
        } else
        {
            MyApplication.repository.GetRemoteDb().SetUserUid(null);
            MyApplication.repository.GetLocalDb().SetUserUidFolder(null);
            Debug.Log("Usuario inexistente por ahora..");
        }
    }

    private void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }
}
