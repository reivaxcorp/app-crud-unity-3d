using UnityEngine;
using UnityEngine.UI;

public class ButtonBoxUi : MonoBehaviour
{
  
    public void OnButtonClick()
    {
        Image _myImage = GetComponent<Image>(); 
        // 1. Verificamos que haya un sprite asignado
        if (_myImage != null && _myImage.sprite != null)
        {
            // 2. Extraemos la Texture2D del sprite
            Texture2D texture2d = _myImage.sprite.texture;

            // 3. Enviamos el nombre y la textura al Manager
            GetComponentInParent<ButtonBoxUiManager>()
                 .OnClickBox(this.gameObject.name, texture2d);
        }
        else
        {
            Debug.LogWarning($"El slot {gameObject.name} no tiene una imagen cargada aún.");
        }
    }
}
