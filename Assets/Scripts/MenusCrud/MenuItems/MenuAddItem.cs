using System;
using UnityEngine;

public class MenuAddItem : MenuCrud
{

    /// <summary>
    /// Cuando colocamos subir ítem, lo primero que hacemos es subir la imagén, luego escribimos
    /// los datos en realtime database, con WriteDocument, si la subida se realizó correctamente.
    /// </summary>
    public async void OnUploadItem()
    {
        if (IsDataSetted())
        {
            try
            {
                progressText?.StartProgressTextAnimation("Subiendo", resultMsj);
                byte[] fileBytes = fileManager.GetBytesImageSelected();
                UploadFileRemote uploadFileRemote = new UploadFileRemote(fileBytes, fileManager.folderUidName, iResult: this);
                bool resultUpload = await uploadFileRemote.UploadFileFirebaseStorage();
                if (resultUpload) { 
                    WriteDocument(uploadFileRemote.newImageName); 
                }
            }
            catch (Exception excepcion)
            {
                SetResultCrudUi(EResultMenuAction.Failed, "Error - " + excepcion.Message);
            }
        }
    }

    /// <summary>
    /// Una vez subida la imagén, procedemos a escribir el documento.
    /// </summary>
    /// <param name="imageName"></param>
    private void WriteDocument(string imageName)
    {
        if(MyApplication.repository != null)
        {
            ItemRemote itemRemote = new ItemRemote(name: inputFieldName.text, imageName: imageName);
            MyApplication.repository.SaveItemRemote(itemRemote, resultUi: this);
         
            Invoke("ShowInterstitialAd", 3f);
        } else
        {
            Debug.LogWarning("El repositorio es Null");
        }
    }
}
