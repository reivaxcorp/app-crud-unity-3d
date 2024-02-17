using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoystickController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] Camera _camera;
    [SerializeField] float moveSpeed = 5f;
    private bool isMovingUp;
    private bool isMovingDown;
    private bool isMovingLeft;
    private bool isMovingRight;

    private void Start()
    {
        if(Application.isMobilePlatform)
        {
            gameObject.SetActive(true);
        } else
        {
            gameObject.SetActive(false);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("dons");
        if (eventData.pointerEnter.name == "ButtonUp")
        {
            isMovingUp = true;
        }
        else if (eventData.pointerEnter.name == "ButtonDown")
        {
            isMovingDown = true;
        }
        else if (eventData.pointerEnter.name == "ButtonLeft")
        {
            isMovingLeft = true;
        }
        else if (eventData.pointerEnter.name == "ButtonRight")
        {
            isMovingRight = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerEnter == null)
        {
            isMovingRight = false;
            isMovingLeft = false;
            isMovingUp = false;
            isMovingDown = false;
            return;
        }

        if (eventData.pointerEnter.name == "ButtonUp")
        {
            isMovingUp = false;
        }
        else if (eventData.pointerEnter.name == "ButtonDown")
        {
            isMovingDown = false;
        }
        else if (eventData.pointerEnter.name == "ButtonLeft")
        {
            isMovingLeft = false;
        }
        else if (eventData.pointerEnter.name == "ButtonRight")
        {
            isMovingRight = false;
        }
    }

    void LateUpdate()
    {
        if (isMovingUp)
        {
            MoveCamera(Vector3.forward);
        }
        if (isMovingDown)
        {
            MoveCamera(Vector3.back);
        }
        if (isMovingLeft)
        {
            MoveCamera(Vector3.left);
        }
        if (isMovingRight)
        {
            MoveCamera(Vector3.right);
        }
    }

    void MoveCamera(Vector3 direction)
    {
        _camera.transform.Translate(direction * moveSpeed * Time.deltaTime);
    }
}
