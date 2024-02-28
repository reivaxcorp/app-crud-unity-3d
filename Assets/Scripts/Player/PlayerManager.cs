using StarterAssets;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private Vector3 _initialPosition;
    private CharacterController characterController;
    private ThirdPersonController thirdPersonController;

    // Start is called before the first frame update
    void Start()
    {
        _initialPosition = transform.position;
        InitRef();
    }

    public void ResetOriginalPosition()
    {
        if (characterController != null)
        {
            transform.position = _initialPosition;
            DisableController(false);
        }
    }

    /// <summary>
    /// Para trasladar nuestro personaje es necesario deshabilitar los scrips de
    /// Started Assets
    /// </summary>
    /// <param name="isDisable"></param>
    public void DisableController(bool isDisable)
    {
        if (characterController != null && thirdPersonController != null)
        {
            characterController.enabled = !isDisable;
            thirdPersonController.enabled = !isDisable;
        }
    }

    private void InitRef()
    {
        characterController = GetComponent<CharacterController>();
        if(characterController == null ) { Debug.LogWarning("Deberia haber un CharacterController"); }
        thirdPersonController = GetComponent<ThirdPersonController>();
        if (thirdPersonController == null) { Debug.LogWarning("Deberia haber un ThirdPersonController"); }
    }
}
