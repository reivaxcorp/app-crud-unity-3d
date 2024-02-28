/*********************************************************************************
 * Nombre del Archivo:     MyCameraController.cs
 * Descripción:            Movemos la camara por medio de touch and drag
 *                         
 * Autor:                  Javier
 * Organización:           ReivaxCorp.
 *
 * Derechos de Autor (c) [2024] ReivaxCorp
 * 
 * Permiso es otorgado, sin cargo, para que cualquier persona obtenga una copia
 * de este software y de los archivos de documentación asociados (el "Software"),
 * para tratar en el Software sin restricción, incluyendo sin limitación los
 * derechos para usar, copiar, modificar, fusionar, publicar, distribuir,
 * sublicenciar, y/o vender copias del Software, y para permitir a las personas a
 * quienes pertenezca el Software, sujeto a las siguientes condiciones:
 *
 * El aviso de derechos de autor anterior y este aviso de permiso se incluirán en
 * todas las copias o partes sustanciales del Software.
 *
 * EL SOFTWARE SE PROPORCIONA "TAL CUAL", SIN GARANTÍA DE NINGÚN TIPO, EXPRESA O
 * IMPLÍCITA, INCLUYENDO PERO NO LIMITADO A LAS GARANTÍAS DE COMERCIABILIDAD,
 * IDONEIDAD PARA UN PROPÓSITO PARTICULAR Y NO INFRACCIÓN. EN NINGÚN CASO LOS
 * AUTORES O TITULARES DE DERECHOS DE AUTOR SERÁN RESPONSABLES DE CUALQUIER
 * RECLAMACIÓN, DAÑO O OTRA RESPONSABILIDAD, YA SEA EN UNA ACCIÓN DE CONTRATO, AGRAVIO
 * O DE OTRO MODO, DERIVADAS DE, FUERA DE O EN CONEXIÓN CON EL SOFTWARE O EL USO U OTROS
 * TRATOS EN EL SOFTWARE.
 *********************************************************************************/

using UnityEngine;

public class MyCameraController : MonoBehaviour
{
    private float panSpeed = 20f;
    private float zoomSpeed = 5f;

    private void Start()
    {
        if(Application.isMobilePlatform)
        {
            this.panSpeed = 0.5f;
            this.zoomSpeed = 0.1f;
        } else
        {
            this.panSpeed = 50f;
            this.zoomSpeed = 17f;
        }
    }

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
