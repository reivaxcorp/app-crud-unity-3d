/*********************************************************************************
 * Nombre del Archivo:     MyRepository.cs
 * Descripción:            Patrón repositorio, nos ayudará a comunicarnos con la base de datos local
 *                         y remota, con sus interfaces correspondientes, para aislar la capa de datos de la
 *                         aplicación.  
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

using System.Collections.Generic;
using System.Threading.Tasks;
 
public class MyRepository : IRepositoryLocal, IRepositoryRemote
{
    private IRepositoryLocal localDb;
    private IRepositoryRemote remoteDb;


    public MyRepository(IRepositoryLocal localDb, IRepositoryRemote remoteDb)
    {
        this.localDb = localDb;
        this.remoteDb = remoteDb;
    }

    public async Task<bool> DeleteItemRemoteById(string id, IResult iResultUi)
    {
        return await remoteDb.DeleteItemRemoteById(id, iResultUi);
    }
   
    public async Task<List<ItemLocal>> GetLocalItemsAsync()
    {
        return await localDb.GetLocalItemsAsync();
    }

    public async Task SaveLocalItemsAsync(List<ItemLocal> listItemsLocal)
    {
        await localDb.SaveLocalItemsAsync(listItemsLocal);
    }

    public async Task<bool> SaveItemRemote(ItemRemote itemRemote, IResult resultUi)
    {
        return await remoteDb.SaveItemRemote(itemRemote, resultUi);
    }

    public async Task<bool> UpdateItemRemote(ItemRemote itemRemote, IResult resultUi)
    {
        return await remoteDb.UpdateItemRemote(itemRemote, resultUi);
    }

    public async Task<ItemLocal> GetLocalItemById(string id)
    {
        return await localDb.GetLocalItemById(id);
    }

    public RemoteDb GetRemoteDb()
    {
        return remoteDb as RemoteDb;
    }

    public LocalDb GetLocalDb()
    {
        return localDb as LocalDb;
    }
}