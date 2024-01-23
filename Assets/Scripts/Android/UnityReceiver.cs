using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UnityReceiver : MonoBehaviour
{
    [SerializeField] TMP_InputField nombre;
    public void ReceiveData(string fileUri)
    {
        // Lógica para manejar la URI del archivo en Unity
        Debug.Log("Received file URI in Unity: " + fileUri);
        nombre.text = fileUri;
    }
}
