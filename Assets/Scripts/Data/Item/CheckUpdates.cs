using System;
using System.Collections.Generic;
using UnityEngine;

public class CheckUpdates : MonoBehaviour
{

    /// <summary>
    /// Verificamos los valores a actualizar y eliminar y aniadir, de la base de datos local
    /// </summary>
    /// <param name="itemsRemoteList">Lista de realtime database</param>
    /// <param name="itemsLocalList">Lista previamente guardada</param>
    /// <returns>
    /// Primera lista de la tupa: Items a añadir 
    /// Segunda lista de la tupla: Items a actualizar,
    /// Tercera lista de la tupla: Items a eliminar
    /// </returns>
    public static Tuple<List<ItemLocal>, List<ItemLocal>, List<ItemLocal>> CheckUpdatesItems(
       List<ItemRemote> itemsRemoteList,
       List<ItemLocal> itemsLocalList
       )
    {
        List<ItemLocal> itemToAdd = new List<ItemLocal>();
        List<ItemLocal> itemToUpdate = new List<ItemLocal>();
        List<ItemLocal> itemToRemove = new List<ItemLocal>();

        foreach (ItemLocal itemLocal in itemsLocalList)
        {
            // si no esta en el remoto, debemos removerlo
            ItemRemote itemRemote =
                itemsRemoteList.Find(p => p.Id.Equals(itemLocal.Id));

            if (itemRemote == null)
            {
                // lo agregamos a items para remover ya que no estan en el remoto
                itemToRemove.Add(itemLocal);
            }
            // si ha cambiado debemos actualizarlo
            else if (!IsItemUpdated(itemRemote, itemLocal))
            {
                itemToUpdate.Add(itemRemote.ItemRemoteToItemLocal());
            }
        }

        // agregamos los productos que faltan
        foreach (ItemRemote itemRemote in itemsRemoteList)
        {
            ItemLocal itemToSaveLocal =
              itemsLocalList.Find(p => p.Id.Equals(itemRemote.Id));

            if (itemToSaveLocal == null)
            {
                itemToAdd.Add(itemRemote.ItemRemoteToItemLocal());
            }
        }

        return new Tuple<List<ItemLocal>,
                         List<ItemLocal>,
                         List<ItemLocal>>(
                         itemToUpdate,
                         itemToRemove,
                         itemToAdd);
    }



    /// <summary>
    /// Si el producto cambio, necesitamos bajarlo de nuevo
    /// </summary>
    /// <param name="itemRemote"></param>
    /// <param name="itemLocal"></param>
    /// <returns></returns>
    private static bool IsItemUpdated(ItemRemote productRemote, ItemLocal itemLocal)
    {
        return productRemote.Timestamp == itemLocal.Timestamp;
    }

}
