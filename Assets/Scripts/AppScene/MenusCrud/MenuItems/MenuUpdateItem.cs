/*********************************************************************************
 * Nombre del Archivo:     MenuUpdateItem.cs
 * Descripción:            Encargado de actualizar ítems desde su menu, asi como también
 *                         borrar items, y comunicarse con las clases correspondientes de manejo
 *                         para el manejo de archivos.
 *                         
 * Autor:                  Javier
 * Organización:           ReivaxCorp.
 *
 * Derechos de Autor (c) [2024] ReivaxCorp
 * 
 * Permiso es otorgado, sin cargo, para que cualquier persona obtenga una copia
 * de este software y de los archivos de documentación asociados (el "Software"),
 * para tratar en el Software sin restricción, incluyendo sin limitación los
 * derechos para usar, copiar, modificar, fusionar, publicar, distribuir,
 * sublicenciar, y/o vender copias del Software, y para permitir a las personas a
 * quienes pertenezca el Software, sujeto a las siguientes condiciones:
 *
 * El aviso de derechos de autor anterior y este aviso de permiso se incluirán en
 * todas las copias o partes sustanciales del Software.
 *
 * EL SOFTWARE SE PROPORCIONA "TAL CUAL", SIN GARANTÍA DE NINGÚN TIPO, EXPRESA O
 * IMPLÍCITA, INCLUYENDO PERO NO LIMITADO A LAS GARANTÍAS DE COMERCIABILIDAD,
 * IDONEIDAD PARA UN PROPÓSITO PARTICULAR Y NO INFRACCIÓN. EN NINGÚN CASO LOS
 * AUTORES O TITULARES DE DERECHOS DE AUTOR SERÁN RESPONSABLES DE CUALQUIER
 * RECLAMACIÓN, DAÑO O OTRA RESPONSABILIDAD, YA SEA EN UNA ACCIÓN DE CONTRATO, AGRAVIO
 * O DE OTRO MODO, DERIVADAS DE, FUERA DE O EN CONEXIÓN CON EL SOFTWARE O EL USO U OTROS
 * TRATOS EN EL SOFTWARE.
 *********************************************************************************/

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
        StartAnimationTextMenu(false, "");
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
                    StartAnimationTextMenu(false, "Actualizando");

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
            dialogDeleteConfirm.ShowDialog("Borrar ítem" , "¿Deseas eliminar el ítem?", this);
        } else
        {
            Debug.LogWarning("DialogDeleteConfirm es null, colocalo en el inspector");
        }
    }
     
    private async Task<bool> UpdateImageRemote()
    {
        // Obtenemos los bytes de la imagén temporal seleccionada
        byte[] fileBytes = fileManager.GetBytesImageSelected();
        // Generar nombre de imagén aleatorea
        generateImageName = Guid.NewGuid().ToString();
        ManageStorageRemote manageStorageRemote =
            new ManageStorageRemote(generateImageName, fileManager.folderNameUser, fileBytes);
        // subir nueva imagén
        bool resultUpload = await manageStorageRemote.UploadFileFirebaseStorage();
        fileManager.ChangeNameImageCopySelected(generateImageName);
        // borrar imagén anterior
        await DeleteImageRemote();
        // actualizar documento
        UpdateDocumentRemote();
        return resultUpload;
    }

    private async void UpdateDocumentRemote()
    {
        // Ítem a actualizar
        ItemRemote itemRemote = new ItemRemote(
            id: currentItemSelected.Id,
            name: inputFieldName.text,
            imageName: isImageChanged ? generateImageName : oldImageName,
            creationDate: currentItemSelected.CreationDate);
        
        // actualizamos el documente de firebase realtimadatabase
        bool updateResult = await MyApplication.repository.UpdateItemRemote(itemRemote, resultUi: this);
        if (updateResult) { Invoke("ShowInterstitialAd", AppConfig.timeInterstitialAd); }
    }

    /// <summary>
    /// borramos imagén anterior remoto, cuando ya hemos colocado otra imagén nueva
    /// de firebase storage, o borramos la imagén cuando borramos el ítem
    /// </summary>
    private async Task<bool> DeleteImageRemote()
    {
        // imagén de firebase storage
        ManageStorageRemote manageMaterialRemote =
                     new ManageStorageRemote(currentItemSelected.ImageName);
        await manageMaterialRemote.DeleteImageRemote();
        return true;
    }
 
    /// <summary>
    /// Verificamos si el usuario cambio algo en la Ui
    /// </summary>
    /// <returns></returns>
    private bool IsSomeDatachanged()
    {
        return isImageChanged || !inputFieldName.text.Equals(currentItemSelected.Name);
    }

    public async void ConfirmDialogDelete(bool isDeleteConfirm)
    {
        if(isDeleteConfirm)
        {
            await DeleteImageRemote();
            bool deleteResult = await MyApplication.repository.DeleteItemRemoteById(currentItemSelected.Id, iResultUi: this);
            if (deleteResult) { Invoke("ShowInterstitialAd", AppConfig.timeInterstitialAd); }
        }
    }

}
