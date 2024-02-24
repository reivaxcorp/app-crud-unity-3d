using Firebase.Extensions;
using System.Threading.Tasks;
using UnityEngine;

public class ManageTextureRemote
{
    private string _storageUrl = "gs://appcrudunity3d.appspot.com/users/images/"; // Reemplaza con la URI pública de tu imagen.
                                                                               // public Material materialToUpdate; // El material que se actualizará con la imagen descargada.
    public ManageTextureRemote(string imageName)
    {
        _storageUrl += FirebaseSDK.GetInstance().auth.CurrentUser.UserId + "/imageItems/" + imageName + ".png";
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
    public async Task<bool> DeleteImageRemote()
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
