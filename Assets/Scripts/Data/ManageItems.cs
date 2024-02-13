using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class ManageItems : MonoBehaviour
{

    [SerializeField]
    private GameObject itemPrefab;
    [SerializeField]
    private GameObject myItemsOrdered;
    [SerializeField]
    private GameObject loadingMsj;
    private BuildItem buildItem;
    private bool waitToFirebaseInitialized;
    private NetworkManager networkManager;

    private void Awake()
    {
        buildItem = gameObject.AddComponent<BuildItem>();
    }

    // Start is called before the first frame update
    void Start()
    {
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
                FirebaseSDK.GetInstance().db != null)
            {
                waitToFirebaseInitialized = false;

                CheckInternetConection();
            }
        }
    }

    private void CheckInternetConection()
    {
        this.networkManager = GetComponent<NetworkManager>();

        if (networkManager != null)
        {
            networkManager.handleInternetAvariableResult += ReadData;
            networkManager.ListeningInternetAvariable();
        } else
        {
            Debug.LogWarning("NetworManager.cs no esta en el Manager");
        }
    }

    private void ReadData(bool isInternetAvariable)
    {
        if(isInternetAvariable)
        {
            ListeningDbRemote();
        } else
        {
            SyncronizeData(null);
        }
    }

    // Nos suscribimos a un evento y llamamos al metodo que lanzara el evento.
    private void ListeningDbRemote()
    {
        RemoteDb remoteDbRef = MyApplication.repository.GetRemoteDb();
        remoteDbRef.handleValueResult += SyncronizeData;
        remoteDbRef.FirebaseValueChanged();
    }

    /// <summary>
    /// Sincronizamos datos guardados con los datos locales.
    /// </summary>
    /// <param name="itemsRemoteList">La lista con el que se realizará la operación. Puede ser null.</param>
    private async void SyncronizeData(List<ItemRemote> itemsRemoteList)
    {
        
        List<ItemLocal> itemsLocalList = MyApplication.repository.GetLocalItems();
        List<ItemLocal> itemsToSave = new List<ItemLocal>();

        List<Task> tasks = new List<Task>(); // Lista para almacenar tareas asíncronas

        // Estamos con conexión a internet.
        if (itemsRemoteList != null)
        {
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
                    ItemLocal itemLocal = itemsRemoteList.Find(item => item.Id == itemToUpdate.Id)
                        .ItemRemoteToItemLocal();
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
            OrderItem(itemsToSave);
            // actualizamos la lista local con la remota
            MyApplication.repository.SaveLocalItems(itemsToSave);
        }
        else
        {
            // Estamos sin sin conexión a internet, cargamos los datos locales
            foreach (ItemLocal itemLocal in itemsLocalList)
            {
                Task task = Task.CompletedTask;
                task = CreateItemInScene(itemLocal);
                tasks.Add(task);
            }

            // Esperar a que todas las tareas se completen
            await Task.WhenAll(tasks);
            OrderItem(itemsLocalList);
        }

        SetLoadingMsj(false);
    }

    private async Task<bool> CreateItemInScene(ItemLocal item)
    {
        if (itemPrefab != null)
        {
            if (myItemsOrdered.transform.Find(item.Id) == null)
            {
                {
                    GameObject itemToCreate = Instantiate(itemPrefab);
                    itemToCreate.GetComponentInChildren<TextMeshPro>().text = item.Name;
                    itemToCreate.name = item.Id;
                    await buildItem.AsignMaterialAsync(item.Id, item.Path, itemToCreate);
                }
            } else
            {
                Debug.LogWarning("Ítem no encontrado " + item.Id);
            }
        }
        else
        {
            Debug.LogWarning("Por favor, coloca la referencia del item prefab en el ispector");
            return false;
        }
        return true;
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
        if(loadingMsj != null)
        {
            loadingMsj.SetActive(isActive);
        } else
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
        RemoteDb remoteDbRef = MyApplication.repository.GetRemoteDb();
        if (remoteDbRef != null)
            remoteDbRef.handleValueResult -= SyncronizeData;
        if(networkManager != null)
            networkManager.handleInternetAvariableResult -= ReadData;
    }
}
