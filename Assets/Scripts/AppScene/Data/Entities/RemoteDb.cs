/*********************************************************************************
 * Nombre del Archivo:     RemoteDb.cs
 * Descripción:            Creamos, leemos, actualizamos, borramos los ítems en Realtime Database.
                           También aprovecharemos la base de datos en tiempo real, al escuchar algún cambio
                           que será manejado por nuestro ItemManager.cs 
 *                         
 * Autor:                  Javier
 * Organización:           ReivaxCorp.
 *
 * Derechos de Autor (c) [2024] ReivaxCorp
 * 
 * Permiso es otorgado, sin cargo, para que cualquier persona obtenga una copia
 * de este software y de los archivos de documentación asociados (el "Software"),
 * para tratar en el Software sin restricción, incluyendo sin limitación los
 * derechos para usar, copiar, modificar, fusionar, publicar, distribuir,
 * sublicenciar, y/o vender copias del Software, y para permitir a las personas a
 * quienes pertenezca el Software, sujeto a las siguientes condiciones:
 *
 * El aviso de derechos de autor anterior y este aviso de permiso se incluirán en
 * todas las copias o partes sustanciales del Software.
 *
 * EL SOFTWARE SE PROPORCIONA "TAL CUAL", SIN GARANTÍA DE NINGÚN TIPO, EXPRESA O
 * IMPLÍCITA, INCLUYENDO PERO NO LIMITADO A LAS GARANTÍAS DE COMERCIABILIDAD,
 * IDONEIDAD PARA UN PROPÓSITO PARTICULAR Y NO INFRACCIÓN. EN NINGÚN CASO LOS
 * AUTORES O TITULARES DE DERECHOS DE AUTOR SERÁN RESPONSABLES DE CUALQUIER
 * RECLAMACIÓN, DAÑO O OTRA RESPONSABILIDAD, YA SEA EN UNA ACCIÓN DE CONTRATO, AGRAVIO
 * O DE OTRO MODO, DERIVADAS DE, FUERA DE O EN CONEXIÓN CON EL SOFTWARE O EL USO U OTROS
 * TRATOS EN EL SOFTWARE.
 *********************************************************************************/

using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RemoteDb : IRepositoryRemote
{
    public delegate void OnHandleValueChangedCallBack(List<ItemRemote> itemsRemoteList);
    public event OnHandleValueChangedCallBack handleValueResult;
    private string userUid;


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

        // Esperar 1 segundo antes de continuar para asegurarse de que el suscriptor se ha registrado correctamente
        await Task.Delay(1000);
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

        AppConfig.SetCurrentItemsCount(itemsRemoteList.Count);
        // Debug.Log("handled itemsRemoteList " + itemsRemoteList.Count);
        handleValueResult?.Invoke(itemsRemoteList);
    }

    public void CancelHandleValueChanged()
    {
        if (!IsUserUid()) return;

        FirebaseSDK.GetInstance().defaultInstance
         .GetReference("users")
         .Child(userUid)
         .Child("items")
         .ValueChanged -= HandleValueChanged; // unsubscribe from ValueChanged.
    }


    public async Task<bool> SaveItemRemote(ItemRemote itemRemote, IResult resultUi)
    {
        bool saveSuccess = false;

        if (!IsUserUid()) return saveSuccess;

        DatabaseReference rootRef = FirebaseSDK.GetInstance().defaultInstance.RootReference;

        // key generada con Push()
        string key = rootRef.Child("users").Child(userUid).Child("items").Push().Key;

        // Obtener la marca de tiempo del servidor en formato Unix
        long timestampUnix = TimeUtils.GetTimeStampUnix();

        ItemRemote entry =
            new ItemRemote(key, itemRemote.Name, itemRemote.ImageName, timestampUnix);

        Dictionary<string, System.Object> entryValues = entry.ToDictionary();

        await rootRef.Child("users").Child(userUid).Child("items").Child(key).SetValueAsync(entryValues)
            .ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                // Manejar error
                Debug.LogError("Error al escribir en la base de datos: " + task.Exception);
                resultUi.SetResultCrudUi("Error", "Error al escribir en la base de datos");
                saveSuccess = false;
            }
            else
            {
                // Operación exitosa
                Debug.Log("Datos escritos exitosamente en la base de datos");
                resultUi.SetResultCrudUi("Completado", "Nuevo ítem agregado");
                saveSuccess = true;
            }
        });

        return saveSuccess;
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

                     // Manejar error
                     Debug.LogError("Error al escribir en la base de datos: " + task.Exception);
                     iResult.SetResultCrudUi("Error", "Error al actualizar");
                     updateSuccess = false;
                 }
                 else
                 {
                     // Operación exitosa
                     Debug.Log("Datos escritos exitosamente en la base de datos");
                     iResult.SetResultCrudUi("Actualización", "Datos actualizados exitosamente");
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

                     // Manejar error
                     Debug.LogError("Error al borrar el item remoto en la base de datos: " + task.Exception);
                     iResult.SetResultCrudUi("Error", "Error al borrar el ítem remoto de la base de datos");
                     deleteSuccess = false;
                 }
                 else
                 {
                     // Operación exitosa
                     Debug.Log("Ítem remoto borrado correctamente");
                     iResult.SetResultCrudUi("Borrado", "Ítem remoto borrado correctamente");
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