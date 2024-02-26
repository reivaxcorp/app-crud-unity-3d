using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogDeleteConfirm : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textTitle;
    [SerializeField] TextMeshProUGUI textBody;
    [SerializeField] MenuUpdateItem menuUpdaItem;
    private IResultDialogDelete iResultDialogDelete;
    private void Awake()
    {
        CheckReferences();
    }

    public void ShowDialog(string title, string message, IResultDialogDelete resultDialogDelete)
    {
        this.iResultDialogDelete = resultDialogDelete;
        SetTitle(title);
        SetBodyText(message);
        ShowDialog();
    }


    // el usuario cerro con el boton "Aceptar" del dialogo
    public void OnAccept()
    {
        iResultDialogDelete.ConfirmDialogDelete(true);
        OnClosed();
    }

    // el usuario cerro con la "X" el dialogo de confirmacion
    public void OnClosed()
    {
        gameObject.SetActive(false);
    }

    public void ShowDialog()
    {
        gameObject.SetActive(true);
    }

    public void SetTitle(string title)
    {
        this.textTitle.text = title;
    }

    public void SetBodyText(string bodyText)
    {
        this.textBody.text = bodyText;
    }

    private void OnDisable()
    {
        ClearDialog();
    }

    private void ClearDialog()
    {
        textTitle.text = "";
        textBody.text = "";
    }

    private void CheckReferences()
    {
        if (textTitle == null) Debug.LogWarning("Pon la referencia Title en el inspector");
        if (textBody == null) Debug.LogWarning("Pon la referencia Msj en el inspector");
        if (menuUpdaItem == null) Debug.LogWarning("Pon la reference de MenuUpdaItem en el inspector");
    }

}
