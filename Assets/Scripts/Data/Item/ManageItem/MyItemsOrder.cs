using System.Collections.Generic;
using UnityEngine;

public class MyItemsOrder : MonoBehaviour
{
    [SerializeField] GameObject item;
    // Start is called before the first frame update
    
    public void OrderItemPositionInScene(List<ItemLocal> itemlocals)
    {
        float nextPositionX = this.transform.position.x;

        if(item != null)
        {
            for (int index = 0; index < itemlocals.Count; index++)
            {
                GameObject itemInScene = GameObject.Find(itemlocals[index].Id);

                if (itemInScene != null)
                {
                    //itemInScene.transform.position = this.transform.position;
                    itemInScene.transform.SetParent(this.transform);
                    itemInScene.transform.position =
                        new Vector3(nextPositionX, transform.position.y, transform.position.z);
                    Renderer rendeder = itemInScene.GetComponent<Renderer>();
                    nextPositionX += rendeder.bounds.size.x * 2;
                } else
                {
                    Debug.LogWarning("El ítem no existe enla escena!");
                }
            }
        } else
        {
            Debug.LogWarning("Por favor coloca el item prefab (item) en el inspector");
        }
      
    }
}
