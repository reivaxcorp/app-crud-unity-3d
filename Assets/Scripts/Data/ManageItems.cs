using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class ManageItems : MonoBehaviour
{

    [SerializeField]
    private GameObject itemPrefab;
    private BuildItem buildItem;
    private bool waitToFirebaseInitialized;

    private void Awake()
    {
        buildItem = gameObject.AddComponent<BuildItem>();
    }

    // Start is called before the first frame update
    void Start()
    {
        waitToFirebaseInitialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (waitToFirebaseInitialized)
        {
            if (FirebaseSDK.GetInstance().isFirebaseReady &&
                MyApplication.repository != null &&
                FirebaseSDK.GetInstance().db != null)
            {
                waitToFirebaseInitialized = false;
                ListeningDbRemote();
            }
        }
    }

    private void ListeningDbRemote()
    {
        RemoteDb remoteDbRef =
             MyApplication.repository.GetRemoteDb();
        remoteDbRef.handleValueResult += SyncronizeData;
        remoteDbRef.FirebaseValueChanged();
    }

    private async void SyncronizeData(List<ItemRemote> itemsRemoteList)
    {
        List<ItemLocal> itemsLocalList = MyApplication.repository.GetLocalItems();
        List<ItemLocal> itemsToSave = new List<ItemLocal>();

        List<Task> tasks = new List<Task>(); // Lista para almacenar tareas asíncronas

        List<ItemManager> itemListUpdates =
            CheckUpdates.CheckUpdatesItems(itemsRemoteList, itemsLocalList);

        foreach (ItemManager itemToUpdate in itemListUpdates)
        {
            Task task = Task.CompletedTask; // Inicializar una tarea completada

            if (itemToUpdate.IsFieldsUpdated && itemToUpdate.IsImageUpdated)
            {
                ItemLocal itemLocal = itemsRemoteList.Find(item => item.Id == itemToUpdate.Id).ItemRemoteToItemLocal();
                MyApplication.repository.RemoveTexture(itemToUpdate.Id);
                itemsToSave.Add(itemLocal);
                task = CreateItemInScene(itemLocal);
            }
            else if (itemToUpdate.IsFieldsUpdated)
            {
                ItemLocal itemLocal = itemsRemoteList.Find(item => item.Id == itemToUpdate.Id).ItemRemoteToItemLocal();
                itemsToSave.Add(itemLocal);
                task = CreateItemInScene(itemLocal);
            }
            else if (itemToUpdate.IsImageUpdated)
            {
                ItemLocal itemLocal = itemsRemoteList.Find(item => item.Id == itemToUpdate.Id).ItemRemoteToItemLocal();
                MyApplication.repository.RemoveTexture(itemToUpdate.Id);
                itemsToSave.Add(itemLocal);
                task = CreateItemInScene(itemLocal);
            }
            else if (itemToUpdate.IsRemove)
            {
                MyApplication.repository.DeleteLocalItemById(itemToUpdate.Id);
            }
            else if (itemToUpdate.IsAdd)
            {
                // Nuevo item añadido
                ItemLocal itemLocal = itemsRemoteList.Find(item => item.Id == itemToUpdate.Id).ItemRemoteToItemLocal();
                itemsToSave.Add(itemLocal);
                task = CreateItemInScene(itemLocal);
            }
            else
            {
                // sin cambios el ítem local con el ítem remoto
                ItemLocal itemLocal = itemsLocalList.Find(item => item.Id == itemToUpdate.Id);
                itemsToSave.Add(itemLocal);
                task = CreateItemInScene(itemLocal);

            }
            tasks.Add(task); // Agregar la tarea a la lista de tareas
        }

        // Esperar a que todas las tareas se completen
        await Task.WhenAll(tasks);

        MyApplication.repository.SaveLocalItems(itemsToSave);

    }

    private async Task<bool> CreateItemInScene(ItemLocal item)
    {
        if (itemPrefab != null)
        {
            GameObject itemToCreate = Instantiate(itemPrefab);
            itemToCreate.GetComponentInChildren<TextMeshPro>().text = item.Name;
            itemToCreate.name = item.Id;
            await buildItem.AsignMaterialAsync(item.Id, item.Path, itemToCreate);
            return true;
        }
        else
        {
            Debug.LogWarning("Por favor, coloca la referencia del item prefab en el ispector");
            return false;
        }
    }

    private async Task<bool> UpdateItemInScene(ItemLocal item, bool isFieldUpdate, bool isImageUpdate)
    {
        GameObject gameObjectExists = GameObject.Find(item.Name);
        if (gameObjectExists != null)
        {
            if(isImageUpdate)
            {
                gameObjectExists.GetComponentInChildren<TextMeshPro>().text = item.Name;
            }
            if(isImageUpdate)
            {
                await buildItem.AsignMaterialAsync(item.Id, item.Path, gameObjectExists);
            }
        }
        return true;
    }

    private void DeleteItemInScene(ItemLocal item)
    {
        GameObject gameObjectExists = GameObject.Find(item.Name);
        if (gameObjectExists != null)
        { 
            Destroy(gameObjectExists);
        }
    }



    private void OnDestroy()
    {
        DesuscribeEventDbListening();
    }

    private void DesuscribeEventDbListening()
    {
        RemoteDb remoteDbRef = MyApplication.repository.GetRemoteDb();
        remoteDbRef.handleValueResult -= SyncronizeData;
    }
}
