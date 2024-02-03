using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAddItem : MenuCrud, IResultFile
{

    public void FileUploaded(bool isFileUploaded, string pathReference, string imagenIdMetadata)
    {
        if(isFileUploaded)
        {
            ItemRemote itemRemote = new ItemRemote();
            WriteDocument(pathReference, imagenIdMetadata);
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
            }
            catch (Exception excepcion)
            {
                SetResultCrudUi(false, "Error - " + excepcion.Message);
            }
        }
    }

    private void WriteDocument(string pathReference, string imagenIdMetadata)
    {
        if(MyApplication.repository != null)
        {
            ItemRemote itemRemote = new ItemRemote(name: inputFieldName.text, path: pathReference, imageIdMetadata: imagenIdMetadata);
            MyApplication.repository.SaveItemRemote(itemRemote, resultUi: this);
        } else
        {
            Debug.LogWarning("El repositorio es Null");
        }
    }
}
