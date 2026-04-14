using UnityEngine;

public class ButtonBoxUi : MonoBehaviour, IButtonUIItem
{
    public void ChangeImageView(Texture2D image)
    {
        throw new System.NotImplementedException();
    }

    public void OnPressedBox()
    {
        GetComponentInParent<ButtonBoxUiManager>().OnClickBox(this.gameObject.name);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
