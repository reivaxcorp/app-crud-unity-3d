/*********************************************************************************
 * Nombre del Archivo:     ProgressText.cs
 * Descripción:            Clase que animará un texto cuando creamos un ítem.  
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

using System.Collections;
using TMPro;
using UnityEngine;

public class ProgressText : MonoBehaviour
{
    private bool startAnimText;
    private Coroutine animationCoroutine; // Almacena la referencia a la corrutina

    public void StartProgressTextAnimation(string text, TextMeshProUGUI textToAnim)
    {
        this.startAnimText = true;
        textToAnim.color = Color.blue;
        animationCoroutine = StartCoroutine(AnimateText(text, textToAnim));
    }
    public void StopProgressTextAnimation()
    {
        this.startAnimText = false;
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }
    }

    private IEnumerator AnimateText(string text, TextMeshProUGUI textToAnim)
    {
        while (startAnimText) 
        {
            textToAnim.text = text + " .";
            yield return new WaitForSeconds(0.5f);

            textToAnim.text = text + " ..";
            yield return new WaitForSeconds(0.5f);

            textToAnim.text = text + " ...";
            yield return new WaitForSeconds(0.5f);

            textToAnim.text = text;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
