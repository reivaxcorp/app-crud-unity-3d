using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemExtensionsTest
{
    public static ItemLocalTest ItemRemoteToItemLocal(this ItemRemoteTest itemRemote)
    {
        return new ItemLocalTest
        {
            // Realiza la conversión de los campos según sea necesario
            Id = itemRemote.Id,
            Name = itemRemote.Name,
            ImageName = itemRemote.ImageName,
            CreationDate = itemRemote.CreationDate
        };
    }

    public static List<ItemLocalTest> ItemsRemoteToItemLocal(this List<ItemRemoteTest> itemsRemote)
    {
        List<ItemLocalTest> itemsLocal = new List<ItemLocalTest>();

        foreach (var remoteItem in itemsRemote)
        {
            ItemLocalTest localItem = new ItemLocalTest
            {
                // Realiza la conversión de los campos según sea necesario
                Id = remoteItem.Id,
                Name = remoteItem.Name,
                ImageName = remoteItem.ImageName,
                CreationDate = remoteItem.CreationDate
            };

            itemsLocal.Add(localItem);
        }

        return itemsLocal;
    }
}
