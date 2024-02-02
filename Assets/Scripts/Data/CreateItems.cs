using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateItems : MonoBehaviour
{
    private bool readDataFirebase;

    // Start is called before the first frame update
    void Start()
    {
        readDataFirebase = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(readDataFirebase)
        {
            if(FirebaseSDK.GetInstance().isFirebaseReady && MyApplication.repository != null)
            {
                readDataFirebase = false;
                RetrieveAndLogItems();
            }
        }
    }

    private async void RetrieveAndLogItems()
    {
       
            List<ItemRemote> items = await MyApplication.repository.GetProductsRemoteAsync();

            if (items != null)
            {
                foreach (var item in items)
                {
                    Debug.Log($"Item ID: {item.Id}, Name: {item.Name}, Path: {item.Path}, Timestamp: {item.Timestamp}");
                }
            }
            else
            {
                Debug.Log("No se obtuvieron items de la base de datos remota.");
            }

    }
}
