using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class RemoteDb : IRepositoryRemote
{

    public void DeleteItemRemoteById(ItemRemote itemLocal)
    {
        throw new System.NotImplementedException();
    }

    public ItemRemote GetItemRemoteById(string id)
    {
        throw new System.NotImplementedException();
    }

    public Task<List<ItemRemote>> GetProductsRemoteAsync()
    {
        throw new System.NotImplementedException();
    }

    public void SaveItemRemote(string itemName, string remoteFilePath)
    {
        // string id, string name, string path, string timestamp
        // Debug.Log(referenciaRuta);
        String userUid = FirebaseSDK.GetInstance().auth.CurrentUser.UserId;
        DatabaseReference referenciaBaseDatos = FirebaseSDK.GetInstance().db.RootReference;
        string clave = referenciaBaseDatos.Child("users").Child("items").Child(userUid).Push().Key;

        // Obtener la marca de tiempo del servidor en formato Unix
        long timestampUnix = (long)(System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds;

        ItemRemote entry =
            new ItemRemote(clave, itemName, remoteFilePath, timestampUnix);
        Dictionary<string, System.Object> entryValues = entry.ToDictionary();

        referenciaBaseDatos.Child("users").Child("items").Child(userUid).Child(clave).SetValueAsync(entryValues).ContinueWith(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                // Manejar error
                Debug.LogError("Error al escribir en la base de datos: " + task.Exception);
            }
            else
            {
                // Operación exitosa
                Debug.Log("Datos escritos exitosamente en la base de datos");
            }
        }); ;
    }

    public void UpdateItemRemoteById(ItemRemote itemRemote)
    {
        throw new System.NotImplementedException();
    }

}
