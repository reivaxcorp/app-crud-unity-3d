using TMPro;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public TextMeshProUGUI resultMsj;
    protected void SetMsjResult(string msj, Color color) {

        if (resultMsj != null)
        {
            resultMsj.SetText(msj);
            resultMsj.color = color;
        }
        else
        {
            Debug.LogWarning("msj result menu is null");
        }
    }
}
