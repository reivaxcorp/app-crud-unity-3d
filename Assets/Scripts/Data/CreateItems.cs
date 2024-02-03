using System;
using System.Collections.Generic;
using UnityEngine;

public class CreateItems : MonoBehaviour
{
    private bool readDataFirebase;
    [SerializeField]
    private GameObject itemPrefab;
    private BuildItem buildItem;

    private void Awake()
    {
        buildItem = gameObject.AddComponent<BuildItem>();
    }

    // Start is called before the first frame update
    void Start()
    {
        readDataFirebase = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (readDataFirebase)
        {
            if (FirebaseSDK.GetInstance().isFirebaseReady && MyApplication.repository != null)
            {
                readDataFirebase = false;
                VerifyUpdates();
            }
        }
    }

    private async void VerifyUpdates()
    {

        List<ItemLocal> itemsLocalList = MyApplication.repository.GetItems();
        List<ItemRemote> itemsRemoteList = await MyApplication.repository.GetProductsRemoteAsync();

        List<ItemLocal> itemsUplodated = new List<ItemLocal>();

        if (itemsLocalList == null && itemsRemoteList == null) { return; }

        if (itemsLocalList != null)
        {
            List<ItemManager> itemUpdates =
                CheckUpdates.CheckUpdatesItems(itemsRemoteList, itemsLocalList);

            List<ItemLocal> itemsUpdated = new List<ItemLocal>();

            foreach (ItemManager itemToUpdate in itemUpdates)
            {

                if (itemToUpdate.IsFieldsUpdated && itemToUpdate.IsImageUpdated)
                {
                    ItemLocal itemLocal = itemsRemoteList.Find(item => item.Id == itemToUpdate.Id).ItemRemoteToItemLocal();
                    MyApplication.repository.RemoveTexture(itemToUpdate.Id);
                    itemsUpdated.Add(itemLocal);
                    CreateItem(itemLocal);
                }
                else if (itemToUpdate.IsFieldsUpdated)
                {
                    ItemLocal itemLocal = itemsRemoteList.Find(item => item.Id == itemToUpdate.Id).ItemRemoteToItemLocal();
                    itemsUpdated.Add(itemLocal);
                    CreateItem(itemLocal);
                }
                else if (itemToUpdate.IsImageUpdated)
                {
                    ItemLocal itemLocal = itemsRemoteList.Find(item => item.Id == itemToUpdate.Id).ItemRemoteToItemLocal();
                    MyApplication.repository.RemoveTexture(itemToUpdate.Id);
                    itemsUpdated.Add(itemLocal);
                    CreateItem(itemLocal);
                }
                else
                {

                    // Nuevo item añadido
                    ItemLocal itemLocal = itemsRemoteList.Find(item => item.Id == itemToUpdate.Id).ItemRemoteToItemLocal();
                    itemsUpdated.Add(itemLocal);
                    CreateItem(itemLocal);
                }
            }

            MyApplication.repository.SaveItemsLocal(itemsUplodated);
        }
        else
        {

        }
    }

    private async void CreateItem(ItemLocal item)
    {
        if (itemPrefab != null)
        {
            GameObject itemToCreate = Instantiate(itemPrefab);
            itemToCreate.name = item.Name;
            await buildItem.AsignMaterialAsync(item.Id, itemToCreate, item.Path);
        }
        else
        {
            Debug.LogWarning("Por favor, coloca la referencia del item prefab en el ispector");
        }
    }
}
