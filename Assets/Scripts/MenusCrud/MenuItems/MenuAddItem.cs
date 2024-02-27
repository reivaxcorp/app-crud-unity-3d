using System;
using UnityEngine;

public class MenuAddItem : MenuCrud, IResult
{
    private string generateImageName;

    public void SetResultCrudUi(string title, string msj)
    {
        StartAnimationTextMenu(false, "");
        uiApp.MenuSetActive(false);
        OpenDialog(title, msj);
    }

    /// <summary>
    /// Cuando colocamos subir ítem, lo primero que hacemos es subir la imagén, luego escribimos
    /// los datos en realtime database, con WriteDocument, si la subida se realizó correctamente.
    /// </summary>
    public async void CreateDocumentRemote()
    {
        if (IsDataSetted())
        {
            StartAnimationTextMenu(true, "Creando");
            // Obtenemos los bytes de la imagén temporal seleccionada
            byte[] fileBytes = fileManager.GetBytesImageSelected();
            // Generar nombre de imagén aleatorea
            generateImageName = Guid.NewGuid().ToString();
            ManageStorageRemote manageStorageRemote =
                new ManageStorageRemote(generateImageName, fileManager.folderNameUser, fileBytes);
            // subir nueva imagén
            await manageStorageRemote.UploadFileFirebaseStorage();
            fileManager.ChangeNameImageCopySelected(generateImageName);
            WriteDocumentRemote(generateImageName);
        }
    }

    /// <summary>
    /// Una vez subida la imagén, procedemos a escribir el documento.
    /// </summary>
    /// <param name="imageName"></param>
    private void WriteDocumentRemote(string imageName)
    {
        if (MyApplication.repository != null)
        {
            ItemRemote itemRemote = new ItemRemote(name: inputFieldName.text, imageName: imageName);
            MyApplication.repository.SaveItemRemote(itemRemote, resultUi: this);
            // Invoke("ShowInterstitialAd", 3f);
        }
        else
        {
            Debug.LogWarning("El repositorio es Null");
        }
    }

}
