/*********************************************************************************
 * Nombre del Archivo:     AndroidPermission.cs
 * Descripción:            Solicitamos los permisos que sera necesario 
 *                         para acceder al sistema de archivos del usuario
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

using System.Collections;
using UnityEngine;
using UnityEngine.Android;

public enum PermissionStatus
{
    Granted,
    Denied
}

public class AndroidPermission : MonoBehaviour
{
    public delegate void PermissionCallback(PermissionStatus status);
    public event PermissionCallback OnPermissionResult;

    private const string StoragePermission = Permission.ExternalStorageRead;

    public void RequestStoragePermission()
    {
        StartCoroutine(RequestStoragePermissionCoroutine());
    }

    private IEnumerator RequestStoragePermissionCoroutine()
    {
        if (!Permission.HasUserAuthorizedPermission(StoragePermission))
        {
            Permission.RequestUserPermission(StoragePermission);

            float elapsedTime = 0f;
            float timeout = 10f; // Tiempo para aceptar los permisos

            while (!Permission.HasUserAuthorizedPermission(StoragePermission) && elapsedTime < timeout)
            {
                yield return null;
                elapsedTime += Time.deltaTime;
            }

            // Una vez pasado el tiempo, preguntamos si el  usuario confirmo los permisos
            if (Permission.HasUserAuthorizedPermission(StoragePermission))
            {
                OnPermissionResult?.Invoke(PermissionStatus.Granted);
            }
            else
            {
                OnPermissionResult?.Invoke(PermissionStatus.Denied);
            }
        }
        else
        {
            OnPermissionResult?.Invoke(PermissionStatus.Granted);
        }
    }

}
