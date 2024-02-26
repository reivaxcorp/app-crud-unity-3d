using System;
using UnityEngine;

public class MenuAddItem : MenuCrud, IResult
{
    private string generateImageName;

    public void SetResultCrudUi(EResultMenuAction result, string msj)
    {
        progressText?.StopProgressTextAnimation();

        if (resultMsj != null)
        {
            switch (result)
            {
                case EResultMenuAction.FileSuccessUploated:
                    resultMsj.text = msj;
                    resultMsj.color = Color.green;
                    fileManager.ChangeNameImageCopySelected(generateImageName);
                    WriteDocument(generateImageName);
                    break;
                case EResultMenuAction.FileFailedUploated:
                    resultMsj.text = msj;
                    resultMsj.color = Color.red;
                    break;
                case EResultMenuAction.FileCancelUploated:
                    resultMsj.text = msj;
                    resultMsj.color = Color.cyan;
                    break;
                case EResultMenuAction.DocumentSuccessCreated:
                    resultMsj.text = msj;
                    resultMsj.color = Color.green;
                    OpenDialog("ItemCreado", "Ítem creado correctamente");
                    uiApp.HideMenu();
                    break;
                case EResultMenuAction.DocumentFailedCreated:
                    resultMsj.text = msj;
                    resultMsj.color = Color.red;
                    break;
                case EResultMenuAction.DocumentCancelCreated:
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
  
    /// <summary>
    /// Cuando colocamos subir ítem, lo primero que hacemos es subir la imagén, luego escribimos
    /// los datos en realtime database, con WriteDocument, si la subida se realizó correctamente.
    /// </summary>
    public async void CreateDocumentRemote()
    {
        if (IsDataSetted())
        {
                progressText?.StartProgressTextAnimation("Subiendo", resultMsj);
                byte[] fileBytes = fileManager.GetBytesImageSelected();
                UploadFileRemote uploadFileRemote = new UploadFileRemote(fileBytes, fileManager.folderNameUser, iResult: this);
                generateImageName = Guid.NewGuid().ToString();
                uploadFileRemote.SetImageNameGenerate(generateImageName);
                await uploadFileRemote.UploadFileFirebaseStorage();
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
            // Invoke("ShowInterstitialAd", 3f);
        } else
        {
            Debug.LogWarning("El repositorio es Null");
        }
    }
     
}
