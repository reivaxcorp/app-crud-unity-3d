/*********************************************************************************
 * Nombre del Archivo:     ItemRemote.cs
 * Descripción:            La representación remota de nuestro ítem, el que escribiremos
 *                         en RealtimeDatabase.  
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


using System;
using System.Collections.Generic;
 /// <summary>
/// El item que salvaremos remotamente.
/// </summary>
public class ItemRemote 
{
    public string Id {  get; set; }
    public string Name { get; set; }
    public string ImageName { get; set; }
    public long CreationDate { get; set; }

    public ItemRemote(){}

    public ItemRemote(string name, string imageName)
    {
        Name = name;
        ImageName = imageName;
    }

    public ItemRemote(string id, string name, string imageName, long creationDate)
    {
        Id = id;
        Name = name;
        ImageName = imageName;
        CreationDate = creationDate;
    }
     
    public Dictionary<string, Object> ToDictionary()
    {
        Dictionary<string, Object> result = new Dictionary<string, Object>();
        result["id"] = Id;
        result["name"] = Name;
        result["image_name"] = ImageName;
        result["creation_date"] = CreationDate;

        return result;
    }
}
