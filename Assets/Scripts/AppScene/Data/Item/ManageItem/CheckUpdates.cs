/*********************************************************************************
 * Nombre del Archivo:     CheckUpdates.cs
 * Descripción:            Verificará si hay algun cambio con los items remotos con los locales.
 *                         Enviara una lista del tipo ItemUpdate a nuestro ManageItems.cs, que contendra 
 *                         los ítems a actualizar,  ó no. Si hay algún ítem nuevo que debamos agregar,
 *                         esta clase nos dará su id y bajaremos los recursos necesarios
 *                         para crear ese ítem en nuestra aplicación.  
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

public class CheckUpdates
{
    private static List<ItemRemote> listItemsChanged = new List<ItemRemote>();

    /// <summary>
    /// Verificamos los valores a actualizar y eliminar y aniadir, de la base de datos local
    /// </summary>
    /// <param name="itemsRemoteList">Lista de realtime database</param>
    /// <param name="itemsLocalList">Lista previamente guardada</param>
    /// <returns>
    /// Lista, con los items a actualizar, eliminar o crear.
    /// </returns>
    public static List<ItemUpdate> CheckUpdatesItems(
       List<ItemRemote> itemsRemoteList,
       List<ItemLocal> itemsLocalList
       )
    {
        listItemsChanged.Clear();

        List<ItemUpdate> itemUpdates = new List<ItemUpdate>();

        foreach (ItemLocal itemLocal in itemsLocalList)
        {

            ItemRemote itemRemote =
                itemsRemoteList.Find(p => p.Id.Equals(itemLocal.Id));

            // Se a removido un item de la base de datos
            if (itemRemote == null)
            {
                itemUpdates.Add(new ItemUpdate(
                    id: itemLocal.Id,
                    isImageUpdated: false,
                    isFieldsUpdated: false,
                    isRemove: true,
                    isAdd: false));
            }
            // Se ha cambiado los campos y la imagén
            else if (IsFielsUpdate(itemRemote, itemLocal) && IsImageUpdated(itemRemote, itemLocal))
            {
                itemUpdates.Add(new ItemUpdate(
                  id: itemLocal.Id,
                  isImageUpdated: true,
                  isFieldsUpdated: true,
                  isRemove: false,
                  isAdd: false));

                listItemsChanged.Add(itemRemote);
            }
            // Solo se han cambiado los campos
            else if (IsFielsUpdate(itemRemote, itemLocal))
            {
                itemUpdates.Add(new ItemUpdate(
                 id: itemLocal.Id,
                 isImageUpdated: false,
                 isFieldsUpdated: true,
                 isRemove: false,
                 isAdd: false));

                listItemsChanged.Add(itemRemote);
            }
            // Se ha cambiado la imagén
            else if (IsImageUpdated(itemRemote, itemLocal))
            {
                itemUpdates.Add(new ItemUpdate(
                id: itemLocal.Id,
                isImageUpdated: true,
                isFieldsUpdated: false,
                isRemove: false,
                isAdd: false));

                listItemsChanged.Add(itemRemote);
            }
            else
            {
                // sin cambios  el ítem como está.
                itemUpdates.Add(new ItemUpdate(
                id: itemLocal.Id,
                isImageUpdated: false,
                isFieldsUpdated: false,
                isRemove: false,
                isAdd: false));
            }
        }

        // Nuevos items a añadir
        foreach (ItemRemote itemRemote in itemsRemoteList)
        {
            ItemLocal itemLocal =
              itemsLocalList.Find(p => p.Id.Equals(itemRemote.Id));

            if (itemLocal == null)
            {
                itemUpdates.Add(new ItemUpdate(
                id: itemRemote.Id,
                isImageUpdated: false,
                isFieldsUpdated: false,
                isRemove: false,
                isAdd: true));

                listItemsChanged.Add(itemRemote);
            }
        }

        return itemUpdates;
    }

    /// <summary>
    /// Para obtener la lista que items se acabaron de actualizar, ignorando los que
    /// en realidad no han cambiado en nada.
    /// </summary>
    /// <returns></returns>
    public static List<ItemRemote> GetItemsChanged()
    {
        return listItemsChanged;
    }

    /// <summary>
    /// Si el producto cambio, necesitamos bajarlo de nuevo
    /// </summary>
    /// <param name="itemRemote"></param>
    /// <param name="itemLocal"></param>
    /// <returns></returns>
    private static bool IsFielsUpdate(ItemRemote itemRemote, ItemLocal itemLocal)
    {
        return !itemRemote.Name.Equals(itemLocal.Name);
    }

    // si cambia el el image_id_metadata, es probrable que cambie el nombre de la imagen, 
    // y si cambia a otra imagen cambia tambien el path.
    private static bool IsImageUpdated(ItemRemote itemRemote, ItemLocal itemLocal)
    {
        return !itemRemote.ImageName.Equals(itemLocal.ImageName);
    }

}
