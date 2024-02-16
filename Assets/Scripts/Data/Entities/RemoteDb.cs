using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Networking.UnityWebRequest;

public class RemoteDb : IRepositoryRemote
{
    public delegate void OnHandleValueChangedCallBack(List<ItemRemote> itemsRemoteList);
    public event OnHandleValueChangedCallBack handleValueResult;
    private string userUid;

    public RemoteDb()
    {
        // Obtén el ID del usuario actual
         this.userUid = FirebaseSDK.GetInstance().user.UserId;
    }

    public RemoteDb GetRemoteDb()
    {
        return this;
    }

    public void FirebaseValueChanged()
    {
        // Verificar el estado de la conexión a Internet
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            FirebaseSDK.GetInstance().db
                   .GetReference("users")
                   .Child("items")
                   .Child(userUid)
             .ValueChanged += HandleValueChanged;
        }
    }

    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        // Do something with the data in args.Snapshot

        List<ItemRemote> itemsRemoteList = new List<ItemRemote>();

        foreach (DataSnapshot itemSnapshot in args.Snapshot.Children)
        {
            // Convierte los datos del snapshot en una instancia de ItemRemote
            ItemRemote item = new ItemRemote(
                id: itemSnapshot.Child("id").GetValue(true).ToString(),
                name: itemSnapshot.Child("name").GetValue(true).ToString(),
                imageName: itemSnapshot.Child("image_name").GetValue(true).ToString(),
                creationDate: long.Parse(itemSnapshot.Child("creation_date").GetValue(true).ToString()));

            itemsRemoteList.Add(item);
        }
        handleValueResult?.Invoke(itemsRemoteList);
    }

    public async Task<List<ItemRemote>> GetItemsRemote()
    {
        // Crear un objeto TaskCompletionSource para controlar la finalización de la tarea
        var tcs = new TaskCompletionSource<List<ItemRemote>>();

        // Obtén la referencia a la ubicación de los items para el usuario actual
        await FirebaseSDK.GetInstance().db
            .GetReference("users")
            .Child("items")
            .Child(userUid).GetValueAsync().ContinueWithOnMainThread(task =>
            {

                if (task.IsFaulted)
                {
                    // Handle the error...
                    tcs.SetException(task.Exception);
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    List<ItemRemote> itemsList = new List<ItemRemote>();

                    foreach (DataSnapshot itemSnapshot in snapshot.Children)
                    {
                        // Convierte los datos del snapshot en una instancia de ItemRemote
                        ItemRemote item = new ItemRemote(
                            id: itemSnapshot.Child("id").GetValue(true).ToString(),
                            name: itemSnapshot.Child("name").GetValue(true).ToString(),
                            imageName: itemSnapshot.Child("image_name").GetValue(true).ToString(),
                            creationDate: long.Parse(itemSnapshot.Child("creation_date").GetValue(true).ToString()));

                        itemsList.Add(item);
                    }
                    // Establecer el resultado de la tarea como la lista de items
                    tcs.SetResult(itemsList);
                }
            });

        // Devolver la tarea asociada con el TaskCompletionSource
        return await tcs.Task;
    }

    public void SaveItemRemote(ItemRemote itemRemote, IResult resultUi)
    {
        DatabaseReference rootRef = FirebaseSDK.GetInstance().db.RootReference;

        // key generada con Push()
        string key = rootRef.Child("users").Child("items").Child(userUid).Push().Key;

        // Obtener la marca de tiempo del servidor en formato Unix
        long timestampUnix = (long)(System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds;

        ItemRemote entry =
            new ItemRemote(key, itemRemote.Name, itemRemote.ImageName, timestampUnix);

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
        });
    }

    public void UpdateItemRemote(ItemRemote itemRemote, IResult iResult)
    {
        DatabaseReference rootRef = FirebaseSDK.GetInstance().db.RootReference;
        rootRef
            .Child("users")
            .Child("items")
            .Child(userUid)
            .Child(itemRemote.Id)
            .UpdateChildrenAsync(itemRemote.ToDictionary()).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    
                    // Manejar error
                    Debug.LogError("Error al escribir en la base de datos: " + task.Exception);
                    iResult.SetResultCrudUi(false, "Error al actualizar la base de datos");
                }
                else
                {
                    // Operación exitosa
                    Debug.Log("Datos escritos exitosamente en la base de datos");
                    iResult.SetResultCrudUi(true, "Datos actualizados exitosamente en la base de datos");
                }
            });
    }

   
    public void DeleteItemRemoteById(string id, IResult iResult)
    {
        DatabaseReference rootRef = FirebaseSDK.GetInstance().db.RootReference;
        rootRef
            .Child("users")
            .Child("items")
            .Child(userUid)
            .Child(id)
            .RemoveValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {

                    // Manejar error
                    Debug.LogError("Error al borrar el item remoto en la base de datos: " + task.Exception);
                    iResult.SetResultCrudUi(false, "Error al borrar el ítem remoto de la base de datos");
                }
                else
                {
                    // Operación exitosa
                    Debug.Log("Ítem remoto borrado correctamente");
                    iResult.SetResultCrudUi(true, "Ítem remoto borrado correctamente");
                }
            });
    }
}
