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

    // Acción del botón "Crear item"
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
                SetResultCrudUi(false, "Error - " + excepcion.Message);
            }
        }
    }

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
