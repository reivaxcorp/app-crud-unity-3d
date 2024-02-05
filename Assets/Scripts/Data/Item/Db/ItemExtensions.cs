using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemExtensions 
{
    public static ItemLocal ItemRemoteToItemLocal(this ItemRemote itemRemote)
    {
        return new ItemLocal
        {
            // Realiza la conversión de los campos según sea necesario
            Id = itemRemote.Id,
            Name = itemRemote.Name, 
            Path = itemRemote.Path,
            ImageIdMetadata = itemRemote.ImageIdMetadata,
            CreationDate = itemRemote.CreationDate
        };
    }

    public static List<ItemLocal> ItemsRemoteToItemLocal(this List<ItemRemote> itemsRemote)
    {
        List<ItemLocal> itemsLocal = new List<ItemLocal>();

        foreach (var remoteItem in itemsRemote)
        {
            ItemLocal localItem = new ItemLocal
            {
                // Realiza la conversión de los campos según sea necesario
                Id = remoteItem.Id,
                Name = remoteItem.Name, 
                Path = remoteItem.Path,
                ImageIdMetadata = remoteItem.ImageIdMetadata,
                CreationDate = remoteItem.CreationDate
            };

            itemsLocal.Add(localItem);
        }

        return itemsLocal;
    }
}
