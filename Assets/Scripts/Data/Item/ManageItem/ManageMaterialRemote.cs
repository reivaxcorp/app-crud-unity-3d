using Firebase.Extensions;
using System.Threading.Tasks;
using UnityEngine;

public class ManageMaterialRemote
{
    private string _storageUrl = "gs://appcrudunity3d.appspot.com/users/images/"; // Reemplaza con la URI pública de tu imagen.
                                                                               // public Material materialToUpdate; // El material que se actualizará con la imagen descargada.
    public ManageMaterialRemote(string imageName)
    {
        _storageUrl += FirebaseSDK.GetInstance().user.UserId + "/" + imageName;
        //Debug.Log("storage " + _storageUrl);
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
                 Debug.LogError("Error al descargar la imagen: " + task2.Exception);
                 initializationTask.SetResult(new Texture2D(1, 1));
             }
             else
             {
                 // Obtiene los bytes de la imagen descargada.
                 byte[] fileContents = task2.Result;

                 // Crea una textura desde los bytes de la imagen.
                 Texture2D texture = new Texture2D(1, 1);

                 if (texture.LoadImage(fileContents))
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
}
