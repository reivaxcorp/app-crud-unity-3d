using System;
using UnityEngine;

/// <summary>
/// Comprobamos la actulización de los items cuando hacemos touch en alguno
/// Si el usuario eligio una nueva imagen se actualizará en cambio no lo hará
/// </summary>
public class MenuUpdateItem : MenuCrud, IResultFile
{
    private ItemLocal selectedItemLocal;
    private string oldImageName;
    private string newImageName;

    public void FileUploaded(bool isFileUploaded, string imageName)
    {
        if (isFileUploaded)
        {
            this.newImageName = imageName;
            UpdateItemRemote();
        }
    }

    public void InitMenu(string itemId)
    {
        if (MyApplication.repository != null)
        {
            try
            {
                ItemLocal itemLocal = MyApplication.repository.GetLocalItemById(itemId);
                this.selectedItemLocal = itemLocal;
                this.oldImageName = itemLocal.ImageName;
                this.newImageName = itemLocal.ImageName; // si elige otra imagén cambiará
                Texture2D texture2D = MyApplication.repository.LoadTextureAsPNG(itemLocal.ImageName);
                SetImageName(itemLocal.Name);
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
        if (IsDataSetted() && IsItemChanged())
        {
            try
            {
                progressText?.StartProgressTextAnimation("Actualizando", resultMsj);

                if(isImageChanged)
                {
                    byte[] fileBytes = fileManager.GetBytesImageSelected();
                    UploadFileRemote uploadFileRemote = new UploadFileRemote(fileBytes, fileManager.folderUidName, inputFieldName.text, iResult: this, iFileResult: this);
                    uploadFileRemote.UpoloadFileFirebaeStorage();
                } 
                else
                {
                    // solo se cambio los campos
                    UpdateItemRemote(); 
                }
            }
            catch (Exception excepcion)
            {
                SetResultCrudUi(false, "Error - " + excepcion.Message);
            }
        }
    }

    private void UpdateItemRemote()
    {
        // Ítem a actualizar
        ItemRemote itemRemote = new ItemRemote(
            id: selectedItemLocal.Id, 
            name: inputFieldName.text,
            imageName: !newImageName.Equals(oldImageName) ? newImageName : oldImageName, 
            creationDate: selectedItemLocal.CreationDate);

        // actualizamos el documente de firebase realtimadatabase
        MyApplication.repository.UpdateItemRemote(itemRemote, this);
        // borramos la imagén anterior en firebase storage si es necesario
        if(!newImageName.Equals(oldImageName))
        {
            ManageMaterialRemote manageMaterialRemote = new ManageMaterialRemote(oldImageName);
            manageMaterialRemote.DeleteImageRemote();
        }
    }

    private bool IsItemChanged()
    {
        string sanitizedFileName = StringSanitizer.SanitizeString(inputFieldName.text);
        return !newImageName.Equals(oldImageName) || !sanitizedFileName.Equals(selectedItemLocal.Name);
    }
}
