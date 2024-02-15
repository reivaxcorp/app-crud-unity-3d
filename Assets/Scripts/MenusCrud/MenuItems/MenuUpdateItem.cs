using System;
using UnityEngine;

public class MenuUpdateItem : MenuCrud
{
    private string itemSelectedId;

    public void InitMenu(string itemId)
    {
        if (MyApplication.repository != null)
        {
            try
            {
                this.itemSelectedId = itemId;
                ItemLocal itemLocal = MyApplication.repository.GetLocalItemById(itemId);
                SetImageName(itemLocal.Name);
                Texture2D texture2D = MyApplication.repository.LoadTextureAsPNG(itemLocal.ImageName);
                SetImagePreview(texture2D);

            }catch(Exception exception)
            {
                Debug.LogError("Error en la textura o el ítem no existe! " + exception);
            }
        }
        else
        {
            Debug.LogWarning("El repositorio es null");
        }
    }

    public void UpdateItem()
    {

    }
}
