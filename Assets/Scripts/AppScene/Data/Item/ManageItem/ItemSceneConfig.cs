/*********************************************************************************
 * Nombre del Archivo:     ItemSceneConfig.cs 
 * Descripción:            Es una clase que nos ayudará en colocar los ítems uno al lado del otro, 
 *                         evitando que se solapen, cada vez que hagamos las operaciones de añadir o borrar
 *                         algún item, esta clase reordenara los ítems en la escena.
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

using System.Collections.Generic;
using UnityEngine;

public class ItemSceneConfig : MonoBehaviour
{
    [SerializeField] GameObject item;
    [SerializeField] GameObject player;

    private List<GameObject> itemsGameObjects;

    private void Awake()
    {
        itemsGameObjects = new List<GameObject>();
    }

    public void SetItemGameObject(GameObject itemInScene)
    {
        itemsGameObjects.Add(itemInScene);
    } 

    /// <summary>
    /// Cuando leemos los datos de la base de datos local, debemos ordenar los ítems
    /// en la escena.
    /// </summary>
    public void OrderAllItemPositionInScene()
    {
        float nextPositionX = this.transform.position.x;

        if (item != null)
        {
            for (int index = 0; index < itemsGameObjects.Count; index++)
            {
                GameObject itemInScene = itemsGameObjects[index];

                if (itemInScene != null)
                {
                    SetItemParentToThis(itemInScene);
                    nextPositionX = TranslateItemSideBySide(itemInScene, nextPositionX);
                    EnablePhysicsItem(itemInScene);
                }
                else
                {
                    Debug.LogWarning("El ítem no existe enla escena!");
                }
            }
        }
        else
        {
            Debug.LogWarning("Por favor coloca el item prefab (item) en el inspector");
        }
    }

    /// <summary>
    /// Cuando un usuario actualiza o añade un nuevo ítem, debemos ordenarlos en la escena
    /// Pero dejar los que no fueron modificados en su lugar. 
    /// </summary>
    /// <param name="itemsRemoteListUpdated"></param>
    public void OrderSomeItemPositionInScene(List<ItemRemote> itemsRemoteListUpdated)
    {
        if (player == null)
        {
            Debug.LogWarning("La referencia Player no existe en ItemSceneConfig en el inspector, por favor colocala");
            return;
        }

        List<GameObject> itemsToOrder = new List<GameObject>();

        for (int index = 0; index < itemsRemoteListUpdated.Count; index++)
        {
            GameObject itemInScene =
                itemsGameObjects.Find(item => item.name.Equals(itemsRemoteListUpdated[index].Id));

            if (itemInScene != null)
            {
                itemsToOrder.Add(itemInScene);
            }
        }

        if (item != null)
        {
            for (int index = 0; index < itemsToOrder.Count; index++)
            {
                GameObject itemInScene = itemsToOrder[index];

                if (itemInScene != null)
                {
                    SetItemParentToThis(itemInScene);
                    TranslateItemFromOfPlayer(itemInScene);
                    EnablePhysicsItem(itemInScene);
                }
                else
                {
                    Debug.LogWarning("El ítem no existe enla escena!");
                }
            }
        }
        else
        {
            Debug.LogWarning("Por favor coloca el item prefab (item) en el inspector");
        }
    }

    /// <summary>
    /// Cuando eliminamos un ítem, también debemos eliminarlo de la lista de gameObjects.
    /// </summary>
    /// <param name="id"></param>
    public void DeleteOldGameObjectItem(string itemToDelete)
    {
        GameObject itemExists = itemsGameObjects.Find(item => item.name.Equals(itemToDelete));
        if (itemExists != null)
        {
            itemsGameObjects.Remove(itemExists);
        }
    }

    /// <summary>
    /// Traslada los ítems uno al lado del otro, cuando la aplicación empieza.
    /// </summary>
    /// <param name="itemInScene">GameObject item</param>
    /// <param name="nextPositionX">La próxima posición</param>
    /// <returns></returns>
    private float TranslateItemSideBySide(GameObject itemInScene, float nextPositionX)
    {
        float nextPos = nextPositionX;

        Renderer rendeder = itemInScene.GetComponent<Renderer>();
        itemInScene.transform.position =
            new Vector3(nextPositionX, transform.position.y, transform.position.z);
        nextPos += rendeder.bounds.size.x * 2;
        return nextPos;
    }

   /// <summary>
   /// Trasladamos el item, recién actualizado ó añadido, al lado del jugador.
   /// </summary>
   /// <param name="itemInScene"></param>
    private void TranslateItemFromOfPlayer(GameObject itemInScene)
    {
        Renderer rendeder = itemInScene.GetComponent<Renderer>();
       
        Vector3 currentPlayerPosition =
            player.GetComponent<Transform>().position;
        // colocar los ítems actualizados delante del jugador
        itemInScene.transform.position =
            new Vector3(
                currentPlayerPosition.x,
                currentPlayerPosition.y + rendeder.bounds.size.y,
                currentPlayerPosition.z + rendeder.bounds.size.z * 2
            );
    }

    private void SetItemParentToThis(GameObject itemInScene)
    {
        itemInScene.transform.SetParent(this.transform);
    }

    private void EnablePhysicsItem(GameObject itemInScene)
    {
        ItemScript itemScript = itemInScene.GetComponent<ItemScript>();
        if (itemScript != null)
        {
            itemScript.EnablePhysicsItem();
        }
    }

}
