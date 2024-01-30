using Firebase.Extensions;
using Firebase.Storage;
using System.Threading.Tasks;
using UnityEngine;

public class UploadFileRemote
{
 
    public UploadFileRemote(byte[] fileBytes, string folderUserUid, string fileName, IResultUi iResultUi, IResultFile iFileResult)
    {
        // string FAKE_UID = "C8prXOcdOPRj4DGkfFDbXIEqRJ42";

        FirebaseStorage firebaseStorage = FirebaseSDK.GetInstance().firebaseStorage;

        if (firebaseStorage != null)
        {
            StorageReference storageRef =
                firebaseStorage.GetReferenceFromUrl("gs://appcrudunity3d.appspot.com");
            StorageReference userRef = storageRef.Child("users").Child("images").Child(folderUserUid).Child(fileName);

            // Debemos continuar en el hilo principal, ya que debemos actualizar la UI, por eso usamos
            // ContinueWithOnMainThread.
            userRef.PutBytesAsync(fileBytes)
                .ContinueWithOnMainThread((Task<StorageMetadata> task) => {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        Debug.Log(task.Exception.ToString());
                        iResultUi.SetResultCrudUi(false, "Tarea fallida ó cancelada");
                        // Uh-oh, an error occurred!
                    }
                    else
                    {
                        // Metadata contains file metadata such as size, content-type, and md5hash.
                        StorageMetadata metadata = task.Result;
                        string md5Hash = metadata.Md5Hash;
                        Debug.Log("Finished uploading..." + metadata.Path);
                        Debug.Log("md5 hash = " + md5Hash);
                        iResultUi.SetResultCrudUi(true, "Archivo subido!");
                        iFileResult.FileUploaded(true, metadata.Path);
                    }
                });
        }
        else
        {
            Debug.LogWarning("FirebaseStorage es null");
            iResultUi.SetResultCrudUi(false, "FirebaseStorage no existe");
        }
    }
}
