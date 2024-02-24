using Firebase.Extensions;
using Firebase.Storage;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class UploadFileRemote
{
    private Byte[] _fileBytes;
    private string _folderUserUid;
    private IResult _iResult;
    private string _newImageName;
    public string newImageName
    {
        private set { _newImageName = value; }
        get { return _newImageName; }
    }

    public UploadFileRemote(
        byte[] fileBytes,
        string folderUserUid,
        IResult iResult
        )
    {
        _fileBytes = fileBytes;
        _folderUserUid = folderUserUid;
        _iResult = iResult;
    }

    public async Task<bool> UploadFileFirebaseStorage()
    {
        FirebaseStorage firebaseStorage = FirebaseSDK.GetInstance().firebaseStorage;

        if (firebaseStorage != null)
        {
            string generateImageName = Guid.NewGuid().ToString();

            StorageReference storageRef = firebaseStorage.GetReferenceFromUrl("gs://appcrudunity3d.appspot.com");
            StorageReference userRef = storageRef
                .Child("users")
                .Child(_folderUserUid)
                .Child("imageItems")
                .Child(generateImageName + ".png");

            // Crear metadatos de archivo incluyendo el tipo de contenido
            var newMetadata = new MetadataChange();
            newMetadata.ContentType = "image/png";

            // Debemos continuar en el hilo principal, ya que debemos actualizar la UI, por eso usamos
            // ContinueWithOnMainThread.
            await userRef.PutBytesAsync(_fileBytes, newMetadata, null, CancellationToken.None)
                .ContinueWithOnMainThread((Task<StorageMetadata> task) =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        Debug.Log(task.Exception.ToString());
                        _iResult.SetResultCrudUi(EResultMenuAction.Failed, "Tarea fallida ó cancelada");
                    }
                    else
                    {
                        // Los metadatos contienen información del archivo como tamaño, tipo de contenido y hash md5.
                        StorageMetadata metadata = task.Result;
                        string md5Hash = metadata.Md5Hash;
                        Debug.Log("¡Subida finalizada!" + metadata.Path);
                        Debug.Log("Hash MD5 = " + md5Hash);
                        newImageName = generateImageName;
                        _iResult.SetResultCrudUi(EResultMenuAction.Success, "¡Archivo subido correctamente!");
                    }
                });
            return true; // Se ha iniciado correctamente la operación de subida
        }
        else
        {
            Debug.LogWarning("FirebaseStorage es null");
            _iResult.SetResultCrudUi(EResultMenuAction.Failed, "Error en FirebaseStorage \n(Recargando dependencias..)");
            return false; // No se ha podido iniciar la operación de subida
        }
    }
}
