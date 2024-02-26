using System;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Comprobamos la actulización de los items cuando hacemos touch en alguno
/// Si el usuario eligio una nueva imagen se actualizará en cambio no lo hará
/// </summary>
public class MenuUpdateItem : MenuCrud, IResult
{
    private ItemLocal currentItemSelected;
    private string oldImageName;
    private bool isDelteItem;

    /// <summary>
    /// Cuando abrimos el menu, seteamos los valores.
    /// </summary>
    /// <param name="itemId"></param>
    public async void InitMenu(string itemId)
    {
        if (MyApplication.repository != null)
        {
            try
            {
                ItemLocal itemLocal = await MyApplication.repository.GetLocalItemById(itemId);
                this.currentItemSelected = itemLocal;
                FileManager fileManager = new FileManager(FirebaseSDK.GetInstance().auth.CurrentUser.UserId);
                Texture2D texture2D = fileManager.LoadFileAsTexture2D(itemLocal.ImageName); 
                SetImageName(itemLocal.Name);
                SetImagePreview(texture2D);
                this.oldImageName = itemLocal.ImageName;
            }
            catch (Exception exception)
            {
                Debug.LogError("Error en la textura o el ítem no existe! " + exception);
            }
        }
        else
        {
            Debug.LogWarning("El repositorio es null");
        }
    }

    public async void SetResultCrudUi(EResultMenuAction result, string msj)
    {
        progressText?.StopProgressTextAnimation();

        if (resultMsj != null)
        {
            switch (result)
            {
                case EResultMenuAction.FileSuccessUploated:
                    resultMsj.text = msj;
                    resultMsj.color = Color.green;
                    fileManager.DeletePreviousCopyImage();
                    fileManager.SetCurrentImageName(oldImageName);
                    fileManager.ChangeNameImageCopySelected(imageNameGenerated);
                    break;
                case EResultMenuAction.FileFailedUploated:
                    resultMsj.text = msj;
                    resultMsj.color = Color.red;
                    break;
                case EResultMenuAction.FileCancelUploated:
                    resultMsj.text = msj;
                    resultMsj.color = Color.cyan;
                    break;
                case EResultMenuAction.DocumentSuccessUpdate:
                    if(isImageChanged)
                    {
                        // imagén de firebase storage
                        ManageTextureRemote manageMaterialRemote =
                                     new ManageTextureRemote(oldImageName);
                        await manageMaterialRemote.DeleteImageRemote();
                    }
                    resultMsj.text = msj;
                    resultMsj.color = Color.green;
                    HideMenu();
                    ResetMenu();
                    break;
                case EResultMenuAction.DocumentFailedUpdate:
                    resultMsj.text = msj;
                    resultMsj.color = Color.red;
                    break;
                case EResultMenuAction.DocumentCancelUpdate:
                    resultMsj.text = msj;
                    resultMsj.color = Color.cyan;
                    break;
                default:
                    break;
            }
        }
        else
        {
            Debug.LogWarning("Por favor, coloca resultMsj en el Inspector");
        }
    }

    public void SetResultWriteDocument(EResultMenuAction result, string title, string body)
    {
        if (result == EResultMenuAction.FileSuccessUploated)
        {
            OpenDialog(title, body);
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
                        // solo se actualizaron los campos y no la imagén
                        UpdateDocumentRemote();
                    }
                }
                catch (Exception excepcion)
                {
                    SetResultCrudUi(EResultMenuAction.FileFailedUploated, "Error - " + excepcion.Message);
                }
            }
            else
            {
                SetResultCrudUi(EResultMenuAction.DocumentCancelCreated, "Nada ha cambiado....");
            }
        }
    }

    public bool IsDataSetted()
    {
        ClearResultCrud();

        if (inputFieldName == null)
        {
            LogWarningAndSetResult("InputFieldName no asignado en el Inspector");
            return false;
        }

        // Sanitizar el nombre de la imagen utilizando la expresión regular
        string sanitizedFileName = StringSanitizer.SanitizeString(inputFieldName.text);

        if (string.IsNullOrEmpty(sanitizedFileName))
        {
            LogWarningAndSetResult("Ingrese el nombre de la imagén");
            return false;
        }

        if (sanitizedFileName.Length > 30)
        {
            LogWarningAndSetResult("Nombre debe ser menor a 30 caracteres");
            return false;
        }

        if (menuImagePreview == null)
        {
            LogWarningAndSetResult("MenuImagePreview no asignado en el Inspector");
            return false;
        }

        if (menuImagePreview.sprite == null)
        {
            LogWarningAndSetResult("Seleccione una imagen");
            return false;
        }

        return true;
    }

    private void LogWarningAndSetResult(string mensajeAdvertencia)
    {
        Debug.LogWarning(mensajeAdvertencia);
        SetResultCrudUi(EResultMenuAction.FileFailedUploated, mensajeAdvertencia);
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
            imageName: isImageChanged ? imageNameGenerated : oldImageName, 
            creationDate: currentItemSelected.CreationDate);

        // actualizamos el documente de firebase realtimadatabase
        MyApplication.repository.UpdateItemRemote(itemRemote, this);
      
    }
 

    private async Task<bool> UpdateImageRemote()
    {
        byte[] fileBytes = fileManager.GetBytesImageSelected();
        UploadFileRemote uploadFileRemote = new UploadFileRemote(fileBytes, fileManager.folderNameUser, iResult: this);
        bool resultUpload = await uploadFileRemote.UploadFileFirebaseStorage();
        SetImageNameGenerate(uploadFileRemote.newImageName);
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
