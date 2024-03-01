/*********************************************************************************
 * Nombre del Archivo:     FileManager.cs
 * Descripción:            Se encargará en la administración de archivos localmente en el 
 *                         dispositivo, cargando las texturas, cambiando nombres.  
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
using System.IO;
using UnityEngine;

public class FileManager
{
    /// <summary>
    /// Cambiar de tamaño si quieres mas calidad de imagén, tamaño en px.
    /// Ten en cuenta que a mayor tamaño mas tardara en cargar las imágenes. Y también
    /// se verá reflejado en la cuota de firebase, si tienes muchos usuarios.
    /// </summary>
    private const int SIZE_WIDTH = 512;
    private const int SIZE_HEIGHT = 512;

    public string folderNameUser
    {
        private set { _folderNameUser = value; }
        get { return _folderNameUser; }
    }

    private string _currentImageName;
    private string _folderNameUser;
 
    public FileManager(string folderNameUser)
    {
        _folderNameUser = folderNameUser;
    }

    /// <summary>
    /// Creamos un intent para android, el usuario debe seleccionar una imagen, el resultado 
    /// será manejado por Manager GameObject de la Hierachy en su script 
    /// ReceiverMessagesFromAndroid.cs
    /// </summary>
    public void CreateIntentFileAndroid()
    {
        // Llamar a tu actividad de Android
        AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityPlayer = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");

        // Crear el intent para obtener contenido
        AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent");
        intent.Call<AndroidJavaObject>("setAction", "android.intent.action.GET_CONTENT");
        intent.Call<AndroidJavaObject>("setType", "image/*");  // Filtra por archivos de imagen

        // Inicia la actividad personalizada con startActivityForResult
        int requestCode = 123; // Puedes cambiar este código a tu preferencia
        unityPlayer.Call("startActivityForResult", intent, requestCode, null);
    }

    /// <summary>
    /// Obtenemos los bytes de la imagén temporal seleccionada
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public byte[] GetBytesImageSelected()
    {

        if (_currentImageName != null && _folderNameUser != null)
        {

            string filePath = FilesPath.GetFolderItemPath(_currentImageName, _folderNameUser);

            if (File.Exists(filePath))
            {
                // Leer todos los bytes del archivo en filePath
                byte[] imageBytes = File.ReadAllBytes(filePath);

                // Ahora 'imageBytes' contiene los bytes de la imagen que puedes usar para subir a Firebase Storage.

                Debug.Log("Archivo leído con éxito: " + _currentImageName);

                return imageBytes;
            }
        }

        throw new Exception("Ruta de archivo no encontrada");
    }

    /// <summary>
    /// el nombre de la carpeta de nuestra cuenta, será nuestro Uid de usuario, donde colocaremos las imagenes
    /// de los items de esa cuenta.
    /// </summary>
    public void SetFolderUidName(string uidFolderName)
    {
        this._folderNameUser = uidFolderName;
    }

    public void SetCurrentImageName(string imageName)
    {
        this._currentImageName = imageName;
    }

    /// <summary>
    /// Borramos la imagén anterior del dispositivo, cuando el usuario elige otra imagén
    /// </summary>
    /// <param name="ImageName"></param>
    public void DeletePreviousCopyImage()
    {
        if (_currentImageName != null && _folderNameUser != null)
        {

            string filePath = FilesPath.GetFolderItemPath(_currentImageName, _folderNameUser);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log("Archivo eliminado con éxito: " + _currentImageName);
            }
        }
    }

    /// <summary>
    /// Borramos la imagen que tengamos anteriormente, al actualizar la imagen
    /// </summary>
    public void DeleteOldImageLocalImage(string oldImageName)
    {
        string filePath = FilesPath.GetFolderItemPath(oldImageName, _folderNameUser);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("Archivo antiguo eliminado con éxito: " + oldImageName);
        }
    }

    /// <summary>
    /// Cuando elegimos un item del explorador de archivos, hacemos una copia pero
    /// redimencionada, en el directorio de la aplicación
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="imageName"></param>
    public void SaveFileInternalExtorage(Texture2D texture, string imageName)
    {
        if (_folderNameUser != null && _folderNameUser.Length > 0)
        {
            // Redimensionar la textura a 512x512 px (si es necesario)
            Texture2D resizedTexture = TextureScaler.ScaleTexture(texture, SIZE_WIDTH, SIZE_HEIGHT);

            string path = FilesPath.GetFolderItemPath(imageName, _folderNameUser);

            // Escribir los bytes de la textura en un archivo PNG
            byte[] bytesImage = resizedTexture.EncodeToPNG();
            File.WriteAllBytes(path, bytesImage);

            // Puedes mostrar un mensaje de éxito o realizar otras acciones después de guardar la imagen
            Debug.Log("Imagen guardada con éxito en el almacenamiento interno de la aplicación");
        }
        else
        {
            Debug.LogWarning("User uid doesn't exist");
        }
    }

    public Texture2D LoadFileAsTexture2D(string imageName)
    {
        if (_folderNameUser != null && _folderNameUser.Length > 0)
        {
            string filePath = FilesPath.GetFolderItemPath(imageName, folderNameUser);

            if (File.Exists(filePath))
            {
                byte[] bytes = File.ReadAllBytes(filePath);
                Texture2D loadedTexture = new Texture2D(2, 2); // Crea una textura vacía
                loadedTexture.LoadImage(bytes); // Carga los bytes como textura PNG
                return loadedTexture;
            }
            else
            {
                return null;
            }
        }
        return null;
    }

    /// <summary>
    /// Cambiamos el nombre de la imagen copiada en nuestro dispositivo interno,
    /// por la que se subio a firebase storage, asi no la bajamos ahorrando una bajada
    /// innecesaria de firebase storage.
    /// </summary>
    /// <param name="generateImageName">Nombre de archivo generado aleatoreamente, para guardarlo en el documento de firebase realtime database</param>
    public void ChangeNameImageCopySelected(string generateImageName)
    {
        if (generateImageName != null)
        {
            string originalPath = FilesPath.GetFolderItemPath(_currentImageName, _folderNameUser);
            string newPath = FilesPath.GetFolderItemPath(generateImageName, _folderNameUser); // Ruta nueva con el nuevo nombre

            if (File.Exists(originalPath))
            {
                File.Move(originalPath, newPath);
                Debug.Log("Nombre de imagen cambiado con éxito.");
            }
            else
            {
                Debug.LogError("La imagen original no existe en la ruta proporcionada.");
            }
        }
        else
        {
            Debug.LogWarning("generateImageName es null");
        }
    }
}
