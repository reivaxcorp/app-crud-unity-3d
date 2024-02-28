/*********************************************************************************
 * Nombre del Archivo:     ManageItems.cs
 * Descripción:            En primer lugar, cargamos los ítems locales del usuario guardados 
 *                         en su dispositivo (si los hay), luego verificamos si hay algún cambio en la base 
 *                         de datos de RealtimeDatabase remota, y si los hay, actualizamos y volvemos a 
 *                         guardar los datos actualizados en el dispositivo. A su vez también escuchamos
 *                         los cambios generados en tiempo real de la base de datos de Firebase RealtimeDatabase.
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
    private GameObject myItemsOrdered;
    [SerializeField]
    private GameObject loadingScreen;
    [SerializeField]
    private GameObject addItemBtn;
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

        List<ItemLocal> itemsLocal = await MyApplication.repository.GetLocalItemsAsync();

        this.itemsLocalList = itemsLocal;

        List<Task> tasks = new List<Task>(); // Lista para almacenar tareas asíncronas

        foreach (ItemLocal itemLocal in itemsLocalList)
        {
            Task task = CreateItemInScene(itemLocal);
            tasks.Add(task);
        }
        await Task.WhenAll(tasks);

        OrderItem(itemsLocalList);
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

        Debug.Log("Is somoe list equals " + isSomeListDbEquals + " " +
       " Lista local " +
       itemsLocalList.Count + " lista remota " + itemsRemoteList.Count);


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
                    DeleteOldImage(itemOld.ImageName);
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
                    DeleteOldImage(itemOld.ImageName);
                    task = UpdateItemInScene(item: itemLocalUptated, isFieldUpdate: false, isImageUpdate: true);
                    itemsToSave.Add(itemLocalUptated);
                }
                else if (itemToUpdate.IsRemove)
                {
                    ItemLocal itemLocalToDelete = itemsLocalList.Find(item => item.Id.Equals(itemToUpdate.Id));
                    DeleteItemInScene(itemLocalToDelete);
                    DeleteOldImage(itemLocalToDelete.ImageName);
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
                    // es necesario agregar los que no fueron cambiados tambien
                    // sin cambios el ítem local con el ítem remoto
                    ItemLocal itemLocal = itemsLocalList.Find(item => item.Id.Equals(itemToUpdate.Id));
                    task = CreateItemInScene(itemLocal);
                    itemsToSave.Add(itemLocal);
                }
                tasks.Add(task); // Agregar la tarea a la lista de tareas*/
            }

            // Esperar a que todas las tareas se completen
            await Task.WhenAll(tasks);
            OrderItem(itemsToSave);
            // actualizamos la lista local con la remota

            itemsLocalList = itemsToSave;
            await MyApplication.repository.SaveLocalItemsAsync(itemsLocalList);
        }

        syncStarted = false;
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
            if (myItemsOrdered.transform.Find(item.Id) == null)
            {
                {
                    GameObject itemToCreate = Instantiate(itemPrefab);
                    itemToCreate.transform.position = myItemsOrdered.transform.position;

                    TextMeshPro[] textMeshProChildren = itemToCreate.GetComponentsInChildren<TextMeshPro>();

                    if (textMeshProChildren.Length == 2 && textMeshProChildren[0] != null && textMeshProChildren[1] != null)
                    {
                        textMeshProChildren[0].text = item.Name;
                        textMeshProChildren[1].text = "Creado:\n" + TimeUtils.ConvertTimeStampUnixToDate(item.CreationDate);
                    }

                    itemToCreate.name = item.Id;
                    await buildItem.AsignMaterialAsync(item.ImageName, itemToCreate);
                }
            }
        }
        else
        {
            Debug.LogWarning("Por favor, coloca la referencia del item prefab en el ispector");
            return false;
        }
        return true;
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

    private bool CheckDependenciesInitialize()
    {
        return    
                  MyApplication.repository != null &&
                  FirebaseSDK.GetInstance().isFirebaseReady &&
                  FirebaseSDK.GetInstance().auth.CurrentUser != null;
    }

    // Cuando no tenemos conexion a internet, no podemos añadir items.
    private void DisableBtnAddItem(bool isEnable)
    {
        if(addItemBtn != null)
        {
            addItemBtn.SetActive(!isEnable);
        } else
        {
            Debug.LogWarning("Por favor, coloca el botón añadir item (addItemBtn) en el inspector del Manager");
        }
    }
}
