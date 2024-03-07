/*********************************************************************************
 * Nombre del Archivo:     AccountAuthResult.cs
 * Descripción:            Clase que nos ayudará en manejar los resultados de auntentificación de el 
 *                         usuario, inicio sesión, creación de cuentas, login. Que se enviará a la 
                           Ui de la aplicación
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


public enum AuthType
{
    LOGOUT, LOGIN_SUCCESS, LOGIN_FAILURE, LOGIN_CANCEL,
    CREATE_ACCOUNT_SUCCESS, CREATE_ACCOUNT_FAILURE, CREATE_ACCOUNT_CANCEL,
    SEND_MAIL_VERIFICATION_SUCCESS, SEND_MAIL_VERIFICATION_CANCEL, SEND_MAIL_VERIFICATION_FAILURE
}

/// <summary>
/// Get result when we do auth actions.
/// </summary>
public class AccountAuthResult
{

    public string Message { get => _message; private set => _message = value; }
    public AuthType AuthType { get => _authType; private set => _authType = value; }

    private string _message;
    private AuthType _authType;

    public AccountAuthResult(AuthType authType, string message)
    {
        Message = message;
        AuthType = authType;
    }
}