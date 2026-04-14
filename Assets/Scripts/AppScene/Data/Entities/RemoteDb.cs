using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq; // Añadido para facilitar la búsqueda en la lista

public class RemoteDb : IRepositoryRemote
{
    public delegate void OnHandleValueChangedCallBack(List<ItemRemote> itemsRemoteList);
    public event OnHandleValueChangedCallBack handleValueResult;
    private string userUid;

    // Mantenemos una referencia local a los items para saber qué IDs están ocupados
    private List<ItemRemote> lastKnownItems = new List<ItemRemote>();

    public void SetUserUid(string userUid)
    {
        this.userUid = userUid;
    }

    public RemoteDb GetRemoteDb()
    {
        return this;
    }

    public async Task FirebaseValueChanged()
    {
        if (!IsUserUid()) return;

        FirebaseSDK.GetInstance().defaultInstance
            .GetReference("users")
            .Child(userUid)
            .Child("items")
            .ValueChanged += HandleValueChanged;

        await Task.Delay(1000);
    }

    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        List<ItemRemote> itemsRemoteList = new List<ItemRemote>();

        if (args.Snapshot.Exists)
        {
            foreach (DataSnapshot itemSnapshot in args.Snapshot.Children)
            {
                Dictionary<string, object> itemData = (Dictionary<string, object>)itemSnapshot.Value;

                ItemRemote item = new ItemRemote
                {
                    Id = itemSnapshot.Key, // Usamos la Key del nodo como ID (será "1", "2", etc.)
                    Name = itemData["name"].ToString(),
                    ImageName = itemData["image_name"].ToString(),
                    CreationDate = long.Parse(itemData["creation_date"].ToString())
                };

                itemsRemoteList.Add(item);
            }
        }

        lastKnownItems = itemsRemoteList; // Actualizamos nuestra copia local
        AppConfig.SetCurrentItemsCount(itemsRemoteList.Count);
        handleValueResult?.Invoke(itemsRemoteList);
    }

    public void CancelHandleValueChanged()
    {
        if (!IsUserUid()) return;

        FirebaseSDK.GetInstance().defaultInstance
         .GetReference("users")
         .Child(userUid)
         .Child("items")
         .ValueChanged -= HandleValueChanged;
    }

    public async Task<bool> SaveItemRemote(ItemRemote itemRemote, IResult resultUi)
    {
        bool saveSuccess = false;
        if (!IsUserUid()) return saveSuccess;

        // --- LÓGICA DE IDS FIJOS (1-10) ---
        string nextId = GetFirstAvailableId();

        if (nextId == null)
        {
            Debug.LogWarning("Límite de 10 cubos alcanzado.");
            resultUi.SetResultCrudUi("Limit Reached", "You can only have 10 items.");
            return false;
        }

        DatabaseReference rootRef = FirebaseSDK.GetInstance().defaultInstance.RootReference;
        long timestampUnix = TimeUtils.GetTimeStampUnix();

        // El ID ya no es un Push(), es el número que encontramos
        ItemRemote entry = new ItemRemote(nextId, itemRemote.Name, itemRemote.ImageName, timestampUnix);
        Dictionary<string, System.Object> entryValues = entry.ToDictionary();

        await rootRef.Child("users").Child(userUid).Child("items").Child(nextId).SetValueAsync(entryValues)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError("Error en Firebase: " + task.Exception);
                    resultUi.SetResultCrudUi("Error", "Validation failed (Check Rules)");
                    saveSuccess = false;
                }
                else
                {
                    Debug.Log("Cubo guardado en Slot: " + nextId);
                    resultUi.SetResultCrudUi("Completed", "Item added to slot " + nextId);
                    saveSuccess = true;
                }
            });

        return saveSuccess;
    }

    // Función auxiliar para encontrar el primer hueco del 1 al 10
    private string GetFirstAvailableId()
    {
        for (int i = 1; i <= 10; i++)
        {
            string idEvaluado = i.ToString();
            // Si ningún item actual tiene este ID, está libre
            if (!lastKnownItems.Any(item => item.Id == idEvaluado))
            {
                return idEvaluado;
            }
        }
        return null; // Todo lleno
    }

    public async Task<bool> UpdateItemRemote(ItemRemote itemRemote, IResult iResult)
    {
        bool updateSuccess = false;
        if (!IsUserUid()) return updateSuccess;

        DatabaseReference rootRef = FirebaseSDK.GetInstance().defaultInstance.RootReference;
        await rootRef
             .Child("users")
             .Child(userUid)
             .Child("items")
             .Child(itemRemote.Id)
             .UpdateChildrenAsync(itemRemote.ToDictionary()).ContinueWithOnMainThread(task =>
             {
                 if (task.IsFaulted || task.IsCanceled)
                 {
                     Debug.LogError("Error actualizando: " + task.Exception);
                     iResult.SetResultCrudUi("Error", "Update failed");
                     updateSuccess = false;
                 }
                 else
                 {
                     iResult.SetResultCrudUi("Update", "Data updated successfully");
                     updateSuccess = true;
                 }
             });

        return updateSuccess;
    }

    public async Task<bool> DeleteItemRemoteById(string id, IResult iResult)
    {
        if (!IsUserUid()) return false;

        DatabaseReference rootRef = FirebaseSDK.GetInstance().defaultInstance.RootReference;
        bool deleteSuccess = false;

        await rootRef
             .Child("users")
             .Child(userUid)
             .Child("items")
             .Child(id)
             .RemoveValueAsync().ContinueWithOnMainThread(task =>
             {
                 if (task.IsFaulted || task.IsCanceled)
                 {
                     Debug.LogError("Error al borrar: " + task.Exception);
                     iResult.SetResultCrudUi("Error", "Delete failed");
                     deleteSuccess = false;
                 }
                 else
                 {
                     Debug.Log("Slot " + id + " liberado.");
                     iResult.SetResultCrudUi("Deleted", "Slot freed successfully");
                     deleteSuccess = true;
                 }
             });
        return deleteSuccess;
    }

    private bool IsUserUid()
    {
        if (userUid == null)
        {
            Debug.LogWarning("No hay un userUid!!!");
            return false;
        }
        return true;
    }
}