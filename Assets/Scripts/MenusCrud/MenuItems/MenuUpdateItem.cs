using System;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Comprobamos la actulización de los items cuando hacemos touch en alguno
/// Si el usuario eligio una nueva imagen se actualizará en cambio no lo hará
/// </summary>
public class MenuUpdateItem : MenuCrud, IResult, IResultDialogDelete
{
    [SerializeField] DialogDeleteConfirm dialogDeleteConfirm;
    private ItemLocal currentItemSelected;
    private string oldImageName;
    private string generateImageName;

    public void SetResultCrudUi(string title, string msj)
    {
        progressText?.StopProgressTextAnimation();
        uiApp.MenuSetActive(false);
        OpenDialog(title, msj);
    }

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
                SetImageNameInInput(itemLocal.Name);
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
                        // solo se actualizaron los campos y no la imagén
                        UpdateDocumentRemote();
                    }
                }
                catch (Exception excepcion)
                {
                    SetResultCrudUi("Error", "Error - " + excepcion.Message);
                }
            }
            else
            {
                SetResultCrudUi("Todo igual", "Nada ha cambiado....");
            }
        }
    }

    public void DeleteItem()
    {
        if(dialogDeleteConfirm != null)
        {
            dialogDeleteConfirm.ShowDialog("Borrar ítem" , "Deseas eliminar el ítem", this);
        } else
        {
            Debug.LogWarning("DialogDeleteConfirm es null, colocalo en el inspector");
        }
    }
     
    private async Task<bool> UpdateImageRemote()
    {
        byte[] fileBytes = fileManager.GetBytesImageSelected();
        UploadFileRemote uploadFileRemote = new UploadFileRemote(fileBytes, fileManager.folderNameUser);
        generateImageName = Guid.NewGuid().ToString();
        uploadFileRemote.SetImageNameGenerate(generateImageName);
        bool resultUpload = await uploadFileRemote.UploadFileFirebaseStorage();
        fileManager.ChangeNameImageCopySelected(generateImageName);
        await DeleteImageRemote();
        UpdateDocumentRemote();
        return resultUpload;
    }

    private void UpdateDocumentRemote()
    {
        // Ítem a actualizar
        ItemRemote itemRemote = new ItemRemote(
            id: currentItemSelected.Id,
            name: inputFieldName.text,
            imageName: isImageChanged ? generateImageName : oldImageName,
            creationDate: currentItemSelected.CreationDate);

        // actualizamos el documente de firebase realtimadatabase
        MyApplication.repository.UpdateItemRemote(itemRemote, resultUi: this);
    }

    /// <summary>
    /// borramos imagen desactualiza de firebase storage
    /// </summary>
    private async Task<bool> DeleteImageRemote()
    {
        // imagén de firebase storage
        ManageStorageRemote manageMaterialRemote =
                     new ManageStorageRemote(currentItemSelected.ImageName);
        await manageMaterialRemote.DeleteImageRemote(resultUi: this);
        return true;
    }
 
    /// <summary>
    /// Verificamos si el usuario cambio algo en la Ui
    /// </summary>
    /// <returns></returns>
    private bool IsSomeDatachanged()
    {
        string sanitizedFileName = StringSanitizer.SanitizeString(inputFieldName.text);
        return isImageChanged || !sanitizedFileName.Equals(currentItemSelected.Name);
    }

    public async void ConfirmDialogDelete(bool isDeleteConfirm)
    {
        if(isDeleteConfirm)
        {
            SetItemToDelete(true);
            await DeleteImageRemote();
            await MyApplication.repository.DeleteItemRemoteById(currentItemSelected.Id, iResultUi: this);
        }
    }

}
