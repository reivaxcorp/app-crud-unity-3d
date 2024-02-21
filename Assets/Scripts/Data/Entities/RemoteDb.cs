using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

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

    public async Task FirebaseValueChanged()
    {

        FirebaseSDK.GetInstance().defaultInstance
            .GetReference("users")
            .Child("items")
            .Child(userUid)
            .ValueChanged += HandleValueChanged;

        // Esperar 1 segundo antes de continuar para asegurarse de que el suscriptor se ha registrado correctamente
        await Task.Delay(1000);
    }

    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        Debug.Log("handled");
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        // Do something with the data in args.Snapshot

        List<ItemRemote> itemsRemoteList = new List<ItemRemote>();

        foreach (DataSnapshot itemSnapshot in args.Snapshot.Children)
        {
            // Obtener el valor del DataSnapshot y convertirlo a un diccionario
            Dictionary<string, object> itemData = (Dictionary<string, object>)itemSnapshot.Value;

            // Crear un nuevo objeto ItemRemote y asignar los valores del diccionario
            ItemRemote item = new ItemRemote
            {
                // Ajusta estas líneas según la estructura de tus datos remotos
                Id = itemData["id"].ToString(),
                Name = itemData["name"].ToString(),
                ImageName = itemData["image_name"].ToString(),
                CreationDate = long.Parse(itemData["creation_date"].ToString())
            };

            // Agregar el objeto ItemRemote a la lista
            itemsRemoteList.Add(item);
        }

        Debug.Log("handled itemsRemoteList " + itemsRemoteList.Count);
        handleValueResult?.Invoke(itemsRemoteList);
    }

    public void CancelHandleValueChanged()
    {
        Debug.Log("desuscribe " + userUid);
        FirebaseSDK.GetInstance().defaultInstance
         .GetReference("users")
         .Child("items")
         .Child(userUid).ValueChanged -= HandleValueChanged; // unsubscribe from ValueChanged.
    }

    public async Task<List<ItemRemote>> GetItemsRemote()
    {

        List<ItemRemote> itemsList = new List<ItemRemote>();

        // Obtén la referencia a la ubicación de los items para el usuario actual
        await FirebaseSDK.GetInstance().defaultInstance
            .GetReference("users")
            .Child("items")
            .Child(userUid).GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    // Handle the error...
                    throw new Exception("Error al recuperar los datos " + task.Exception.ToString());
                }
                else if (task.IsCompleted)
                {
                    // Iterar sobre los hijos del DataSnapshot
                    foreach (DataSnapshot itemSnapshot in task.Result.Children)
                    {
                        // Obtener el valor del DataSnapshot y convertirlo a un diccionario
                        Dictionary<string, object> itemData = (Dictionary<string, object>)itemSnapshot.Value;

                        // Crear un nuevo objeto ItemRemote y asignar los valores del diccionario
                        ItemRemote item = new ItemRemote
                        {
                            // Ajusta estas líneas según la estructura de tus datos remotos
                            Id = itemData["id"].ToString(),
                            Name = itemData["name"].ToString(),
                            ImageName = itemData["image_name"].ToString(),
                            CreationDate = long.Parse(itemData["creation_date"].ToString())
                        };

                        // Agregar el objeto ItemRemote a la lista
                        itemsList.Add(item);
                    }

                    /*foreach(Dictionary<string, object> dictionary in (snapshot.Value as Dictionary<string, object>).Values)
                    {
                       foreach(string key in dictionary.Keys)
                        {
                             Debug.Log(key + ": " + dictionary[key]);
                        }
                    }*/


                    /*foreach (DataSnapshot itemSnapshot in snapshot.Children)
                    {
                        // Convierte los datos del snapshot en una instancia de ItemRemote
                        ItemRemote item = new ItemRemote(
                            id: itemSnapshot.Child("id").GetValue(true).ToString(),
                            name: itemSnapshot.Child("name").GetValue(true).ToString(),
                            imageName: itemSnapshot.Child("image_name").GetValue(true).ToString(),
                            creationDate: long.Parse(itemSnapshot.Child("creation_date").GetValue(true).ToString()));

                        itemsList.Add(item);
                    }*/
                }
            });

        return itemsList;
    }

    public void SaveItemRemote(ItemRemote itemRemote, IResult resultUi)
    {
        DatabaseReference rootRef = FirebaseSDK.GetInstance().defaultInstance.RootReference;

        // key generada con Push()
        string key = rootRef.Child("users").Child("items").Child(userUid).Push().Key;

        // Obtener la marca de tiempo del servidor en formato Unix
        long timestampUnix = TimeUtils.GetTimeStampUnix();

        ItemRemote entry =
            new ItemRemote(key, itemRemote.Name, itemRemote.ImageName, timestampUnix);

        Dictionary<string, System.Object> entryValues = entry.ToDictionary();

        rootRef.Child("users").Child("items").Child(userUid).Child(key).SetValueAsync(entryValues).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                // Manejar error
                Debug.LogError("Error al escribir en la base de datos: " + task.Exception);
                resultUi.SetResultCrudUi(EResultMenuAction.Failed, "Error al escribir en la base de datos");
            }
            else
            {
                // Operación exitosa
                Debug.Log("Datos escritos exitosamente en la base de datos");
                resultUi.SetResultWriteDocument(EResultMenuAction.Success, "Ítem subido", "Nuevo ítem agregado");
            }
        });
    }

    public void UpdateItemRemote(ItemRemote itemRemote, IResult iResult)
    {
        DatabaseReference rootRef = FirebaseSDK.GetInstance().defaultInstance.RootReference;
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
                    RemoveFailedImageUploaded(itemRemote.ImageName); // remover el archivo subido
                    iResult.SetResultCrudUi(EResultMenuAction.Failed, "Error al actualizar la base de datos");
                }
                else
                {
                    // Operación exitosa
                    Debug.Log("Datos escritos exitosamente en la base de datos");
                    iResult.SetResultCrudUi(EResultMenuAction.Success, "Datos actualizados exitosamente en la base de datos");
                }
            });
    }


    public async Task<bool> DeleteItemRemoteById(string id, IResult iResult)
    {
        DatabaseReference rootRef = FirebaseSDK.GetInstance().defaultInstance.RootReference;

        bool deleteSuccess = false;

        await rootRef
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
                     iResult.SetResultCrudUi(EResultMenuAction.Failed, "Error al borrar el ítem remoto de la base de datos");
                     deleteSuccess = false;
                 }
                 else
                 {
                     // Operación exitosa
                     Debug.Log("Ítem remoto borrado correctamente");
                     iResult.SetResultCrudUi(EResultMenuAction.Success, "Ítem remoto borrado correctamente");
                     deleteSuccess = true;
                 }
             });
        return deleteSuccess;
    }

    /// <summary>
    /// Anteriormente subimos un archivo a firebase storage, asi cuando falla la lectura
    /// en RealtimeDatabase, debemos quitar el archivo subido de firebase storage
    /// </summary>
    private async void RemoveFailedImageUploaded(string imageNameToRemove)
    {
        ManageMaterialRemote manageMaterialRemote = new ManageMaterialRemote(imageNameToRemove);
        await manageMaterialRemote.DeleteImageRemote();
    }
}