using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuLogin : Menu
{

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void SetMenuResult(string name)
    {
        if (resultMsj != null)
        {
            resultMsj.text = name;
        }
        else
        {
            Debug.LogWarning("msj result menu is null");
        }
    }
}
