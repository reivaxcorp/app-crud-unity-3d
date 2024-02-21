using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class ManageItems : MonoBehaviour
{

    [SerializeField]
    private GameObject itemPrefab;
    [SerializeField]
    private GameObject myItemsOrdered;
    [SerializeField]
    private GameObject loadingScreen;
    private BuildItem buildItem;
    private bool waitToFirebaseInitialized;
    private NetworkManager networkManager;
    private bool syncStarted;
    private List<ItemLocal> itemsLocalList;

    private void Awake()
    {
        buildItem = gameObject.AddComponent<BuildItem>();
    }

    // Start is called before the first frame update
    void Start()
    {
        this.itemsLocalList = new List<ItemLocal>();
        this.syncStarted = false;
        waitToFirebaseInitialized = true;
        SetLoadingMsj(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (waitToFirebaseInitialized)
        {
            if (FirebaseSDK.GetInstance().isFirebaseReady &&
                MyApplication.repository != null &&
                FirebaseSDK.GetInstance().defaultInstance != null)
            {
                waitToFirebaseInitialized = false;
           
                LoadLocalData();
            }
        }
    }

    // cargamos la base de datos local, y si hay internet luego la remota
    private async void LoadLocalData()
    {

        List<ItemLocal> itemsLocal = await MyApplication.repository.GetLocalItemsAsync();
       
        this.itemsLocalList = itemsLocal;

        List<Task> tasks = new List<Task>(); // Lista para almacenar tareas asíncronas

        foreach (ItemLocal itemLocal in itemsLocalList)
        {
            Task task = CreateItemInScene(itemLocal);
            tasks.Add(task);
        }
        OrderItem(itemsLocalList);
        await Task.WhenAll(tasks);

        StartCoroutine(CheckInternetConection());
    }

    IEnumerator CheckInternetConection()
    {
        // Esperar 3 segundos
        yield return new WaitForSeconds(3f);

        this.networkManager = GetComponent<NetworkManager>();

        if (networkManager != null)
        {
            networkManager.handleInternetAvariableResult += ResultInternetAvariable;
            networkManager.ListeningInternetAvariable();
        }
        else
        {
            Debug.LogWarning("NetworManager.cs no esta en el Manager");
        }
    }

    // si hay internet, podemos leer la base de datos remote, de lo contrario no hacemos nada
    private void ResultInternetAvariable(bool isInternetAvariable)
    {
        if (isInternetAvariable)
        {
            ListeningDbRemote();
        }
    }

    // Escuchamos los cambios en la base de datos remota, al suscribirnos a los cambios
    private async void ListeningDbRemote()
    {
        RemoteDb remoteDbRef = MyApplication.repository.GetRemoteDb();
        remoteDbRef.handleValueResult += SyncronizeData;
        await remoteDbRef.FirebaseValueChanged();
    }

    /// <summary>
    /// Sincronizamos datos guardados con los datos locales.
    /// </summary>
    /// <param name="itemsRemoteList">La lista con el que se realizará la operación. Puede ser null.</param>
    private async void SyncronizeData(List<ItemRemote> itemsRemoteList)
    {
        if (syncStarted) return;

        syncStarted = true;

        //  List<ItemLocal> itemsLocalList = await MyApplication.repository.GetLocalItemsAsync();
        List<ItemLocal> itemsToSave = new List<ItemLocal>();

        List<Task> tasks = new List<Task>(); // Lista para almacenar tareas asíncronas

        bool isSomeListDbEquals = IsListsDbEquals(itemsLocalList, itemsRemoteList);
        Debug.Log("Ambás db son iguales? " + isSomeListDbEquals + " " +
            itemsLocalList.Count + " " + itemsRemoteList.Count);

        //  printDb(itemsLocalList, itemsRemoteList);

        if (!isSomeListDbEquals)
        {

            List<ItemUpdate> itemListUpdates =
                     CheckUpdates.CheckUpdatesItems(itemsRemoteList, itemsLocalList);

            foreach (ItemUpdate itemToUpdate in itemListUpdates)
            {

                Task task = Task.CompletedTask; // Inicializar una tarea completada

                if (itemToUpdate.IsFieldsUpdated && itemToUpdate.IsImageUpdated)
                {
                    ItemLocal itemLocalUptated = itemsRemoteList.Find(item => item.Id.Equals(itemToUpdate.Id))
                        .ItemRemoteToItemLocal();
                    ItemLocal itemOld = itemsLocalList.Find(item => item.Id.Equals(itemToUpdate.Id));
                    RemoveLocalTexture(itemOld.ImageName);
                    RemoveRemoteOldTexture(itemOld.ImageName);
                    task = UpdateItemInScene(item: itemLocalUptated, isFieldUpdate: true, isImageUpdate: true);
                    itemsToSave.Add(itemLocalUptated);
                }
                else if (itemToUpdate.IsFieldsUpdated)
                {
                    ItemLocal itemLocalUptated = itemsRemoteList.Find(item => item.Id.Equals(itemToUpdate.Id))
                        .ItemRemoteToItemLocal();
                    task = UpdateItemInScene(item: itemLocalUptated, isFieldUpdate: true, isImageUpdate: false);
                    itemsToSave.Add(itemLocalUptated);
                }
                else if (itemToUpdate.IsImageUpdated)
                {
                    ItemLocal itemLocalUptated = itemsRemoteList.Find(item => item.Id.Equals(itemToUpdate.Id))
                        .ItemRemoteToItemLocal();
                    ItemLocal itemOld = itemsLocalList.Find(item => item.Id.Equals(itemToUpdate.Id));
                    RemoveLocalTexture(itemOld.ImageName);
                    RemoveRemoteOldTexture(itemOld.ImageName);
                    task = UpdateItemInScene(item: itemLocalUptated, isFieldUpdate: false, isImageUpdate: true);
                    itemsToSave.Add(itemLocalUptated);
                }
                else if (itemToUpdate.IsRemove)
                {
                    ItemLocal itemLocal = itemsLocalList.Find(item => item.Id.Equals(itemToUpdate.Id));
                    await MyApplication.repository.DeleteLocalItemById(itemLocal.Id);
                    RemoveLocalTexture(itemLocal.ImageName);
                    RemoveRemoteOldTexture(itemLocal.ImageName);
                    DeleteItemInScene(itemLocal);
                }
                else if (itemToUpdate.IsAdd)
                {
                    // Nuevo item añadido
                    ItemLocal itemLocal = itemsRemoteList.Find(item => item.Id.Equals(itemToUpdate.Id))
                        .ItemRemoteToItemLocal();
                    task = CreateItemInScene(itemLocal);
                    itemsToSave.Add(itemLocal);
                }
                else
                {
                    // sin cambios el ítem local con el ítem remoto
                    ItemLocal itemLocal = itemsLocalList.Find(item => item.Id.Equals(itemToUpdate.Id));
                    task = CreateItemInScene(itemLocal);
                    itemsToSave.Add(itemLocal);
                }
                tasks.Add(task); // Agregar la tarea a la lista de tareas
            }

            // Esperar a que todas las tareas se completen
            await Task.WhenAll(tasks);
            OrderItem(itemsToSave);
            // actualizamos la lista local con la remota
            await MyApplication.repository.SaveLocalItemsAsync(itemsToSave);
        }
        else
        {
            // Estamos sin conexión a internet, cargamos los datos locales
            foreach (ItemLocal itemLocal in itemsLocalList)
            {
                Task task = Task.CompletedTask;
                task = CreateItemInScene(itemLocal);
                itemsToSave.Add(itemLocal);
                tasks.Add(task);
            }

            // Esperar a que todas las tareas se completen
            await Task.WhenAll(tasks);
            OrderItem(itemsLocalList);

            await MyApplication.repository.SaveLocalItemsAsync(itemsToSave);
        }

        SetLoadingMsj(false);

        syncStarted = false;
    }

    private async Task<bool> CreateItemInScene(ItemLocal item)
    {
        if (itemPrefab != null)
        {
            if (myItemsOrdered.transform.Find(item.Id) == null)
            {
                {
                    GameObject itemToCreate = Instantiate(itemPrefab);
                    TextMeshPro[] textMeshProChildren = itemToCreate.GetComponentsInChildren<TextMeshPro>();

                    if (textMeshProChildren.Length == 2 && textMeshProChildren[0] != null && textMeshProChildren[1] != null)
                    {
                        textMeshProChildren[0].text = item.Name;
                        textMeshProChildren[1].text = TimeUtils.ConvertTimeStampUnixToDate(item.CreationDate);
                    }

                    itemToCreate.name = item.Id;
                    await buildItem.AsignMaterialAsync(item.ImageName, itemToCreate);
                }
            }
            else
            {
                Debug.Log("Ítem ya existente (No es necesario crearlo) " + item.Id);
            }
        }
        else
        {
            Debug.LogWarning("Por favor, coloca la referencia del item prefab en el ispector");
            return false;
        }
        return true;
    }

    private void RemoveLocalTexture(string oldImageName)
    {
        MyApplication.repository.RemoveLocalTexture(oldImageName);
    }

    private async void RemoveRemoteOldTexture(string remoteOldImage)
    {
        ManageMaterialRemote manageMaterialRemote =
                     new ManageMaterialRemote(remoteOldImage);
        await manageMaterialRemote.DeleteImageRemote();
    }

    private async Task<bool> UpdateItemInScene(
        ItemLocal item,
        bool isFieldUpdate,
        bool isImageUpdate
        )
    {
        GameObject gameObjectExists = GameObject.Find(item.Id);

        if (gameObjectExists != null)
        {
            if (isFieldUpdate)
            {
                gameObjectExists.GetComponentInChildren<TextMeshPro>().text = item.Name;
            }

            if (isImageUpdate)
            {
                await buildItem.AsignMaterialAsync(item.ImageName, gameObjectExists);
            }
        }
        return true;
    }

    private void DeleteItemInScene(ItemLocal item)
    {
        GameObject gameObjectExists = GameObject.Find(item.Id);

        if (gameObjectExists != null)
        {
            Destroy(gameObjectExists);
        }
    }

    private void OrderItem(List<ItemLocal> itemsLocalList)
    {
        if (myItemsOrdered != null)
        {
            if (itemsLocalList.Count > 0)
            {
                MyItemsOrder myItemsOrder = myItemsOrdered.GetComponent<MyItemsOrder>();
                if (myItemsOrder != null)
                {
                    myItemsOrder.OrderItemPositionInScene(itemsLocalList);
                }
                else
                {
                    Debug.LogWarning("MyItemsOrder.cs no colocado en MyItemsOrdered gameObject");
                }
            }
            else
            {
                Debug.Log("Lista de items local vacia");
            }
        }
        else
        {
            Debug.LogWarning("Por favor pon MyItemsOrdened game object en el inspector en Manager");
        }
    }

    private void SetLoadingMsj(bool isActive)
    {
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(isActive);
        }
        else
        {
            Debug.LogWarning("Por favor pon el LoadingMsj en el Manager desde UiApp gameObject");
        }
    }

    private void OnDestroy()
    {
        DesuscribeEventsDbListening();
    }

    private void DesuscribeEventsDbListening()
    {
        if (MyApplication.repository != null)
        {
            RemoteDb remoteDbRef = MyApplication.repository.GetRemoteDb();
            if (remoteDbRef != null)
            {
                remoteDbRef.CancelHandleValueChanged();
                remoteDbRef.handleValueResult -= SyncronizeData;
            }
        }
        if (networkManager != null)
            networkManager.handleInternetAvariableResult -= ResultInternetAvariable;
    }

    private bool IsListsDbEquals(List<ItemLocal> itemLocals, List<ItemRemote> itemRemotes)
    {
        if (itemLocals.Count != itemRemotes.Count) return false;
        if (itemLocals.Count == 0) return false;

        bool isSameContent = true;

        for (int i = 0; i < itemLocals.Count; i++)
        {
            ItemRemote itemRemote =
                itemRemotes.Find(item => item.Id.Equals(itemLocals[i].Id));
            isSameContent = isSameContent && ItemExtensions.IsSameContent(itemLocals[i], itemRemote);
        }
        return isSameContent;
    }

    private void printDb(List<ItemLocal> itemLocals, List<ItemRemote> itemsRemote)
    {
        Debug.Log("LOCAL DBBBBBBBBBBBBBBBBBBBBBBBBBBBBB");
        for (int i = 0; i < itemLocals.Count; i++)
        {

            Debug.Log(itemLocals[i].Id);
            Debug.Log(itemLocals[i].Name);
            Debug.Log(itemLocals[i].ImageName);
            Debug.Log(itemLocals[i].CreationDate);

        }
        Debug.Log("FIN LOCAL DBBBBBBBBBBBBBBBBBBBBBBBBBBBBB");


        Debug.Log("REMOTE DBBBBBBBBBBBBBBBBBBBBBBBBBBBBB");
        for (int i = 0; i < itemsRemote.Count; i++)
        {
            Debug.Log(itemsRemote[i].Id);
            Debug.Log(itemsRemote[i].Name);
            Debug.Log(itemsRemote[i].ImageName);
            Debug.Log(itemsRemote[i].CreationDate);
        }
        Debug.Log("FIN REMOTE DBBBBBBBBBBBBBBBBBBBBBBBBBBBBB");

    }
}
