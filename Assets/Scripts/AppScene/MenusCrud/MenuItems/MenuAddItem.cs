/*********************************************************************************
 * Nombre del Archivo:     MenuAddItem.cs
 * Descripción:            Clase responsable de añadir nuevos ítems.  
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
using UnityEngine;

public class MenuAddItem : MenuCrud, IResult
{
    private string generateImageName;
    public NetworkManager network;

    public void SetResultCrudUi(string title, string msj)
    {
        StartAnimationTextMenu(false, "");
        uiApp.MenuSetActive(false);
        OpenDialog(title, msj);
    }

    /// <summary>
    /// Cuando colocamos subir ítem, lo primero que hacemos es subir la imagén, luego escribimos
    /// los datos en realtime database, con WriteDocument, si la subida se realizó correctamente.
    /// </summary>
    public async void CreateDocumentRemote()
    {
        if(network.HasInternet() == false)
        {
            OpenDialog("No Conneccion", "You need internet connection");
            return;
        }

        if(AppConfig.IsItemAvariableToPut())
        {
            if (IsDataSetted())
            {
                StartAnimationTextMenu(true, "Creating");

                if(fileManager != null)
                {
                    // Obtenemos los bytes de la imagén temporal seleccionada
                    byte[] fileBytes = fileManager.GetBytesImageSelected();
                    // Generar nombre de imagén aleatorea
                    generateImageName = Guid.NewGuid().ToString();
                    // subir nueva imagén

                    Debug.LogWarning("Intentando subir imagen:" + generateImageName);

                    bool uploadResult = await MyApplication.repository.UploadFileFirebaseStorage(generateImageName, fileManager.folderNameUser, fileBytes);

                    if (uploadResult)
                    {
                        fileManager.ChangeNameImageCopySelected(generateImageName);
                        WriteDocumentRemote(generateImageName);
                    }
                    else
                    {
                        SetResultCrudUi("Error", "Error uploading the document");
                    }
                }
                else
                {
                    SetResultCrudUi("Error", "Image error, try again.");
                    Debug.LogWarning("FileManager es null");
                }
                 
            }
        }
        else
        {
            SetMsjInfoUI("Maximum items reached, edit or delete one");
        }
    }

    /// <summary>
    /// Una vez subida la imagén, procedemos a escribir el documento.
    /// </summary>
    /// <param name="imageName"></param>
    private async void WriteDocumentRemote(string imageName)
    {
        Debug.LogWarning("El nombre de la imagen a subir es: " + imageName);
        if (MyApplication.repository != null)
        {
            ItemRemote itemRemote = new ItemRemote( imageName: imageName);
            bool saveResult = await MyApplication.repository.SaveItemRemote(itemRemote, resultUi: this);
           
            if(saveResult) {
                Debug.Log("Documento escrito en firebase");
            }
        }
        else
        {
            Debug.LogWarning("El repositorio es Null");
        }
    }

}
