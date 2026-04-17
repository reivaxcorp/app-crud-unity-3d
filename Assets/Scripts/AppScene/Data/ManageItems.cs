using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class ManageItems : MonoBehaviour
{

    [SerializeField]
    private GameObject itemPrefab;
    [SerializeField]
    private ItemSceneConfig itemSceneConfig;
    [SerializeField]
    private GameObject loadingScreen;
    [SerializeField]
    private GameObject addItemBtn;
    private BuildManager buildManager;
    private BuildItem buildItem;
    private bool waitToFirebaseInitialized;
    private NetworkManager networkManager;
    private bool syncStarted;
    private List<ItemLocal> itemsLocalList;

    private void Awake()
    {
        buildItem = GetComponent<BuildItem>();
        buildManager = GetComponent<BuildManager>();
        CheckReferences();
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
            if (CheckDependenciesInitialize())
            {
                waitToFirebaseInitialized = false;
                LoadLocalData();
            }
        }
    }

    // cargamos la base de datos local primero, y si hay internet luego la remota
    private async void LoadLocalData()
    {

        List<ItemLocal> itemsLocal = 
            await MyApplication.repository.GetLocalItemsAsync();

        this.itemsLocalList = itemsLocal;

        List<Task> tasks = new List<Task>(); // Lista para almacenar tareas asíncronas

        Debug.Log("tamanio item local " + itemsLocalList.Count);
        foreach (ItemLocal itemLocal in itemsLocalList)
        {
            Task task = CreateItemInScene(itemLocal);
            tasks.Add(task);
        }
        await Task.WhenAll(tasks);

        itemSceneConfig.OrderAllItemPositionInScene();
        // 2. CARGAR LAS COPIAS DESDE EL JSON
        // (Solo si  ya en escena, los mains items, que se argaron en el for de arriba)
        // Esto se dispara una sola vez al inicio
        if (!syncStarted)
        {
           await buildManager.LoadWorld();
        }
        SetLoadingMsj(false); // Ocultar Cargando..
        StartCoroutine(CheckInternetConection());
    }

    /// <summary>
    /// Primero nos fijamos si tenemos conexion a internet, luego nos conectamos
    /// a la base de datos remota. Para luego sincronizar los cambios.
    /// </summary>
    /// <returns></returns>
    IEnumerator CheckInternetConection()
    {
        yield return new WaitForSeconds(1.0f);

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
            DisableBtnAddItem(false);
            ListeningDbRemote();
        }
        else
        {
            DisableBtnAddItem(true);
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
    /// Sincronizamos datos remotos con los datos locales.
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

        if (!isSomeListDbEquals)
        {

            List<ItemUpdate> itemListUpdates =
                     CheckUpdates.CheckUpdatesItems(itemsRemoteList, itemsLocalList);

            Debug.Log("itemListUpdates: " + itemListUpdates.Count);

            foreach (ItemUpdate itemToUpdate in itemListUpdates)
            {

                Task task = Task.CompletedTask; // Inicializar una tarea completada

                
                if (itemToUpdate.IsImageUpdated)
                {
                    ItemLocal itemLocalUptated = itemsRemoteList.Find(item => item.Id.Equals(itemToUpdate.Id))
                        .ItemRemoteToItemLocal();
                    ItemLocal itemOld = itemsLocalList.Find(item => item.Id.Equals(itemToUpdate.Id));
                    DeleteOldImage(itemOld.ImageName);
                    task = UpdateItemInScene(item: itemLocalUptated, isImageUpdate: true);
                    itemsToSave.Add(itemLocalUptated);
                }
                else if (itemToUpdate.IsRemove)
                {
                    ItemLocal itemLocalToDelete = itemsLocalList.Find(item => item.Id.Equals(itemToUpdate.Id));
                    DeleteItemInScene(itemLocalToDelete);
                    DeleteOldImage(itemLocalToDelete.ImageName);
                    itemSceneConfig.DeleteOldGameObjectItem(itemToUpdate.Id);
                }
                else if (itemToUpdate.IsAdd)
                {
                    // Nuevo item añadido
                    ItemLocal itemLocalToAdd =
                        itemsRemoteList.Find(item => item.Id.Equals(itemToUpdate.Id))
                        .ItemRemoteToItemLocal();
                    task = CreateItemInScene(itemLocalToAdd);
                    itemsToSave.Add(itemLocalToAdd);
                }
                else
                {
                    // es necesario agregar los que no fueron cambiados tambien, 
                    // ya que sobreescribiremos la base de datos local.
                    // sin cambios el ítem local con el ítem remoto
                    ItemLocal itemLocal = itemsLocalList.Find(item => item.Id.Equals(itemToUpdate.Id));
                    task = CreateItemInScene(itemLocal);
                    itemsToSave.Add(itemLocal);
                }
                tasks.Add(task); // Agregar la tarea a la lista de tareas*/
            }

            // Esperar a que todas las tareas se completen
            await Task.WhenAll(tasks);

            OrderItem();

            // nueva lista para saber en la base de datos local
            itemsLocalList = itemsToSave;
            await MyApplication.repository.SaveLocalItemsAsync(itemsLocalList);
        }

        syncStarted = false; // para que no se vuelva a pisar valores
                             // si se cambia mientras se esta cargando
    }

    /// <summary>
    /// Cuando tenemos la base de datos local vacía, lo que hacemos es ordenar los items
    /// uno al lado del otro, no al frente del jugador, 
    /// en cambio si hay alguna actualización si 
    /// </summary>
    private void OrderItem()
    {
        // Si borramos los datos o si abrimos la app en otro dispositivo
        if (itemsLocalList.Count == 0)
        {
            itemSceneConfig.OrderAllItemPositionInScene();
        }
        else
        {
            itemSceneConfig.OrderSomeItemPositionInScene(CheckUpdates.GetItemsChanged());
        }
    }

    private void DeleteOldImage(string oldImageName)
    {
        FileManager fileManager = new FileManager(FirebaseSDK.GetInstance().auth.CurrentUser.UserId);
        fileManager.DeleteOldImageLocalImage(oldImageName);
    }

    private async Task<bool> CreateItemInScene(ItemLocal item)
    {
        if (itemPrefab != null)
        {
            // 1. CREAR EL MAIN ITEM (El original con luz tenue)
            if (itemSceneConfig.transform.Find(item.Id) == null)
            {
                GameObject mainItem = Instantiate(itemPrefab);
                mainItem.transform.position = itemSceneConfig.transform.position;
                mainItem.name = item.Id;

                // AGREGAR LUZ AL MAIN (Para diferenciarlo)
                Light mainLight = mainItem.AddComponent<Light>();
                mainLight.range = 3f;
                mainLight.intensity = 0.5f;
                mainLight.color = Color.cyan;

                await buildItem.AsignMaterialAsync(item.ImageName, mainItem);
                itemSceneConfig.SetItemGameObject(mainItem);
            }
        }
        return true;
    }


    private async Task<bool> UpdateItemInScene(ItemLocal item, bool isImageUpdate)
    {
        GameObject mainItem = GameObject.Find(item.Id);
        if (mainItem != null && isImageUpdate)
        {
            // Actualizar el Main
            await buildItem.AsignMaterialAsync(item.ImageName, mainItem);

            // ACTUALIZAR TODAS LAS COPIAS (Gemini: Sincronización de clones)
            await buildManager.UpdateAllClonesTexture(item.Id, item.ImageName);
        }
        return true;
    }

    private void DeleteItemInScene(ItemLocal item)
    {
        // Si borramos el ítem de Firebase, es el fin de su existencia.
        // Gemini: Borramos el original Y todas sus copias locales.

        // 1. Borrar Main
        GameObject mainItem = GameObject.Find(item.Id);
        if (mainItem != null) Destroy(mainItem);

        // 2. Borrar todos los clones del mundo construido
        buildManager.DeleteAllClonesOfId(item.Id);
    }

    private void SetLoadingMsj(bool isActive)
    {
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(isActive);
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
        if (itemLocals.Count == 0 && itemRemotes.Count == 0) return true;

        bool isSameContent = true;

        for (int i = 0; i < itemLocals.Count; i++)
        {
            ItemRemote itemRemote =
                itemRemotes.Find(item => item.Id.Equals(itemLocals[i].Id));
            isSameContent = isSameContent && ItemExtensions.IsSameContent(itemLocals[i], itemRemote);
        }
        return isSameContent;
    }

    private bool CheckDependenciesInitialize()
    {
        Debug.Log(" MyApplication.repository " + MyApplication.repository != null);
        Debug.Log(" irebaseSDK.GetInstance().isFirebaseReady " + FirebaseSDK.GetInstance().isFirebaseReady);
       //Debug.Log("FirebaseSDK.GetInstance().auth.CurrentUser.IsAnonymous " + FirebaseSDK.GetInstance().user.IsAnonymous);

        return
                  MyApplication.repository != null &&
                  FirebaseSDK.GetInstance().isFirebaseReady &&
                  FirebaseSDK.GetInstance().user != null;
    }

    // Cuando no tenemos conexion a internet, no podemos añadir items.
    private void DisableBtnAddItem(bool isEnable)
    {
        if (addItemBtn != null)
        {
            addItemBtn.SetActive(!isEnable);
        }
    }

    private void CheckReferences() {

        if(itemPrefab == null) { Debug.LogWarning("Por favor, coloca el ItemPrefab en el ManageItems.cs en el inspector"); }
        if (itemSceneConfig == null) { Debug.LogWarning("Por favor, coloca el itemSceneConfig en el ManageItems.cs en el inspector"); }
        if (loadingScreen == null) { Debug.LogWarning("Por favor, coloca el loadingScreen en el ManageItems.cs en el inspector"); }
        if (addItemBtn == null) { Debug.LogWarning("Por favor, coloca el addItemBtn en el ManageItems.cs en el inspector"); }
        if (buildItem == null) { Debug.LogWarning("Por favor, coloca el BuildItem.cs en el inspector"); }
    }
}
