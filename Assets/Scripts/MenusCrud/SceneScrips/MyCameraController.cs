using UnityEngine;

public class MyCameraController : MonoBehaviour
{
    public float panSpeed = 20f;
    public float zoomSpeed = 5f;

    private void Update()
    {
        // Movimiento de la cámara mediante deslizamiento del dedo o del mouse
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
            Vector3 move = new Vector3(-touchDeltaPosition.x, 0, -touchDeltaPosition.y) * panSpeed * Time.deltaTime;
            transform.Translate(move, Space.World);
        }
        else if (Input.GetMouseButton(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            Vector3 move = new Vector3(-mouseX, 0, -mouseY) * panSpeed * Time.deltaTime;
            transform.Translate(move, Space.World);
        }

        // Acercamiento de la cámara con dos dedos o con la rueda del mouse
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            Camera.main.fieldOfView += deltaMagnitudeDiff * zoomSpeed * Time.deltaTime;

            // Limitar el campo de visión para evitar acercamiento excesivo
            Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, 10f, 90f);
        }
        else
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            Camera.main.fieldOfView -= scroll * zoomSpeed * 100f * Time.deltaTime;

            // Limitar el campo de visión para evitar acercamiento excesivo
            Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, 10f, 90f);
        }
    }
}
