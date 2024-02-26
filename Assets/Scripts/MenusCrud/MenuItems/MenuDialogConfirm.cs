using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuDialogConfirm : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textTitle;
    [SerializeField] TextMeshProUGUI textBody;
    [SerializeField] MenuManagerApp menuManager;
    private IResultDialog iResultDialog;
    private void Awake()
    {
        CheckReferences();
    }

    public void ShowDialog(string title, string message, IResultDialog resultDialog)
    {
        this.iResultDialog = resultDialog;
        SetTitle(title);
        SetBodyText(message);
        ShowDialog();
    }


    // el usuario cerro con el boton "Aceptar" del dialogo
    public void OnAccept()
    {
        HideDialog();
        iResultDialog.ConfirmButtonDialogPressed(true);
    }

    // el usuario cerro con la "X" el dialogo de confirmacion
    public void OnClosed()
    {
        iResultDialog.ConfirmButtonDialogPressed(false);
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

    private void HideDialog()
    {
        menuManager.HideUiButtons(true);
        gameObject.SetActive(false);
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
        if (menuManager == null) Debug.LogWarning("Pon la reference de MenuManager en el inspector");
    }

}
