using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuAppManager : MonoBehaviour
{
    [SerializeField] Image imageSelected;
    [SerializeField] TMP_InputField inputItemName;
    [SerializeField] DialogConfirm dialogConfirm;

    private void Awake()
    {
        if (inputItemName == null) Debug.LogWarning("Please put InputItemName ref on Inspector");
        if(dialogConfirm == null) Debug.LogWarning("Please put DialogConfirm ref on Inspector");
    }

    public void OnSelecImagen()
    {

    }


    public string GetItemInputName()
    {
        if (inputItemName.text.Length > 0)
        {
            return inputItemName.text;
        }
        return "Sin nombre";
    }
}
