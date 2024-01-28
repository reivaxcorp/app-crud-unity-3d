using Firebase.Extensions;
using Firebase.Storage;
using System.Threading.Tasks;
using UnityEngine;

public class UploadFileRemote
{
 
    public UploadFileRemote(byte[] fileBytes, string folderUserUid, string fileName, IResultCrud iResult)
    {
        //string FAKE_UID = "C8prXOcdOPRj4DGkfFDbXIEqRJ42";
        // Debug.Log("file name " + fileName + " folderUserUid " + folderUserUid);
        FirebaseStorage firebaseStorage = FirebaseSDK.GetInstance().firebaseStorage;

        if (firebaseStorage != null)
        {
            StorageReference storageRef =
                firebaseStorage.GetReferenceFromUrl("gs://appcrudunity3d.appspot.com");
            StorageReference userRef = storageRef.Child("users").Child("images").Child(folderUserUid).Child(fileName);

            // Upload the file to the path "images/rivers.jpg"
            // we need call ContinueWithOnMainThread because need update Ui with our callback.
            userRef.PutBytesAsync(fileBytes)
                .ContinueWithOnMainThread((Task<StorageMetadata> task) => {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        Debug.Log(task.Exception.ToString());
                        iResult.SetResultCrudUi(false, "task.IsFaulted or task.IsCanceled");
                        // Uh-oh, an error occurred!
                    }
                    else
                    {
                        // Metadata contains file metadata such as size, content-type, and md5hash.
                        StorageMetadata metadata = task.Result;
                        string md5Hash = metadata.Md5Hash;
                        Debug.Log("Finished uploading..." + metadata.Path);
                        Debug.Log("md5 hash = " + md5Hash);
                        iResult.SetResultCrudUi(true, "Finished uploading...");
                        iResult.ResultPathReference(metadata.Path);
                    }
                });
        }
        else
        {
            Debug.LogWarning("FirebaseStorage is null");
            iResult.SetResultCrudUi(false, "FirebaseStorage no exists");
        }
    }

}
