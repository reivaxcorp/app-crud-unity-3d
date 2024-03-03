/*********************************************************************************
 * Nombre del Archivo:     PlayerShoot.cs 
           realizado un touch o un click en el editor. Si esta bala colisiona con algun ítem
 *                         en la escena, se abrira su men * Descripción:            Clase encargada de inicializar, una bala que se dirigira hacia el usuario haya
 *              u para editar.
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
using UnityEngine.EventSystems;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] GameObject gunPlayer;
    [SerializeField] GameObject bulletPrefb; // Prefab del objeto que quieres lanzar
    [SerializeField] private float powerBullet = 2000f;

    void Update()
    {
        BulletRayCast();
    }

    private void BulletRayCast()
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
            GameObject bulet = GetBulletInitialized();

            // Calcular la dirección hacia la posición del objetivo
            Vector3 direction = (targetPosition - bulet.transform.position).normalized;

            // Aplicar fuerza al objeto lanzado en la dirección del rayo
            bulet.GetComponent<Rigidbody>().AddForce(direction * powerBullet);
        }
        else
        {
            // Instanciar el objeto a lanzar en la posición del jugador
            GameObject bullet = GetBulletInitialized();
            // Iniciar la coroutine para mover el objeto hacia la posición del objetivo

            // Calcular la dirección hacia la posición del objetivo
            Vector3 direction = ray.direction.normalized;

            // Aplicar fuerza al objeto lanzado en la dirección del rayo
            bullet.GetComponent<Rigidbody>().AddForce(direction * powerBullet);
        }
    }

    private GameObject GetBulletInitialized()
    {
        // Instanciar el objeto a lanzar en la posición del jugador
        GameObject bullet = Instantiate(bulletPrefb, gunPlayer.transform.position, Quaternion.identity);
        BulletScript bulletScript = bullet.GetComponent<BulletScript>();    
        if(bulletScript != null)
        {
            MenuManagerApp menuManagerApp = gameObject.GetComponent<MenuManagerApp>();
            if(menuManagerApp != null)
            {
                bulletScript.SetMenuManager(menuManagerApp);
            } else
            {
                Debug.LogWarning("MenuMnagerApp.cs, no existe en el inspector en UiApp");
            }
        } else
        {
            Debug.LogWarning("bulletScript.cs, no existe en el Bullet prefab");
        }
        return bullet;
    }
}