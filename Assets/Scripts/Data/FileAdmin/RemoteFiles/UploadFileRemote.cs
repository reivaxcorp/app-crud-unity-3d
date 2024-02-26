using Firebase.Extensions;
using Firebase.Storage;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class UploadFileRemote
{
    public string generateImageName
    {
        private set { _generateImageName = value; }
        get { return _generateImageName; }
    }

    private string _generateImageName;
    private Byte[] _fileBytes;
    private string _folderUserUid;
    private IResult _iResult;

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

    public void SetImageNameGenerate(string imageName)
    {
        generateImageName = imageName;
    }

    public async Task<bool> UploadFileFirebaseStorage()
    {
        FirebaseStorage firebaseStorage = FirebaseSDK.GetInstance().firebaseStorage;
       
        bool result = false;

        if (firebaseStorage != null)
        {

            if (generateImageName != null)
            {

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
                            _iResult.SetResultCrudUi(EResultMenuAction.FileFailedUploated, "Tarea fallida ó cancelada");
                            result = false;
                        }
                        else
                        {
                            // Los metadatos contienen información del archivo como tamaño, tipo de contenido y hash md5.
                            StorageMetadata metadata = task.Result;
                            string md5Hash = metadata.Md5Hash;
                            Debug.Log("¡Subida finalizada!" + metadata.Path);
                            Debug.Log("Hash MD5 = " + md5Hash);
                            _iResult.SetResultCrudUi(EResultMenuAction.FileSuccessUploated, "¡Archivo subido correctamente!");
                            result = true; // Se ha iniciado correctamente la operación de subida
                        }
                    });
            }
            else
            {
                Debug.LogWarning("generateImageName es Null");
            }

            return result; 
        }
        else
        {
            Debug.LogWarning("FirebaseStorage es null");
            _iResult.SetResultCrudUi(EResultMenuAction.FileFailedUploated, "Error en FirebaseStorage \n(Recargando dependencias..)");
            return result; // No se ha podido iniciar la operación de subida
        }
    }
}
