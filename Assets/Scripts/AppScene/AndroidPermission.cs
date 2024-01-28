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
            float timeout = 10f; // Ajusta el tiempo de espera según tus necesidades

            while (!Permission.HasUserAuthorizedPermission(StoragePermission) && elapsedTime < timeout)
            {
                yield return null;
                elapsedTime += Time.deltaTime;
            }

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
