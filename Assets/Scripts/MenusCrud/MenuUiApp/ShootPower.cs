using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.HID;

public class ShootPower : MonoBehaviour
{
    [SerializeField] GameObject gun;
    [SerializeField] GameObject objectToLaunchPrefab; // Prefab del objeto que quieres lanzar
    [SerializeField] private float speedBullet = 20f;

    void Update()
    {
        // Verificar si se ha tocado la pantalla en dispositivos táctiles o se ha hecho clic con el mouse
        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetMouseButtonDown(0))
        {
            // Verificar si el puntero está sobre un elemento de la interfaz de usuario
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                // Obtener la posición del toque en la pantalla o del clic del mouse
                Vector3 touchPosition;

                // Verificar si se está ejecutando en un dispositivo táctil
                if (Input.touchCount > 0)
                {
                    // Obtener la posición del toque en la pantalla
                    touchPosition = Input.GetTouch(0).position;
                }
                // Verificar si se está ejecutando en el editor de Unity y se ha hecho clic con el mouse
                else if (Input.GetMouseButtonDown(0))
                {
                    // Obtener la posición del clic del mouse
                    touchPosition = Input.mousePosition;
                }
                else
                {
                    // En caso de que no haya ni toques ni clics, se asigna un valor por defecto
                    touchPosition = Vector3.zero;
                }

                // Convertir la posición del toque de pantalla a un rayo en el mundo
                Ray ray = Camera.main.ScreenPointToRay(touchPosition);

                // Dibujar el rayo en la escena
                Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.green, 5f);

                // Lanzar el objeto en la dirección del rayo
                LaunchObject(ray);
            }
        }
    }

    void LaunchObject(Ray ray)
    {
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Debug.Log("Item box hit.. " + hit.collider);

            // Obtener la posición donde el rayo colisionó con la superficie
            Vector3 targetPosition = hit.point;

            // Instanciar el objeto a lanzar en la posición del jugador
            GameObject objectToLaunch = Instantiate(objectToLaunchPrefab, gun.transform.position, Quaternion.identity);

            // Iniciar la coroutine para mover el objeto hacia la posición del objetivo
            StartCoroutine(MoveToObject(targetPosition, objectToLaunch));
        } else
        {
            // Instanciar el objeto a lanzar en la posición del jugador
            GameObject objectToLaunch = Instantiate(objectToLaunchPrefab, gun.transform.position, Quaternion.identity);
            // Iniciar la coroutine para mover el objeto hacia la posición del objetivo
            StartCoroutine(MoveToInfinity(ray, objectToLaunch));
        }
    }

    IEnumerator MoveToObject(Vector3 targetPosition, GameObject objectToLaunch)
    {
        while (objectToLaunch != null)
        {
            // Calcular la dirección hacia la posición del objetivo
            Vector3 direction = (targetPosition - objectToLaunch.transform.position).normalized;

            // Mover el objeto hacia la posición del objetivo
            objectToLaunch.transform.Translate(direction * speedBullet * Time.deltaTime);

            yield return null; // Esperar al siguiente frame antes de continuar la coroutine
        }
    }

    IEnumerator MoveToInfinity(Ray ray, GameObject objectToLaunch)
    {
        while (objectToLaunch != null)
        {
            // Calcular la dirección hacia la posición del objetivo
            Vector3 direction = ray.direction.normalized;

            // Mover el objeto hacia la posición del objetivo
            objectToLaunch.transform.Translate(direction * speedBullet * Time.deltaTime);

            yield return null; // Esperar al siguiente frame antes de continuar la coroutine
        }
    }
}