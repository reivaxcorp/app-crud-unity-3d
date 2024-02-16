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
    private string _fileName;
    private IResult _iResult;
    private IResultFile _iFileResult;
    private int _tryGetDependencies;

    public UploadFileRemote(
        byte[] fileBytes,
        string folderUserUid,
        string fileName,
        IResult iResult,
        IResultFile iFileResult
        )
    {
        _fileBytes = fileBytes;
        _folderUserUid = folderUserUid;
        _fileName = fileName;
        _iResult = iResult;
        _iFileResult = iFileResult;
        _tryGetDependencies = 2;
    }

    public void UpoloadFileFirebaeStorage()
    {
        if (_tryGetDependencies == 0)
            return;

            FirebaseStorage firebaseStorage = FirebaseSDK.GetInstance().firebaseStorage;

        if (firebaseStorage != null)
        {
            string imageName = Guid.NewGuid().ToString();

            StorageReference storageRef =
                firebaseStorage.GetReferenceFromUrl("gs://appcrudunity3d.appspot.com");
            StorageReference userRef = storageRef
                .Child("users")
                .Child("images")
                .Child(_folderUserUid).Child(imageName + ".png");

            // Create file metadata including the content type
            var newMetadata = new MetadataChange();
            newMetadata.ContentType = "image/png";

            // Debemos continuar en el hilo principal, ya que debemos actualizar la UI, por eso usamos
            // ContinueWithOnMainThread.
            userRef.PutBytesAsync(_fileBytes, newMetadata, null, CancellationToken.None)
                .ContinueWithOnMainThread((Task<StorageMetadata> task) =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        Debug.Log(task.Exception.ToString());
                        _iResult.SetResultCrudUi(false, "Tarea fallida ó cancelada");
                        // Uh-oh, an error occurred!
                    }
                    else
                    {
                        // Metadata contains file metadata such as size, content-type, and md5hash.
                        StorageMetadata metadata = task.Result;
                        string md5Hash = metadata.Md5Hash;
                        Debug.Log("Finished uploading..." + metadata.Path);
                        Debug.Log("md5 hash = " + md5Hash);
                        _iResult.SetResultCrudUi(true, "Archivo subido correctamente!");
                        _iFileResult.FileUploaded(true, imageName);
                    }
                });
        }
        else
        {

            Debug.LogWarning("FirebaseStorage es null");
            _iResult.SetResultCrudUi(false, "Error en FirebaseStorage \n(Recargando dependencias..)");
            RefreshDependencies();
         }
    }

    private async void RefreshDependencies()
    {
        await FirebaseSDK.GetInstance().InitFirebaseDependenciesAsync();
        _tryGetDependencies--;
        UpoloadFileFirebaeStorage();
    }
}
