using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RemoteDb : IRepositoryRemote
{

    public void DeleteItemRemote(ItemRemote itemLocal)
    {
        throw new System.NotImplementedException();
    }

    public ItemRemote GetItemRemoteById(string id)
    {
        throw new System.NotImplementedException();
    }

    public async Task<List<ItemRemote>> GetItemsRemote()
    {
        // Procesa los datos del snapshot según sea necesario
        List<ItemRemote> itemsList = new List<ItemRemote>();

        // Obtén el ID del usuario actual
        string userUid = FirebaseSDK.GetInstance().user.UserId;

        // Obtén la referencia a la ubicación de los items para el usuario actual
        DatabaseReference userItemsReference = FirebaseSDK.GetInstance().db.RootReference
            .Child("users").Child("items").Child(userUid);

        try
        {
            // Realiza la operación de obtención de datos de Firebase
            DataSnapshot snapshot = await userItemsReference.GetValueAsync();

            if (snapshot.Exists)
            {
        
                foreach (DataSnapshot itemSnapshot in snapshot.Children)
                {
                    // Convierte los datos del snapshot en una instancia de ItemRemote
                    ItemRemote item = new ItemRemote
                    {
                        Id = itemSnapshot.Child("id").GetValue(true).ToString(),
                        Name = itemSnapshot.Child("name").GetValue(true).ToString(),
                        Path = itemSnapshot.Child("path").GetValue(true).ToString(),
                        CreationDate = long.Parse(itemSnapshot.Child("timestamp").GetValue(true).ToString())
                    };

                    itemsList.Add(item);
                }

                return itemsList;
            }
            else
            {
                Debug.Log("No hay datos en la ubicación especificada.");
                return itemsList;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error al obtener items: " + e.Message);
            return itemsList;
        }
    }

    public void SaveItemRemote(ItemRemote itemRemote, IResult resultUi)
    {
        // string id, string name, string path, string timestamp
        // Debug.Log(referenciaRuta);
        String userUid = FirebaseSDK.GetInstance().auth.CurrentUser.UserId;
        DatabaseReference rootRef = FirebaseSDK.GetInstance().db.RootReference;

        // key generada con Push()
        string key = rootRef.Child("users").Child("items").Child(userUid).Push().Key;

        // Obtener la marca de tiempo del servidor en formato Unix
        long timestampUnix = (long)(System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds;

        ItemRemote entry =
            new ItemRemote(key, itemRemote.Name, itemRemote.Path, itemRemote.ImageIdMetadata, timestampUnix);

        Dictionary<string, System.Object> entryValues = entry.ToDictionary();

        rootRef.Child("users").Child("items").Child(userUid).Child(key).SetValueAsync(entryValues).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                // Manejar error
                Debug.LogError("Error al escribir en la base de datos: " + task.Exception);
                resultUi.SetResultCrudUi(false, "Error al escribir en la base de datos");
            }
            else
            {
                // Operación exitosa
                Debug.Log("Datos escritos exitosamente en la base de datos");
                resultUi.SetResultWriteDocument(true, "Ítem subido", "Nuevo ítem agregado");
            }
        }); ;
    }

    public void UpdateItemRemoteById(ItemRemote itemRemote)
    {
        throw new System.NotImplementedException();
    }

}
