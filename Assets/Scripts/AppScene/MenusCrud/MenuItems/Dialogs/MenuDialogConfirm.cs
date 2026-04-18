using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class MenuDialogConfirm : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textTitle;
    [SerializeField] TextMeshProUGUI textBody;
    [SerializeField] MenuManagerApp menuManager;

    private ICallBackActions _actionsCallback;

    private void Awake()
    {
        CheckReferences();
    }

    public void SetCallBack(ICallBackActions actions)
    {
        _actionsCallback = actions;
    }

    public void ShowDialog(string title, string message)
    {
        SetTitle(title);
        SetBodyText(message);
        ShowDialog();
    }

    // el usuario cerro con la "X" el dialogo de confirmacion
    public void OnClosed()
    {
        HideDialog();
    }

    public void OnAccept()
    {
        if(_actionsCallback != null)
        {
            _actionsCallback.OnConfirmButton();
            _actionsCallback = null;
        }else
        {
            HideDialog();
        }
    }

    public void ShowDialog()
    {
        gameObject.SetActive(true);
    }

    public void SetTitle(string title)
    {
        textTitle.text = title;
    }

    public void SetBodyText(string bodyText)
    {
        textBody.text = bodyText;
    }

    private void HideDialog()
    {
        menuManager.HideUiButtons(false);
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
