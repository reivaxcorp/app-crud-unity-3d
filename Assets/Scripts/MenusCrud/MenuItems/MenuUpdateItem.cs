using System;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Comprobamos la actulización de los items cuando hacemos touch en alguno
/// Si el usuario eligio una nueva imagen se actualizará en cambio no lo hará
/// </summary>
public class MenuUpdateItem : MenuCrud
{
    private ItemLocal selectedItemLocal;
    private string newImageName;
    private string oldImageName;

    public void InitMenu(string itemId)
    {
        if (MyApplication.repository != null)
        {
            try
            {
                ItemLocal itemLocal = MyApplication.repository.GetLocalItemById(itemId);
                this.selectedItemLocal = itemLocal;
                Texture2D texture2D = MyApplication.repository.LoadTextureAsPNG(itemLocal.ImageName);
                SetImageName(itemLocal.Name);
                SetImagePreview(texture2D);
                this.oldImageName = itemLocal.ImageName;
            }
            catch(Exception exception)
            {
                Debug.LogError("Error en la textura o el ítem no existe! " + exception);
            }
        }
        else
        {
            Debug.LogWarning("El repositorio es null");
        }
    }

    /// <summary>
    /// Si se cambia la imagén, debemos subirla, y luego si se sube correctamente,
    /// debemos borrar la imagén anterior de firebase storage, por medio de la interface
    /// IResultFile -> FileUploaded método
    /// </summary>
    public async void UpdateItem()
    {
        if (IsDataSetted())
        {
            if(IsSomeDatachanged())
            {
                try
                {
                    progressText?.StartProgressTextAnimation("Actualizando", resultMsj);

                    if (isImageChanged)
                    {
                        await UpdateImageRemote();
                    }
                    else
                    {
                        // solo se cambio los campos
                        UpdateDocumentRemote();
                    }
                }
                catch (Exception excepcion)
                {
                    SetResultCrudUi(EResultMenuAction.Failed, "Error - " + excepcion.Message);
                }
            } 
            else if(IsAllDataChanged())
            {
                UpdateAllItem();
            }
             else
            {
                SetResultCrudUi(EResultMenuAction.Nothing, "Nada ha cambiado....");
            }
        }
    }

    public override void ConfirmButtonDialogPressed(bool isDialogConfirm)
    {
        if(isDialogConfirm)
        {
            if(MyApplication.repository != null)
            {
                MyApplication.repository.DeleteItemRemoteById(selectedItemLocal.Id, this);
                HideMenu();
                ResetMenu();
            }
        }
    }

    public void DeleteItem()
    {
        if(selectedItemLocal != null)
        {
            OpenDialog("Eliminar ítem", "¿Desea eliminar el ítem?");
        }
    }

    private void UpdateDocumentRemote()
    {
        // Ítem a actualizar
        ItemRemote itemRemote = new ItemRemote(
            id: selectedItemLocal.Id, 
            name: inputFieldName.text,
            imageName: isImageChanged ? newImageName : oldImageName, 
            creationDate: selectedItemLocal.CreationDate);

        // actualizamos el documente de firebase realtimadatabase
        MyApplication.repository.UpdateItemRemote(itemRemote, this);
    }

    private async void UpdateAllItem()
    {
        bool resultUpload = await UpdateImageRemote();
        if (resultUpload)
        {
            UpdateDocumentRemote();
        }
    }

    private async Task<bool> UpdateImageRemote()
    {
        byte[] fileBytes = fileManager.GetBytesImageSelected();
        UploadFileRemote uploadFileRemote = new UploadFileRemote(fileBytes, fileManager.folderUidName, iResult: this);
        bool resultUpload = await uploadFileRemote.UploadFileFirebaseStorage();
        await RemoveOldImageRemote();
        this.newImageName = uploadFileRemote.newImageName;
        return resultUpload;
    }

    /// <summary>
    /// borramos la imagén anterior en firebase storage si es necesario
    /// </summary>
    private async Task<bool> RemoveOldImageRemote()
    {
        ManageMaterialRemote manageMaterialRemote = new ManageMaterialRemote(oldImageName);
        bool resultDeleteRemote = await manageMaterialRemote.DeleteImageRemote();
        return resultDeleteRemote;
    }

    private bool IsAllDataChanged()
    {
        string sanitizedFileName = StringSanitizer.SanitizeString(inputFieldName.text);
        return isImageChanged && !sanitizedFileName.Equals(oldImageName);
    }

    private bool IsSomeDatachanged()
    {
        string sanitizedFileName = StringSanitizer.SanitizeString(inputFieldName.text);
        return isImageChanged || !sanitizedFileName.Equals(oldImageName);
    }
}
