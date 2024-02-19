using System.Collections.Generic;

public static class ItemExtensions
{
    public static ItemLocal ItemRemoteToItemLocal(this ItemRemote itemRemote)
    {
        return new ItemLocal
        {
            // Realiza la conversión de los campos según sea necesario
            Id = itemRemote.Id,
            Name = itemRemote.Name,
            ImageName = itemRemote.ImageName,
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
                ImageName = remoteItem.ImageName,
                CreationDate = remoteItem.CreationDate
            };

            itemsLocal.Add(localItem);
        }

        return itemsLocal;
    }

    public static bool IsSameContent(ItemLocal itemLocal, ItemRemote itemRemote)
    {
        if(itemLocal == null || itemRemote == null) return false;

        return
                itemLocal.Id.Equals(itemRemote.Id) &&
                itemLocal.Name.Equals(itemRemote.Name) &&
                itemLocal.ImageName.Equals(itemRemote.ImageName) && 
                itemLocal.CreationDate == itemRemote.CreationDate;
    }
}
