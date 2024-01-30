using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAddItem : MenuCrud, IResultFile
{

    public void FileUploaded(bool isFileUploaded, string pathReference)
    {
        if(isFileUploaded)
        {
            WriteDocument(pathReference);
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

    private void WriteDocument(string pathReference)
    {
        MyApplication.repository.SaveItemRemote(itemName: inputFieldName.text, remoteFilePath: pathReference, resultUi: this);
    }
}
