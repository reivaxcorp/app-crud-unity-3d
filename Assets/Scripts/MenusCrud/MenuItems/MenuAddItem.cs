using System;
using UnityEngine;

public class MenuAddItem : MenuCrud, IResultFile
{

    public void FileUploaded(bool isFileUploaded, string imageName)
    {
        if(isFileUploaded)
        {
            WriteDocument(imageName);
        }
    }

    /// <summary>
    /// Cuando colocamos subir ítem, lo primero que hacemos es subir la imagén, luego escribimos
    /// los datos en realtime database, con WriteDocument, si la subida se realizó correctamente.
    /// </summary>
    public void OnUploadItem()
    {
        if (IsDataSetted())
        {
            try
            {
                progressText?.StartProgressTextAnimation("Subiendo", resultMsj);
                byte[] fileBytes = fileManager.GetBytesImageSelected();
                UploadFileRemote uploadFileRemote = new UploadFileRemote(fileBytes, fileManager.folderUidName, inputFieldName.text, iResult: this, iFileResult: this);
                uploadFileRemote.UpoloadFileFirebaeStorage();
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
        } else
        {
            Debug.LogWarning("El repositorio es Null");
        }
    }
}
