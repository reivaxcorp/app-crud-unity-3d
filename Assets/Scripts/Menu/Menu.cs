using TMPro;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public TextMeshProUGUI resultMsj;
    public virtual void SetMenuResult(string name) {
        resultMsj.text = name;  
    }
}
