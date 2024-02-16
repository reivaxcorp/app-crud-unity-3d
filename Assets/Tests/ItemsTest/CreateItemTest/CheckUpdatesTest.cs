using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckUpdatesTest
{
    /// <summary>
    /// Verificamos los valores a actualizar y eliminar y aniadir, de la base de datos local
    /// </summary>
    /// <param name="itemsRemoteList">Lista de realtime database</param>
    /// <param name="itemsLocalList">Lista previamente guardada</param>
    /// <returns>
    /// Lista, con los items a actualizar, eliminar o crear.
    /// </returns>
    public static List<ItemUpdateTest> CheckUpdatesItems(
       List<ItemRemoteTest> itemsRemoteList,
       List<ItemLocalTest> itemsLocalList
       )
    {
        List<ItemUpdateTest> itemUpdates = new List<ItemUpdateTest>();

        // Comprobar si hay que sincronizar algún ítem local con el ítem remoto.
        foreach (ItemLocalTest itemLocal in itemsLocalList)
        {

            ItemRemoteTest itemSavedLocal =
                itemsRemoteList.Find(p => p.Id.Equals(itemLocal.Id));

            // Se a removido un item de la base de datos
            if (itemSavedLocal == null)
            {
                itemUpdates.Add(new ItemUpdateTest(
                    id: itemLocal.Id,
                    isImageUpdated: false,
                    isFieldsUpdated: false,
                    isRemove: true,
                    isAdd: false));
            }
            // Se ha cambiado los campos y la imagén
            else if (IsFielsUpdate(itemSavedLocal, itemLocal) && IsImageUpdated(itemSavedLocal, itemLocal))
            {
                itemUpdates.Add(new ItemUpdateTest(
                  id: itemLocal.Id,
                  isImageUpdated: true,
                  isFieldsUpdated: true,
                  isRemove: false,
                  isAdd: false));
            }
            // Solo se han cambiado los campos
            else if (IsFielsUpdate(itemSavedLocal, itemLocal))
            {
                itemUpdates.Add(new ItemUpdateTest(
                 id: itemLocal.Id,
                 isImageUpdated: false,
                 isFieldsUpdated: true,
                 isRemove: false,
                 isAdd: false));
            }
            // Se ha cambiado la imagén
            else if (IsImageUpdated(itemSavedLocal, itemLocal))
            {

                itemUpdates.Add(new ItemUpdateTest(
                id: itemLocal.Id,
                isImageUpdated: true,
                isFieldsUpdated: false,
                isRemove: false,
                isAdd: false));
            }
            else
            {
                // sin cambios  el ítem como está.
                itemUpdates.Add(new ItemUpdateTest(
                id: itemLocal.Id,
                isImageUpdated: false,
                isFieldsUpdated: false,
                isRemove: false,
                isAdd: false));
            }
        }

        // Comprobar si hay nuevos items a añadir.
        foreach (ItemRemoteTest itemRemote in itemsRemoteList)
        {
            ItemLocalTest itemToSaveLocal =
              itemsLocalList.Find(p => p.Id.Equals(itemRemote.Id));

            if (itemToSaveLocal == null)
            {
                itemUpdates.Add(new ItemUpdateTest(
                id: itemRemote.Id,
                isImageUpdated: false,
                isFieldsUpdated: false,
                isRemove: false,
                isAdd: true));
            }
        }

        return itemUpdates;
    }


    /// <summary>
    /// Si el producto cambio, necesitamos bajarlo de nuevo
    /// </summary>
    /// <param name="itemRemote"></param>
    /// <param name="itemLocal"></param>
    /// <returns></returns>
    private static bool IsFielsUpdate(ItemRemoteTest productRemote, ItemLocalTest itemLocal)
    {
        return !productRemote.Name.Equals(itemLocal.Name);
    }

    // si cambia el el image_id_metadata, es probrable que cambie el nombre de la imagen, 
    // y si cambia a otra imagen cambia tambien el path.
    private static bool IsImageUpdated(ItemRemoteTest productRemote, ItemLocalTest itemLocal)
    {
        return !productRemote.ImageName.Equals(itemLocal.ImageName);
    }
}
