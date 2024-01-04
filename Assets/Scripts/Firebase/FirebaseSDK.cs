using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Storage;
using System.Threading.Tasks;
using UnityEngine;

public class FirebaseSDK
{
    private static FirebaseSDK instance;

    private FirebaseApp _app;
    public FirebaseApp app
    {
        private set { _app = value; }
        get { return _app; }
    }

    private FirebaseDatabase _db;
    public FirebaseDatabase db
    {
        private set { _db = value; }
        get { return _db; }
    }

    private FirebaseStorage _firebaseStorage;
    public FirebaseStorage firebaseStorage
    {
        private set { _firebaseStorage = value; }
        get { return _firebaseStorage; }
    }

    private FirebaseAuth _auth;
    public FirebaseAuth auth
    {
        private set { _auth = value; }
        get { return _auth; }
    }

    private FirebaseUser _user;
    public FirebaseUser user
    {
        private set { _user = value; }
        get { return _user; }
    }

    private bool _isFirebaseReady;
    public bool isFirebaseReady
    {
        private set { _isFirebaseReady = value; }
        get { return _isFirebaseReady; }
    }

    public async Task<bool> InitFirebaseAsync()
    {

        await FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;

            if (dependencyStatus == DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                this.app = FirebaseApp.DefaultInstance;
                // Set a flag here to indicate whether Firebase is ready to use by your app.

                this.db = FirebaseDatabase.GetInstance(app);

                this.firebaseStorage = FirebaseStorage.DefaultInstance;

                this.auth = FirebaseAuth.DefaultInstance;
                this.auth.StateChanged += AuthStateChanged;
                AuthStateChanged(this, null);
                this.user = auth.CurrentUser;

                // Accede luego a una colección y lee documentos
                // ....
                // Debug.Log("Firebase initialized");
                isFirebaseReady = true;
            }
            else
            {
                Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
                isFirebaseReady = false;
            }
        });

        return isFirebaseReady;
    }

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
                /* displayName = user.DisplayName ?? "";
                 emailAddress = user.Email ?? "";
                 photoUrl = user.PhotoUrl ?? "";*/
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
            //  Debug.Log("Firebase SignOut");
        }
    }

    private void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }
}
