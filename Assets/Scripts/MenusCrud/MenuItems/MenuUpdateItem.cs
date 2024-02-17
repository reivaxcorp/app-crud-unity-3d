using System;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Comprobamos la actulización de los items cuando hacemos touch en alguno
/// Si el usuario eligio una nueva imagen se actualizará en cambio no lo hará
/// </summary>
public class MenuUpdateItem : MenuCrud
{
    private ItemLocal currentItemSelected;
    private string newImageName;
    private string oldImageName;
    private bool isDelteItem;

    public void InitMenu(string itemId)
    {
        if (MyApplication.repository != null)
        {
            try
            {
                ItemLocal itemLocal = MyApplication.repository.GetLocalItemById(itemId);
                this.currentItemSelected = itemLocal;
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
                        bool resultUpload = await UpdateImageRemote();
                        if (resultUpload)
                        {
                            UpdateDocumentRemote();
                        }
                    }
                    else
                    {
                        UpdateDocumentRemote();
                    }
                }
                catch (Exception excepcion)
                {
                    SetResultCrudUi(EResultMenuAction.Failed, "Error - " + excepcion.Message);
                }
            }
             else
            {
                SetResultCrudUi(EResultMenuAction.Nothing, "Nada ha cambiado....");
            }
        }
    }

    public async override void ConfirmButtonDialogPressed(bool isDialogConfirm)
    {
        if(isDialogConfirm)
        {
            if(MyApplication.repository != null)
            {
                if(isDelteItem)
                {
                    await MyApplication.repository.DeleteItemRemoteById(currentItemSelected.Id, this);
                    SetItemToDelete(false);
                }
                HideMenu();
            }
        }
    }

    public void DeleteItem()
    {
        if(currentItemSelected != null)
        {
            SetItemToDelete(true);
            OpenDialog("Eliminar ítem", "¿Desea eliminar el ítem?");
        }
    }

    private void UpdateDocumentRemote()
    {
        // Ítem a actualizar
        ItemRemote itemRemote = new ItemRemote(
            id: currentItemSelected.Id, 
            name: inputFieldName.text,
            imageName: isImageChanged ? newImageName : oldImageName, 
            creationDate: currentItemSelected.CreationDate);

        // actualizamos el documente de firebase realtimadatabase
        MyApplication.repository.UpdateItemRemote(itemRemote, this);
        HideMenu();
    }
 

    private async Task<bool> UpdateImageRemote()
    {
        byte[] fileBytes = fileManager.GetBytesImageSelected();
        UploadFileRemote uploadFileRemote = new UploadFileRemote(fileBytes, fileManager.folderUidName, iResult: this);
        bool resultUpload = await uploadFileRemote.UploadFileFirebaseStorage();
        this.newImageName = uploadFileRemote.newImageName;

        return resultUpload;
    }

    private void SetItemToDelete(bool isItemToDelete)
    {
        this.isDelteItem = isItemToDelete;
    }

    private bool IsSomeDatachanged()
    {
        string sanitizedFileName = StringSanitizer.SanitizeString(inputFieldName.text);
        return isImageChanged || !sanitizedFileName.Equals(oldImageName);
    }
}
