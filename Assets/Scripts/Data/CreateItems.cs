using System;
using System.Collections.Generic;
using UnityEngine;

public class CreateItems : MonoBehaviour
{
    private bool readDataFirebase;
    [SerializeField]
    private GameObject item;
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

        if(itemsLocalList == null && itemsRemoteList == null) { return; }

        if (itemsLocalList != null)
        {
            Tuple<List<ItemLocal>, List<ItemLocal>, List<ItemLocal>> crudList =
                CheckUpdates.CheckUpdatesItems(itemsRemoteList, itemsLocalList);

            List<ItemLocal> itemsToAdd = crudList.Item1;
            List<ItemLocal> itemsToUpdate = crudList.Item2;
            List<ItemLocal> itemsToDelete = crudList.Item3;

            if(itemsToAdd.Count > 0)
            {
                itemsUplodated.AddRange(itemsToAdd);
            }

            // si hay algun item que se actualizó, removemos la textura
            // no importa si solo se cambio el nombre.
            foreach (ItemLocal itemToUpdate in itemsToUpdate)
            {
                ItemLocal oldItemToUpdate =
                     itemsLocalList.Find(i => i.Id.Equals(itemToUpdate.Id));

                if (oldItemToUpdate != null)
                {
                    MyApplication.repository.RemoveTexture(oldItemToUpdate.Id);
                    itemsUplodated.Add(itemToUpdate);
                }
            }

            foreach (ItemLocal itemToDelete in itemsToDelete)
            {

                ItemLocal oldItemToDelete =
                    itemsLocalList.Find(i => i.Id.Equals(itemToDelete.Id));
                if(oldItemToDelete != null)
                {
                    MyApplication.repository.RemoveTexture(oldItemToDelete.Id);
                }
            }

            CreateItem(itemsUplodated);
            MyApplication.repository.SaveItemsLocal(itemsUplodated);
        }
        else
        {

        }
    }

    private async void CreateItem(List<ItemLocal> items)
    {
        if (item !=  null)
        {
            foreach (ItemLocal itemLocal in items)
            {
                GameObject itemToCreate = Instantiate(item);
                itemToCreate.name = itemLocal.Name;
                await buildItem.AsignMaterialAsync(itemLocal.Id, itemToCreate, itemLocal.Path);
            }
        } else
        {
            Debug.LogWarning("Por favor, coloca la referencia del item prefab en el ispector");
        }
        
    }
}
