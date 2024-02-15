using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUpdateItem : MenuCrud
{
    /*[SerializeField] private GameObject closeMenuBtn;
    [SerializeField] private GameObject imageSelectBtn;
    [SerializeField] private GameObject imagePreview;
    [SerializeField] private GameObject inputImageName;
    [SerializeField] private GameObject deleteItemBtn;*/

    public void InitMenu(string idItem)
    {
        if (MyApplication.repository != null)
        {
            try
            {
                Texture2D texture2D = MyApplication.repository.LoadTextureAsPNG(idItem);
                SetImagePreview(texture2D);
                ItemLocal itemLocal = MyApplication.repository.GetLocalItemById(idItem);
                SetImageName(itemLocal.Name);
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

    private void ClearMenu()
    {

    }
}
