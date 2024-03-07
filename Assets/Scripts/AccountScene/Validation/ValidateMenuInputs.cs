/*********************************************************************************
 * Nombre del Archivo:     ValidateMenuInputs.cs
 * Descripción:            Clase encargada de verificar que la entrada del usuario sea correcta, 
 *                         al crear un nuevo ítem.  
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


using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class ValidateMenuInputs : MonoBehaviour
{

    public bool IsValidEmail(string email, TextMeshProUGUI msjLoginResult)
    {
        // Defines a regular expression to validate emails.
        string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        // Checks if the user input matches the pattern.
        if (Regex.IsMatch(email, emailPattern))
        {
            return true;
        }
        else
        {
            msjLoginResult.SetText("Formato de email no valido!");
            return false;
        }
    }

    public bool IsValidPassword(TMP_InputField password, TextMeshProUGUI msjLoginResult)
    {

        if (password.text.Length > 5)
        {
            return true;
        }
        else
        {
            msjLoginResult.SetText("La contraseña debe ser mayor que cinco caracteres");
            return false;
        }
    }

    public bool IsFormatPasswordCorrect(TMP_InputField password, TMP_InputField rePassword, TextMeshProUGUI msjLoginResult)
    {
        if(password.text.Length > 5 && rePassword.text.Length > 5) {
        
            if(!password.text.Equals(rePassword.text))
            {
                msjLoginResult.SetText("Las contraseñas no coinciden");
                return false;
            }
            return true;
        } else
        {
            msjLoginResult.SetText("La contraseña debe ser mayor que cinco caracteres");
            return false;
        }
    }

}
