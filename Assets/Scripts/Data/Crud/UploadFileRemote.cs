using Firebase.Storage;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class UploadFileRemote
{
    public UploadFileRemote(byte[] fileBytes, string folderUserUid, string fileName, IResultCrud iResult)
    {

        Debug.Log("file name " + fileName + " folderUserUid " + folderUserUid);
        FirebaseStorage firebaseStorage = FirebaseSDK.GetInstance().firebaseStorage;

        if (firebaseStorage != null)
        {
            StorageReference storageRef =
                firebaseStorage.GetReferenceFromUrl("gs://appcrudunity3d.appspot.com");
            StorageReference userRef = storageRef.Child("users/"+folderUserUid+"/"+fileName);
            StorageReference userFolder = userRef.Child(folderUserUid);
            StorageReference fileNameItem = userFolder.Child(fileName);

            // Create file metadata including the content type
            var newMetadata = new MetadataChange();
            newMetadata.ContentType = "image/png";

            // Upload the file to the path "images/rivers.jpg"
            userRef.PutBytesAsync(fileBytes, newMetadata, null,
                CancellationToken.None)
                .ContinueWith((Task<StorageMetadata> task) => {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        Debug.Log(task.Exception.ToString());
                        iResult.SetResultCrud(false, "task.IsFaulted or task.IsCanceled");
                        // Uh-oh, an error occurred!
                    }
                    else
                    {
                        // Metadata contains file metadata such as size, content-type, and md5hash.
                        StorageMetadata metadata = task.Result;
                        string md5Hash = metadata.Md5Hash;
                        Debug.Log("Finished uploading...");
                        Debug.Log("md5 hash = " + md5Hash);
                        iResult.SetResultCrud(true, "Finished uploading...");
                    }
                });

            // Upload the file to the path "user/folderUserUid/myimages.jpg"
          /* fileNameItem.PutFileAsync(fileName)
            .ContinueWith((Task<StorageMetadata> task) =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log(task.Exception.ToString());
                    iResult.SetResultCrud(false, "task.IsFaulted or task.IsCanceled");
                    // Uh-oh, an error occurred!
                }
                else
                {
                    // Metadata contains file metadata such as size, content-type, and download URL.
                    StorageMetadata metadata = task.Result;
                    string md5Hash = metadata.Md5Hash;
                    Debug.Log("Finished uploading...");
                    Debug.Log("md5 hash = " + md5Hash);
                    iResult.SetResultCrud(true, "Finished uploading...");
                }
            });*/
        }
        else
        {
            Debug.LogWarning("FirebaseStorage is null");
            iResult.SetResultCrud(false, "FirebaseStorage no exists");
        }
    }
}
