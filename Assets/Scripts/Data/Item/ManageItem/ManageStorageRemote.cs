using Firebase.Extensions;
using Firebase.Storage;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Lectura,subida y borrado de imágenes de firebase storage
/// </summary>
public class ManageStorageRemote
{
    private string _storageUrl = "gs://appcrudunity3d.appspot.com/users/"; // Reemplaza con la URI pública de tu imagen.
    private string _folderUserUid;
    private string _generateImageName;
    private byte[] _fileBytes;

    public ManageStorageRemote(string generateImageName, string folderUserUid, byte[] fileBytes)
    {
        _generateImageName = generateImageName;
        _folderUserUid = folderUserUid;
        _fileBytes = fileBytes;
    }

    public ManageStorageRemote(string imageName)
    {
        _storageUrl += FirebaseSDK.GetInstance().auth.CurrentUser.UserId + "/imageItems/" + imageName + ".png";
    }

    public async Task<bool> UploadFileFirebaseStorage()
    {
        FirebaseStorage firebaseStorage = FirebaseSDK.GetInstance().firebaseStorage;

        bool result = false;

        if (firebaseStorage != null)
        {

            if (_generateImageName != null)
            {

                StorageReference storageRef = firebaseStorage.GetReferenceFromUrl("gs://appcrudunity3d.appspot.com");
                StorageReference userRef = storageRef
                    .Child("users")
                    .Child(_folderUserUid)
                    .Child("imageItems")
                    .Child(_generateImageName + ".png");

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
                            result = false;
                        }
                        else
                        {
                            // Los metadatos contienen información del archivo como tamaño, tipo de contenido y hash md5.
                            StorageMetadata metadata = task.Result;
                            string md5Hash = metadata.Md5Hash;
                            Debug.Log("¡Subida finalizada!" + metadata.Path);
                            Debug.Log("Hash MD5 = " + md5Hash);
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
            return result; // No se ha podido iniciar la operación de subida
        }
    }

    public async Task<Texture2D> DownloadImage()
    {
        TaskCompletionSource<Texture2D> initializationTask = new TaskCompletionSource<Texture2D>();

        // Parsea la URL de almacenamiento para obtener la referencia a la imagen.
        var storageReference = FirebaseSDK.GetInstance().firebaseStorage.GetReferenceFromUrl(_storageUrl);
        // Descarga el archivo.
        await storageReference.GetBytesAsync(long.MaxValue).ContinueWithOnMainThread(task2 =>
         {
             if (task2.IsFaulted || task2.IsCanceled)
             {
                 Debug.Log("La imagén no existe " + _storageUrl);   
                 Debug.LogWarning("Error al descargar la imagen: " + task2.Exception);
                 initializationTask.SetResult(new Texture2D(1, 1));
             }
             else
             {
                 // Obtiene los bytes de la imagen descargada.
                 byte[] fileContents = task2.Result;

                 // Crea una textura desde los bytes de la imagen.
                 Texture2D texture = new Texture2D(1, 1);
                 bool loadImage = texture.LoadImage(fileContents);

                 if (loadImage)
                 {
                     initializationTask.SetResult(texture);
                 }
                 else
                 {
                     initializationTask.SetResult(new Texture2D(1, 1));
                     Debug.LogError("Error al cargar la textura desde los bytes.");
                 }
             }
         });

       return await initializationTask.Task;
    }

    /// <summary>
    /// Para actualizar y borrar, necesitamos borrar la imagén anterior.
    /// </summary>
    /// <param name="filePath"></param>
    public async Task<bool> DeleteImageRemote(IResult resultUi)
    {
        Debug.Log("Imagen remota a eliminar: " + _storageUrl);

        var storageReference = FirebaseSDK.GetInstance().firebaseStorage.GetReferenceFromUrl(_storageUrl);

        bool deleteSuccess = false;

        await storageReference.DeleteAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                Debug.Log("Archivo remoto borrado correctamente.");
                deleteSuccess = true;
            }
            else
            {
                Debug.LogWarning("Archivo remoto anterior no encontrado.");
                deleteSuccess = false;
            }
        });

        return deleteSuccess;
    }
}
