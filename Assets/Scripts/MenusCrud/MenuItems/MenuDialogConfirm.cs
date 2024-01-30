using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuDialogConfirm : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textMeshTitle;
    public delegate void OnAccionSelected(bool acceptAction);
    public event OnAccionSelected onAccionSelected;

    private void Awake()
    {
        if (textMeshTitle == null) Debug.LogWarning("Pon la referencia TextMeshTitle en el inspector");
    }

    public void SetTitle(string title)
    {
        this.textMeshTitle.text = title;
    }

    public void OnAccept()
    {
        onAccionSelected?.Invoke(true);
    }

    public void OnCancel()
    {
        onAccionSelected?.Invoke(false);
    }

    private void OnDisable()
    {
        textMeshTitle.text = "";
    }
}
