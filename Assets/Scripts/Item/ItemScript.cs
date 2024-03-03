/*********************************************************************************
 * Nombre del Archivo:     ItemScrip.cs
 * Descripción:            Script encargado de habilitar algunos funciones para nuestro ítem 
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

public class ItemScript : MonoBehaviour
{
    private BoxCollider boxCollider;
    private Rigidbody rb;

    // Start is called before the first frame update
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();

        if (CheckItem())
        {
            boxCollider.enabled = false;
            rb.useGravity = false;
        }
    }

    /// <summary>
    /// habilitamos el box collider y la gravedad una vez que ya hayan sido ordenados
    /// desde el ManageItems.cs, ya que de otra menera
    /// colisionarán unos con otros. 
    /// </summary>
    /// <returns></returns>
    public void EnablePhysicsItem()
    {
        if (!boxCollider.enabled && !rb.useGravity)
        {
            if (CheckItem())
            {
                boxCollider.enabled = true;
                rb.useGravity = true;
            }
        }
    }

    private bool CheckItem()
    {
        if (rb == null || boxCollider == null)
        {
            Debug.LogWarning("No hay un box collider ó un Rigidbody en el ítem");
            return false;
        }
        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Equals("DeadZone"))
        {
            RestItem();
        }
    }

    /// <summary>
    /// Si se cae el ítem lo trasladamos a la posicion de nuestro ItemSceneConfig (Hierarchy)
    /// </summary>
    private void RestItem()
    {
        gameObject.transform.position = gameObject.transform.parent.position;
    }
}
